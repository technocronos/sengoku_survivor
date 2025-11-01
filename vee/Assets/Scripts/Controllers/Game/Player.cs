using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.Game
{
    public sealed class Player : MonoBehaviour
    {
        public class PlayerStats
        {
            public int HpRate = 1000;
            public int SpdRate = 1000;
            public int ShotDuration = 1000;
            public int ShotLifeTime = 1000;
            public int ShotSpd = 1000;
            public int ShotArea = 1000;
            public int ExpRate = 1000;
            public int CoinsRate = 1000;
            public int AutoRecover = 0;
            public int MagnetArea = 1000;
        }

        public event System.Action<int, int> Damaged = (damage, hp) => { };
        public event System.Action<int, int> Recovered = (damage, hp) => { };

        [SerializeField]
        private SpriteAnimator animator;

        [SerializeField]
        private SpriteRenderer avatar;

        [SerializeField]
        private ParticleSystem blood;

        [SerializeField]
        private ParticleController[] shooters;

        [SerializeField]
        private GameObject direction;

        [SerializeField]
        private GameObject autoDir;

        [SerializeField]
        private FloatingJoystick joystick;

        public PlayerStats Stats { get; private set; }
        private int hp;
        private int hpMax = 30;
        private int speed = 2000;
        private int calcedHpMax;
        private int calcedSpeed;
        private float elapsed;

        private void Start()
        {
            // this.direction.transform.LookAt(Vector3.right);
            this.autoDir.transform.LookAt(Vector3.right);
        }

        public void Initialize(JsonObject raw)
        {
            this.hpMax = raw["hp"];
            this.speed = raw["speed"];
            this.Stats = new PlayerStats();
            this.CalcStats();
            this.hp = this.calcedHpMax;
            OnScreenUi.Instance.SetCurrHp(hp);
        }

        private void Update()
        {
            this.elapsed += Time.deltaTime;
            if (this.elapsed >= 1.0f)
            {
                this.elapsed -= 1.0f;
                this.Recover(Mathf.FloorToInt(this.calcedHpMax * this.Stats.AutoRecover / 1000.0f));
            }

            var horizontal = Input.GetAxis("Horizontal"); // this.joystick.Horizontal;
            var vertical = Input.GetAxis("Vertical");//1; // this.joystick.Vertical;

            var position = transform.localPosition;
            position.x += this.calcedSpeed / 1000.0f * Time.deltaTime * horizontal;
            position.y += this.calcedSpeed / 1000.0f * Time.deltaTime * vertical;
            // this.direction.transform.LookAt(position);
            position.x = Mathf.Clamp(position.x, -4.0f, 4.0f);
            position.y = Mathf.Clamp(position.y, -0.7f, 11.0f);
            this.transform.localPosition = position;
            if (new Vector2(horizontal, vertical).magnitude > 0)
            {
                this.animator.Play();
            }
            else
            {
                this.animator.Stop();
            }

            var dir = this.joystick.Horizontal;
            if (dir <= -0.1f || dir >= 0.1f)
            {
                this.avatar.flipX = dir < 0;
            }

            var enemies = GameManager.Instance.Enemies;
            var distance = float.MaxValue;
            var target = null as Enemy;
            foreach (var enemy in enemies)
            {
                if (enemy.IsDead)
                {
                    continue;
                }
                var d = Vector3.Distance(this.transform.position, enemy.transform.position);
                if (d < distance)
                {
                    target = enemy;
                }
            }
            if (target != null)
            {
                this.autoDir.transform.LookAt(target.transform.position);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag != "Enemy")
            {
                return;
            }
            var enemy = collision.gameObject.GetComponent<Enemy>();
            this.Damage(enemy.Hp);
            enemy.Death();
        }

        public void Damage(int value)
        {
            var soundId = Random.Range(0, 2) == 0 ? "damage_punch1": "damage_kick1";
            SoundService.Instance.PlaySe(soundId);

            this.blood.Play();
            this.hp -= value;
            if (this.hp < 0)
            {
                this.hp = 0;
            }
            OnScreenUi.Instance.SetCurrHp(hp);
            this.Damaged.Invoke(value, this.hp);
        }

        public void Recover(int value)
        {
            this.hp += value;
            if (this.hp > this.calcedHpMax)
            {
                this.hp = this.calcedHpMax;
            }
            OnScreenUi.Instance.SetCurrHp(hp);
            this.Recovered.Invoke(value, this.hp);
        }

        private void CalcStats()
        {
            this.calcedHpMax = Mathf.FloorToInt(this.hpMax * this.Stats.HpRate / 1000.0f);
            this.calcedSpeed = Mathf.FloorToInt(this.speed * this.Stats.SpdRate / 1000.0f);
            foreach (var i in this.shooters)
            {
                i.SetCalcedDuration((i.Duration + this.Stats.ShotDuration) / 1000.0f);
                i.SetCalcedLifeTime((i.LifeTime + this.Stats.ShotLifeTime) / 1000.0f);
                i.SetCalcedSpeed((i.Speed + this.Stats.ShotSpd) / 1000.0f);
                i.SetCalcedSize((i.Size + this.Stats.ShotArea) / 1000.0f);
            }
        }

        public void UpdateSkill(Skill skill)
        {
            switch (skill.Category)
            {
                case 101:
                    this.UpdateShooter(skill);
                    break;
                case 201:
                    this.UpgradeStats(skill);
                    break;
            }
        }

        private void UpdateShooter(Skill skill)
        {
            var shooter = System.Array.Find(this.shooters, i => i.SkillId == skill.SkillId);
            if (shooter != null)
            {
                shooter.Activate();
                shooter.SetAtk(skill.Atk);
                shooter.SetProjectile(skill.Projectile);
                shooter.SetCount(skill.Count);
                shooter.SetSize(skill.Size);
                shooter.SetSpeed(skill.Speed);
                shooter.SetDuration(skill.CoolTime);
                shooter.SetLifeTime(skill.LifeTime);
                this.CalcStats();
            }
            else
            {
                //todo: non-particle weaponの実装
            }
            
        }

        private void UpgradeStats(Skill skill)
        {
            var value = skill.EffectValue;
            switch (skill.EffectId)
            {
                case "hp":
                    this.Stats.HpRate = value;
                    break;
                case "speed":
                    this.Stats.SpdRate = value;
                    break;
                case "exp_rate":
                    this.Stats.ExpRate = value;
                    break;
                case "coins_rate":
                    this.Stats.CoinsRate = value;
                    break;
                case "shot_lifetime":
                    this.Stats.ShotLifeTime = value;
                    break;
                case "shot_spd":
                    this.Stats.ShotSpd = value;
                    break;
                case "shot_area":
                    this.Stats.ShotArea = value;
                    break;
                case "auto_recover":
                    this.Stats.AutoRecover = value;
                    break;
                case "magnet_area":
                    this.Stats.MagnetArea = value;
                    break;
            }
            this.CalcStats();
        }

        public int GetPlayerSpeedInt()
        {
            return calcedSpeed;
        }
    }
}
