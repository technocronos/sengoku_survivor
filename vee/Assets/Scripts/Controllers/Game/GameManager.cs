using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyGame;
using UNCHAIN.ThirdSdk;
using UnityEngine.UI;
using TMPro;

namespace Vs.Controllers.Game
{
    public sealed class GameManager : SingletonMonoBehaviour<GameManager>
    {
        public UnityEngine.UI.Text timeText;

        [SerializeField]
        private UnityEngine.UI.Text coinsText;

        [SerializeField]
        private UnityEngine.UI.Text countText;

        [SerializeField]
        private UnityEngine.UI.Text recordText;

        [SerializeField]
        private EnemySpawner enemySpawner;

        [SerializeField]
        private PopupLvup popupLvUp;

        [SerializeField]
        private Image IrritateGauge;

        [SerializeField]
        private TextMeshProUGUI CommentText;

        [SerializeField]
        private Image StreamerImage;

        [SerializeField]
        private PopupPause popupPause;

        [SerializeField]
        private PopupResult popupResult;

        [SerializeField]
        private TextMeshProUGUI CurrScore;

        [SerializeField]
        private Transform world; // 爆発などのエフェクトを生成する親オブジェクト

        [SerializeField]
        private GameObject explosionPrefab; // 爆発プレハブ

        [SerializeField]
        private GameObject IrritateMaxEffects;

        [SerializeField]
        private Animator irritate_animator;

        public Player Player;
        public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
        public List<Box> Boxes { get; private set; } = new List<Box>();

        public float buffKnockBackLengthMulti = 1f;
        public float buffKnockBackTimeMulti = 1f;

        private bool isStop = true;
        private float time;
        private bool isStopTime = false;
        private int level = 1;
        private int levelCalced = 1;
        private int exp;
        private int expToLevelUp = 10;
        private int coins;
        private int count;

        private List<JsonObject> levelMst;
        private const int initialWeaponId = 901;

        public EquipmentManager EquipmentManager = new EquipmentManager();

        private int previousChara = -1; // 前回のcharaの値を保持
        private float commentTimer = 0f; // コメント表示用のタイマー
        private const float commentInterval = 10f; // コメント表示間隔（10秒）

        private bool explosion_flg = false;
        private bool irritate_max_flg = false;
        
        private const int irritate_explosion_count = 7;

        /// <summary>
        /// text_mst.csvからシンボルに基づいてテキストを取得する
        /// 同じシンボルで複数のテキストがある場合はランダムに選ぶ
        /// </summary>
        /// <param name="symbol">シンボル名（例: "broadcast_1"）</param>
        /// <returns>テキスト（見つからない場合は空文字列）</returns>
        public string GetTextFromMst(string symbol)
        {
            var textMst = Backend.MstDatas.Instance.Get("text_mst");
            // シンボルでフィルタリング（明示的に文字列に変換）
            var texts = textMst.FindAll(i => (string)i["symbol"] == symbol);
            if (texts == null || texts.Count == 0)
            {
                return "";
            }
            // 同じシンボルで複数のテキストがある場合はランダムに選ぶ
            var randomIndex = Random.Range(0, texts.Count);
            return (string)texts[randomIndex]["Ja"];
        }

        private ThirdController thirdController;

        public Dictionary<int, int> onScreenEnemy = new Dictionary<int, int>();

        public Queue<int> onScreenTreasure = new Queue<int>();
        public int onScreenTreasureCount = 0;

        public Dictionary<int, int> ScoreStore = new Dictionary<int, int>();

        public int totalScore = 0;

        private const int ScoreExpId = 1;
        private const int ScoreExp = 10;

        override protected void OnAwake()
        {
            //GameのシーンからでもエディターでプレイできるようにBootstrapからロード
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                return;
            }
            thirdController = FindAnyObjectByType<ThirdController>();
            thirdController.Connect();
        }

        private void OnDestroy()
        {
            if (thirdController == null) return;
            thirdController.Disconnect();
        }

