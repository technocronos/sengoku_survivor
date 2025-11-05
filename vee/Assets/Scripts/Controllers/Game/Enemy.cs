using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.Game
{
    public sealed class Enemy : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer avatar;

        [SerializeField]
        private UnityEngine.UI.Text hpText;

        public bool IsTarget = true;
        public bool IsDead { get; private set; }

        public int Hp = 20;
        public int Atk { get; private set; }
        public float Spd = 0f;
        public int DropId { get; private set; }
        public int ExpAmount = 1;

        public SengokuSurvivors.EnemyType EnemyType { get; private set; } = SengokuSurvivors.EnemyType.Normal;

        private float hitElapsed;

        private void Awake()
        {
            this.hpText.text = $"{this.Hp}";
        }

        public void SetHp(int hp)
        {
            this.Hp = hp;
            this.hpText.text = $"{this.Hp}";
        }

        public void SetAtk(int atk)
        {
            this.Atk = atk;
        }

        public void SetSpd(float spd)
        {
            this.Spd = spd;
        }

        public void SetDropId(int dropId)
        {
            this.DropId = dropId;
        }

        public void SetEnemyType(SengokuSurvivors.EnemyType enemyType)
        {
            this.EnemyType = enemyType;
        }

        public void SetExpAmount(int expAmount)
        {
            this.ExpAmount = expAmount;
        }

        private void Update()
        {
            if (this.IsDead)
            {
                return;
            }
            if (this.hitElapsed > 0)
            {
                this.hitElapsed -= Time.deltaTime;
            }

            var player = GameManager.Instance.Player;
            var dir = Vector3.down; //player.transform.position - this.transform.position;
            var pos = this.transform.position;
            pos += this.Spd * Time.deltaTime * dir.normalized;
            this.transform.position = pos;
            RemoveIfPassed();

            if (this.avatar != null)
            {
                this.avatar.flipX = dir.x < 0;
            }
        }

        private void RemoveIfPassed()
        {
            if (Camera.main.WorldToViewportPoint(transform.position).y < 0) Death(true);
        }

        public void Death(bool force = false)
        {
            if (this.IsDead)
            {
                return;
            }
            this.IsDead = true;
            if (!force)
            {
                GameManager.Instance.AddCount();
                SengokuSurvivors.DropManager.Instance.DropExp(this.transform.position, this.ExpAmount);

                if(this.DropId > 0)
                    SengokuSurvivors.DropManager.Instance.DropItem(this.transform.position, this.DropId);

            }
            GameObject.Destroy(this.gameObject);
        }

        public void OnWeaponTrigger(int damage, string soundId)
        {
            if (this.IsDead)
            {
                return;
            }

            var isCritical = Random.Range(0, 4) == 0;
            this.Hit(damage, isCritical);
            //var soundId = isCritical ? "damage_cri" : ctr.GetSoundId();
            //SoundService.Instance.PlaySe(soundId);
        }

        private void OnParticleCollision(GameObject go)
        {
            if (this.IsDead)
            {
                return;
            }

            var ctr = go.GetComponent<ParticleController>();
            var isCritical = Random.Range(0, 4) == 0;
            this.Hit(ctr.Atk, isCritical);

            var soundId = isCritical ?  "damage_cri" : ctr.GetSoundId();
            SoundService.Instance.PlaySe(soundId);
        }

        private void OnParticleTriggerMT(GameObject go)
        {
            if (this.IsDead)
            {
                return;
            }
            if (this.hitElapsed > 0)
            {
                return;
            }
            this.hitElapsed += 1.0f;

            var ctr = go.GetComponent<ParticleController>();
            var isCritical = Random.Range(0, 4) == 0;
            this.Hit(ctr.Atk, isCritical);

            var soundId = isCritical ?  "damage_cri" : ctr.GetSoundId();
            SoundService.Instance.PlaySe(soundId);
        }

        private void Hit(int damage, bool isCritical)
        {
            var calcedDamage = Mathf.FloorToInt(damage * (isCritical ? 2.0f : 1.0f));

            DamageSpawner.Instance.Spawn(this.transform.position, calcedDamage, isCritical);

            this.Hp -= calcedDamage;
            this.hpText.text = $"{this.Hp}";
            if (this.Hp <= 0)
            {
                this.Death();
            }
        }
    }
}
