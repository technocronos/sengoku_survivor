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

        public int Hp { get; private set; }
        public int Atk { get; private set; }
        public int Spd { get; private set; }
        public int DropId { get; private set; }

        private float hitElapsed;

        public void SetHp(int hp)
        {
            this.Hp = hp;
            this.hpText.text = $"{this.Hp}";
        }

        public void SetAtk(int atk)
        {
            this.Atk = atk;
        }

        public void SetSpd(int spd)
        {
            this.Spd = spd;
        }

        public void SetDropId(int dropId)
        {
            this.DropId = dropId;
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
            var dir = player.transform.position - this.transform.position;
            var pos = this.transform.position;
            pos += this.Spd / 1000.0f * Time.deltaTime * dir.normalized;
            this.transform.position = pos;

            if (this.avatar != null)
            {
                this.avatar.flipX = dir.x < 0;
            }
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
                //SengokuSurvivors.DropManager.Instance.Spawn(this.transform.position, this.DropId);
                SengokuSurvivors.DropManager.Instance.DropExp(this.transform.position,1);
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
