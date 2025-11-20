using SengokuSurvivors;
using System.Collections;
using UnityEngine;

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
        public int DropId { get; private set; }
        public int ExpAmount = 1;

        [System.NonSerialized]
        public bool UseBaseMoving = true;

        public SengokuSurvivors.EnemyType EnemyType { get; private set; } = SengokuSurvivors.EnemyType.Normal;

        private float hitElapsed;

        private EnemySpawner spawner;

        private bool isInKnockback = false;
        private float knockbackDuration = 0.3f;

        public void Initialize(EnemySpawner spawner)
        {
            this.spawner = spawner;
            IsDead = false;

            SengokuSurvivors.IEnemyMovement movementComponent = GetComponent<SengokuSurvivors.IEnemyMovement>();
            if (movementComponent == null) { movementComponent = gameObject.AddComponent<SengokuSurvivors.EnemyMovementSimple>(); }
            movementComponent.Initialize();

            this.hpText.text = $"{this.Hp}";
            
            if (avatar.GetComponent<SengokuSurvivors.OnHitFlashingEffect>() == null)
            {
                avatar.gameObject.AddComponent<SengokuSurvivors.OnHitFlashingEffect>();
            }
            if (hpText != null)
            {
                hpText.gameObject.SetActive(false);
            }
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

            RemoveIfPassed();

            if (this.avatar != null)
            {
                //this.avatar.flipX = dir.x < 0;
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

                if(this.EnemyType != SengokuSurvivors.EnemyType.Normal)
                    SengokuSurvivors.DropManager.Instance.DropItem(this.transform.position, this.DropId);
            }
            isInKnockback = false;
            StopAllCoroutines();
            spawner.Despawn(this);
        }

        public bool OnWeaponTrigger(int damage, string soundId, float knockback = 0f)
        {
            if (this.IsDead)
            {
                return false;
            }

            var isCritical = false;// Random.Range(0, 4) == 0;
            this.Hit(damage, isCritical);
            Knockback(knockback);
            //var soundId = isCritical ? "damage_cri" : ctr.GetSoundId();
            //SoundService.Instance.PlaySe(soundId);
            return true;
        }

        private void OnParticleCollision(GameObject go)
        {
            if (this.IsDead)
            {
                return;
            }

            var ctr = go.GetComponent<ParticleController>();
            var isCritical = false;// Random.Range(0, 4) == 0;
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
            var isCritical = false;// Random.Range(0, 4) == 0;
            this.Hit(ctr.Atk, isCritical);

            var soundId = isCritical ?  "damage_cri" : ctr.GetSoundId();
            SoundService.Instance.PlaySe(soundId);
        }

        private void Hit(int damage, bool isCritical)
        {
            avatar.GetComponent<SengokuSurvivors.OnHitFlashingEffect>().TriggerMaterialChange();
            var calcedDamage = Mathf.FloorToInt(damage * (isCritical ? 2.0f : 1.0f));

            //DamageSpawner.Instance.Spawn(this.transform.position, calcedDamage, isCritical);

            this.Hp -= calcedDamage;
            this.hpText.text = $"{this.Hp}";
            if (this.Hp <= 0)
            {
                this.Death();
            }
        }

        private void Knockback(float knockBackStrength)
        {
            if (knockBackStrength <= float.Epsilon || isInKnockback) return;
            isInKnockback = true;
            StartCoroutine(KnockbackRoutine(knockBackStrength));
        }

        private IEnumerator KnockbackRoutine(float knockbackStrength)
        {
            var movement = GetComponent<IEnemyMovement>();
            movement.SetKnockbackState(true);
            float elapsed = 0f;
            while (elapsed < knockbackDuration)
            {
                yield return null;
                elapsed += Time.deltaTime;
                var pos = transform.localPosition;
                pos.y += Time.deltaTime * knockbackStrength;
                transform.localPosition = pos;
            }
            isInKnockback = false;
            movement.SetKnockbackState(false);
        }
    }
}