        public void Initialize()
        {
            this.popupLvUp.Selected += this.OnLvUpPopupSelected;

            OnScreenUi.Instance.SetExp(exp, expToLevelUp);
            OnScreenUi.Instance.SetCurrLevel(level);

            this.coinsText.text = this.coins.ToString();
            this.countText.text = this.count.ToString();

            var playerMst = Backend.MstDatas.Instance.Get("player_mst");
            var dropMst = Backend.MstDatas.Instance.Get("drop_mst");
            var weaponsMst = Backend.MstDatas.Instance.Get("weapons_mst");
            var accessoriesMst = Backend.MstDatas.Instance.Get("accessories_mst");
            var itemMst = Backend.MstDatas.Instance.Get("item_mst");

            this.EquipmentManager.Initialize(dropMst, weaponsMst, accessoriesMst, itemMst);
            this.Player.Damaged += this.OnDamaged;
            this.Player.Initialize(playerMst[0]);

            {
                var skill = this.EquipmentManager.UpgradeSkill(initialWeaponId);
                this.Player.UpdateSkill(skill);
            }

            var record = PlayerPrefs.GetFloat("record", 0);
            var min = Mathf.FloorToInt(record / 60);
            var sec = Mathf.FloorToInt(record % 60);
            this.recordText.text = $"TIME：{min:00}:{sec:00}";

            //初期非表示
            popupLvUp.gameObject.SetActive(false);
            popupPause.gameObject.SetActive(false);
            popupResult.gameObject.SetActive(false);

            onScreenEnemy.Clear();
            UpdateCurrScore();

            onScreenTreasure.Clear();
            onScreenTreasureCount = 0;
        }

        public void Play()
        {
            this.isStop = false;
        }

        public void GameClear(float delay = 0f)
        {
            Invoke(nameof(GameClear), delay);
        }

        private void GameClear()
        {
            this.isStop = true;
            var result = new PopupResult.GameResult(PopupResult.GameResult.Win);
            this.popupResult.Show(result, () =>
            {
                this.OnNext();
            });
        }

        public void GameOver()
        {
            PlayerPrefs.SetFloat("record", this.time);
            PlayerPrefs.Save();

            Player.gameObject.SetActive(false);

            this.isStop = true;
            var result = new PopupResult.GameResult(PopupResult.GameResult.Lose);
            this.popupResult.Show(result, () =>
            {
                this.OnNext();
            });
        }

        public void OnNext()
        {
            var context = new Controllers.Game.Game.Context();
            ViewService.Instance.ChangeView(context);
        }

        public void RegisterEnemy(Enemy enemy)
        {
            Enemies.Add(enemy);
        }

        public void DeregisterEnemy(Enemy enemy)
        {
            if (!Enemies.Contains(enemy)) return;
            Enemies.Remove(enemy);
        }

        public void RegisterBox(Box box)
        {
            Boxes.Add(box);
        }

        public void DeregisterBox(Box box)
        {
            if (!Boxes.Contains(box)) return;
            Boxes.Remove(box);
        }

        private void Update()
        {
            // Escキーでポーズ/ポーズ解除（ゲームオーバー中は無効）
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (this.popupResult != null && this.popupResult.gameObject.activeSelf)
                {
                    // ゲームオーバー中は何もしない
                    return;
                }
                
                if (this.popupPause != null && this.popupPause.gameObject.activeSelf)
                {
                    // ポーズ中はEscキーでポーズ解除
                    this.popupPause.OnClicked();
                }
                else if (this.popupPause != null)
                {
                    // ポーズ中でない場合はポーズを開く
                    this.OnPauseClicked();
                }
            }

            if (this.isStop)
            {
                return;
            }

            if (!this.isStopTime)
            {
                this.time += Time.deltaTime;
            }
            var min = Mathf.FloorToInt(this.time / 60);
            var sec = Mathf.FloorToInt(this.time % 60);
            this.timeText.text = $"{min:00}:{sec:00}";

#if DEVELOP
            OnScreenUi.Instance.SetOnScreenEnemy(onScreenEnemy);
#endif
            var irritate_point = OnScreenUi.Instance.GetIrritatePoint(onScreenEnemy);
            SetIrritateInfo(irritate_point);

            // 90以上になった瞬間に0.5秒おきに5回連続で爆発
            if (irritate_point >= 90 && !explosion_flg)
            {
                StartCoroutine(SpawnExplosionsSequence(irritate_explosion_count));
                explosion_flg = true;
            }
            // 90以下に下がったらフラグをリセット
            else if(irritate_point < 90)
            {
                explosion_flg = false;
            }

