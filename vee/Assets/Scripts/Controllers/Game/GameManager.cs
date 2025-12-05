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
        [SerializeField]
        private UnityEngine.UI.Text timeText;

        [SerializeField]
        private UnityEngine.UI.Text levelText;

        [SerializeField]
        private UnityEngine.UI.Image expImage;

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
        private PopupGameOver popupGameOver;

        [SerializeField]
        private PopupGameClear popupGameClear;

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

        private ThirdController thirdController;

        public Dictionary<int, int> onScreenEnemy = new Dictionary<int, int>();

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

            this.CalcLevel();
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
            popupGameOver.gameObject.SetActive(false);
            popupGameClear.gameObject.SetActive(false);

            onScreenEnemy.Clear();

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
            this.popupGameClear.Show(() =>
            {
                this.OnNext();
            });
        }

        public void GameOver()
        {
            PlayerPrefs.SetFloat("record", this.time);
            PlayerPrefs.Save();

            this.isStop = true;
            this.popupGameOver.Show(() =>
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


            // if (this.enemySpawner.IsCompleted && this.Enemies.Count(i => i.IsTarget) == 0)
            // {
            //     this.OnGameClear();
            //     return;
            // }

            //if (this.levelCalced > this.level)
            //{
            //    this.level++;
            //    this.CalcLevel();
            //    this.ShowLvUp();
            //}
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

            IrritateGauge.sprite = Resources.Load<Sprite>("Gauge/ira_" + i);

            StreamerImage.sprite = Resources.Load<Sprite>("Chara/chara_" + chara);
            if (chara == 0)
                CommentText.text = "配信来てくれてあざまるー<sprite name=\"1f60b\">";
            else if (chara == 1)
                CommentText.text = "何かイライラするなぁ・・<sprite name=\"2639\">";
            else if (chara == 2)
                CommentText.text = "つかマジでイライラ・・<sprite name=\"1f606\">";
            else if (chara == 3)
                CommentText.text = "イライラする～イライラする～うが～<sprite name=\"1f606\"><sprite name=\"1f606\">";
            else if (chara == 4)
                CommentText.text = "イライラマーーーーーックス<sprite name=\"1f606\"><sprite name=\"1f606\"><sprite name=\"1f606\"><sprite name=\"1f606\">";

        }

        public string GetTimeText()
        {
            return this.timeText.text; 
        }

        public void CalcLevel()
        {
            return;
            this.levelText.text = $"LV{this.level}";
            var prev = this.levelMst.Find(i => i["level"] == this.level);
            var next = this.levelMst.Find(i => i["level"] == this.level + 1);
            this.expImage.fillAmount = (float)(this.exp - prev["exp"]) / next["exp"];
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
            return;
            this.exp += Mathf.FloorToInt(value * this.Player.Stats.ExpRate / 1000.0f);

            var prev = this.levelMst.Find(i => i["level"] == this.level);
            var next = this.levelMst.Find(i => i["level"] == this.level + 1);
            this.expImage.fillAmount = (float)(this.exp - prev["exp"]) / next["exp"];

            var exp = this.exp - prev["exp"];
            var calced = this.levelMst.FindLast(i => i["exp"] <= exp);
            this.levelCalced = calced["level"];
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

        public void Bomb()
        {
            foreach (var i in this.Enemies)
            {
                i.Death();
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

            SoundService.Instance.PlaySe("levelup");
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
                this.GameOver();
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
