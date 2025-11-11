using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyGame;

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
        private PopupPause popupPause;

        [SerializeField]
        private PopupGameOver popupGameOver;

        [SerializeField]
        private PopupGameClear popupGameClear;

        public Player Player { get; private set; }
        public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
        public List<Box> Boxes { get; private set; } = new List<Box>();

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

        public SkillManager SkillManager = new SkillManager();

        override protected void OnAwake()
        {
            //GameのシーンからでもエディターでプレイできるようにBootstrapからロード
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
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
            var skillMst = Backend.MstDatas.Instance.Get("drop_mst");
            this.SkillManager.Initialize(skillMst);
            this.Player = GameObject.FindObjectOfType<Player>();
            this.Player.Damaged += this.OnDamaged;
            this.Player.Initialize(playerMst[0]);

            {
                var skill = this.SkillManager.UpgradeSkill(initialWeaponId, 0);
                this.Player.UpdateSkill(skill);
            }

            var record = PlayerPrefs.GetFloat("record", 0);
            var min = Mathf.FloorToInt(record / 60);
            var sec = Mathf.FloorToInt(record % 60);
            this.recordText.text = $"TIME：{min:00}:{sec:00}";
        }

        public void Play()
        {
            this.isStop = false;
        }

        private void OnGameClear()
        {
            this.isStop = true;
            this.popupGameClear.Show(() =>
            {
                this.OnNext();
            });
        }

        private void OnGameOver()
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

        private void Update()
        {
            if (this.isStop)
            {
                return;
            }

            this.Enemies = GameObject.FindObjectsOfType<Enemy>().ToList();
            this.Boxes = GameObject.FindObjectsOfType<Box>().ToList();

            if (!this.isStopTime)
            {
                this.time += Time.deltaTime;
            }
            var min = Mathf.FloorToInt(this.time / 60);
            var sec = Mathf.FloorToInt(this.time % 60);
            this.timeText.text = $"{min:00}:{sec:00}";

            // if (this.enemySpawner.IsCompleted && this.Enemies.Count(i => i.IsTarget) == 0)
            // {
            //     this.OnGameClear();
            //     return;
            // }
            if (this.levelCalced > this.level)
            {
                this.level++;
                this.CalcLevel();
                this.ShowLvUp();
            }
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
            this.Player.Recover(value);
        }

        public void AddCoins(int value)
        {
            // this.coins += Mathf.FloorToInt(value * this.Player.Stats.CoinsRate / 1000.0f);
            // this.coinsText.text = this.coins.ToString();
        }

        public void AddSkill(int skillId, int skillType)
        {
            if (skillId == 0)
            {
                this.ShowLvUp();
            }
            else
            {
                var skill = this.SkillManager.UpgradeSkill(skillId, skillType);
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

        private void ShowLvUp(int specificSkillId = 0)
        {
            if (this.isStop)
            {
                return;
            }
            this.isStop = true;

            List<JsonObject> skills;
            if (specificSkillId != 0)
            {
                // 特定のスキルIDの選択肢を取得
                skills = this.SkillManager.GetSelectableSkillsForSkillId(specificSkillId);
            }
            else
            {
                // 通常のレベルアップ選択肢
                skills = this.SkillManager.GetSelectableSkills();
            }
            this.popupLvUp.Show(skills);

            SoundService.Instance.PlaySe("levelup");
        }

        private void OnLvUpPopupSelected(int skillId, int type)
        {
            this.isStop = false;

            var skill = this.SkillManager.UpgradeSkill(skillId, type);
            this.Player.UpdateSkill(skill);
        }

        public void OnPauseClicked()
        {
            var current = this.SkillManager.GetCurrentSkills();
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
                this.OnGameOver();
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