            // 10秒ごとにランダムコメントを表示
            commentTimer += Time.deltaTime;
            if (commentTimer >= commentInterval)
            {
                commentTimer = 0f;
                ShowRandomComment();
            }
        }

        private void SetIrritateInfo(int irritate_point)
        {
            int i = 1;
            int chara = 0;

            if (irritate_point >= 0 && irritate_point < 10)
            {
                i = 1;
                chara = 0;
            }
            else if (irritate_point >= 10 && irritate_point < 20)
            {
                i = 2;
                chara = 0;
            }
            else if (irritate_point >= 20 && irritate_point < 30)
            {
                i = 3;
                chara = 1;
            }
            else if (irritate_point >= 30 && irritate_point < 40)
            {
                i = 4;
                chara = 1;
            }
            else if (irritate_point >= 40 && irritate_point < 50)
            {
                i = 5;
                chara = 2;
            }
            else if (irritate_point >= 50 && irritate_point < 60)
            {
                i = 6;
                chara = 2;
            }
            else if (irritate_point >= 60 && irritate_point < 70)
            {
                i = 7;
                chara = 3;
            }
            else if (irritate_point >= 70 && irritate_point < 80)
            {
                i = 8;
                chara = 3;
            }
            else if (irritate_point >= 80)
            {
                i = 9;
                chara = 4;
            }

            if(irritate_point >= 80)
            {
                IrritateMaxEffects.SetActive(true);
            }
            else
            {
                IrritateMaxEffects.SetActive(false);
            }

            IrritateGauge.sprite = Resources.Load<Sprite>("Gauge/ira_" + i);
            
            // charaが変わったかどうかを判定
            bool chara_change_flg = (chara != previousChara);
            
            // charaが変わった場合のみセリフを更新
            if (chara_change_flg)
            {

                StreamerImage.sprite = Resources.Load<Sprite>("Chara/chara_" + chara);

                // text_mst.csvからテキストを取得
                string symbol = $"broadcast_{chara + 1}";
                string text = GetTextFromMst(symbol);
                if (!string.IsNullOrEmpty(text))
                {
                    CommentText.text = text;
                }
                // 前回のcharaを更新
                previousChara = chara;
            }

        }

        /// <summary>
        /// text_mst.csvのrand_commentからランダムに1つ選んでCommentsUiに表示
        /// </summary>
        private void ShowRandomComment()
        {
            if (thirdController == null || thirdController.CommentsUi == null)
            {
                return;
            }

            string text = GetTextFromMst("rand_comment");
            if (!string.IsNullOrEmpty(text))
            {
                thirdController.CommentsUi.AddComment(text);
            }
        }

        public string GetTimeText()
        {
            return this.timeText.text; 
        }

        public void CalcLevel()
        {
            return;
        }

        public void AddExp(int value)
        {
            this.exp += value;
            if (this.exp >= expToLevelUp)
            {
                this.exp -= expToLevelUp;
                expToLevelUp += 3;//expToLevelUp = nextExpToLevelUp;
                this.level++;
                ShowLvUp();
            }
            OnScreenUi.Instance.SetExp(this.exp, expToLevelUp);
            OnScreenUi.Instance.SetCurrLevel(this.level);

            getCurrScore(ScoreExpId, ScoreExp);

            return;
        }

        public void Recover(int value)
        {
            this.Player.RecoverHp(value);
        }

        public void AddCoins(int value)
        {
            // this.coins += Mathf.FloorToInt(value * this.Player.Stats.CoinsRate / 1000.0f);
            // this.coinsText.text = this.coins.ToString();
        }

        public void AddSkill(int skillId)
        {
            if (skillId == 0)
            {
                this.ShowLvUp();
            }
            else
            {
                var skill = this.EquipmentManager.UpgradeSkill(skillId);
                this.Player.UpdateSkill(skill);
            }
        }

        public void Add()
        {
            this.ShowLvUp();
        }

        public void AddCount()
        {
            this.count++;
            this.countText.text = this.count.ToString();
        }

        public void getCurrScore(int id, int score)
        {
            if(score > 0)
            {
                if (GameManager.Instance.ScoreStore.ContainsKey(id))
                {
                    GameManager.Instance.ScoreStore[id] += score;
                }
                else
                {
                    GameManager.Instance.ScoreStore.Add(id, score);
                }
                UpdateCurrScore();
            }
        }

        public void UpdateCurrScore()
        {
            this.totalScore = 0;
            foreach (var score in this.ScoreStore.Values)
            {
                this.totalScore += score;
            }
            if (this.CurrScore != null)
            {
                this.CurrScore.text = this.totalScore.ToString("N0");
            }
        }

        public void Bomb()
        {
            foreach (var i in this.Enemies)
            {
                i.Death();
            }
        }

        /// <summary>
        /// IrritateMaxEffectsのアニメーションを再生し、終了後に爆発シーケンスを開始する
        /// </summary>
        private IEnumerator PlayIrritateMaxEffects()
        {
            // IrritateMaxEffectsを表示
            IrritateMaxEffects.SetActive(true);

            // ゲームを停止
            Time.timeScale = 0.0f;

            // アニメーションを再生
            irritate_animator.Play("irritateeffects");

            // アニメーション終了まで待機
            yield return null; // 1フレーム待機してアニメーションが開始されるのを待つ
            yield return new WaitForAnimation(irritate_animator, 0);

            // ゲームを再開
            Time.timeScale = 1.0f;

            // IrritateMaxEffectsを非表示
            IrritateMaxEffects.SetActive(false);

            //爆発
            StartCoroutine(SpawnExplosionsSequence(irritate_explosion_count));
        }

        /// <summary>
        /// ステージ範囲内のランダムな位置に爆発を生成する
        /// </summary>
        /// <summary>
        /// 0.5秒おきにcount回連続で爆発を発生させるコルーチン
        /// </summary>
        private IEnumerator SpawnExplosionsSequence(int count)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnExplosion();
                yield return new WaitForSeconds(0.5f);
            }
        }

        public void SpawnExplosion()
        {
            if (explosionPrefab == null)
            {
                Debug.LogWarning("GameManager: Explosion prefab is not assigned!");
                return;
            }

            // カメラのビューポートからステージ範囲を取得（スクロールに対応）
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogWarning("GameManager: Main camera not found!");
                return;
            }

            float zDistance = -2 * mainCamera.transform.position.z;
            
            // ビューポートの各端のワールド座標を取得
            Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, zDistance));
            Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, zDistance));
            
            // ステージ範囲内のランダムな位置を生成
            float randomX = Random.Range(Player.stageMinX, Player.stageMaxX);
            float randomY = Random.Range(bottomLeft.y + 2, topRight.y - 5);

            Vector3 explosionPosition = new Vector3(randomX, randomY, 0);

            // 爆発を生成
            var explosionObj = Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);
            if (world != null)
            {
                explosionObj.transform.SetParent(world);
            }
        }

        public void Magnet()
        {
            var gems = GameObject.FindObjectsOfType<ItemGem>().ToList();
            foreach (var i in gems)
            {
                i.Obtain(this.Player.gameObject);
            }
        }

        private void ShowLvUp()
        {
            if (this.isStop)
            {
                return;
            }
            this.isStop = true;

            var skills = this.EquipmentManager.GetSelectableSkills();            
            this.popupLvUp.Show(skills);
        }

        private void OnLvUpPopupSelected(int skillId)
        {
            this.isStop = false;

            var skill = this.EquipmentManager.UpgradeSkill(skillId);
            this.Player.UpdateSkill(skill);
        }

        public void OnPauseClicked()
        {
            var current = this.EquipmentManager.GetCurrentSkills();
            this.popupPause.Show(current);
        }

        private void OnDamaged(int damage, int hp)
        {
            if (this.isStop)
            {
                return;
            }
            if (hp <= 0)
            {
                SoundService.Instance.PlaySe("decide");
                StartCoroutine(PlayPlayerDeathAnimation());
            }
        }

        /// <summary>
        /// プレイヤーの死亡アニメーションを再生（Time.timeScale = 0でも再生できるようにUnscaledTimeに設定）
        /// </summary>
        private IEnumerator PlayPlayerDeathAnimation()
        {
            Time.timeScale = 0.0f;
            Animator playerAnimator = Player.GetComponent<Animator>();
            AnimatorUpdateMode originalUpdateMode = AnimatorUpdateMode.Normal;
            
            if (playerAnimator != null)
            {
                originalUpdateMode = playerAnimator.updateMode;
                playerAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }

            yield return StartCoroutine(Player.PlayAnim("player_death"));

            // アニメーション終了後にupdateModeを元に戻す
            if (playerAnimator != null)
            {
                playerAnimator.updateMode = originalUpdateMode;
                GameOver();
            }
        }

        public void StopTime()
        {
            this.isStopTime = true;
        }

        public void Clear()
        {
            foreach (var i in this.Enemies)
            {
                i.Death(force: true);
            }
        }
    }
}
