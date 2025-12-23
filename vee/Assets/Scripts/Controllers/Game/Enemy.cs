using System.Collections;
using TMPro;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class Enemy : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer avatar;

        [SerializeField]
        private UnityEngine.UI.Text hpText;
        [SerializeField]
        private TextMeshProUGUI TextName;

        public bool IsTarget = true;
        public bool IsDead { get; private set; }

        public int enemyId;
        public int Hp = 20;
        public int Atk { get; private set; }
        public int DropId { get; private set; }
        public int ExpAmount = 1;
        public int Irritate { get; private set; }
        public int Score { get; private set; }

        private int irritate_point = 0;

        public SengokuSurvivors.EnemyType EnemyType { get; private set; } = SengokuSurvivors.EnemyType.Normal;

        private float hitElapsed;

        private EnemySpawner spawner;

        private bool isInKnockback = false;

        public bool isLimit { get; set; } = false;
        public string name { get; set; } = "";

        public void Initialize(EnemySpawner spawner, string name = "")
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

            if(name != "")
            {
                this.name = name;
                TextName.text = this.name;
                TextName.gameObject.SetActive(true);
            }
            else
            {
                TextName.gameObject.SetActive(false);
            }
        }

        public void SetEnemyId(int enemyId)
        {
            this.enemyId = enemyId;
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

        public void SetIrritate(int irritate)
        {
            this.Irritate = irritate;
            this.irritate_point = (int)(this.Irritate / 1000.0f);
        }

        public void SetScore(int score)
        {
            this.Score = score;
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

                //irritate_point >= 80 （イライラマックス）の時は2倍にする
                if (irritate_point >= 80)
                {
                    //this.ExpAmount *= 2;
                }

                SengokuSurvivors.DropManager.Instance.DropExp(this.transform.position, this.ExpAmount);

                GameManager.Instance.getCurrScore(this.enemyId, this.Score);

                if (this.EnemyType == SengokuSurvivors.EnemyType.Elite)
                    SengokuSurvivors.DropManager.Instance.DropItem(this.transform.position, this.DropId);
                if (this.EnemyType == SengokuSurvivors.EnemyType.Boss)
                {
                    // ボスの場合は死亡アニメーションを再生してからゲームクリア
                    StartCoroutine(PlayBossDeathAnimation());
                    return; // アニメーション終了後に処理が続く
                }
            }
            isInKnockback = false;
            StopAllCoroutines();
            spawner.Despawn(this);

            if (GameManager.Instance.onScreenEnemy.ContainsKey(this.enemyId))
            {
                int value = GameManager.Instance.onScreenEnemy[this.enemyId];
                if (value > 0)
                {
                    GameManager.Instance.onScreenEnemy[this.enemyId]--;
                }
            }
        }

        /// <summary>
        /// ボスの死亡アニメーションを再生（Time.timeScale = 0でも再生できるようにUnscaledTimeに設定）
        /// </summary>
        private IEnumerator PlayBossDeathAnimation()
        {
            Time.timeScale = 0.0f;

            // avatar（C109_0オブジェクト）からAnimatorを取得
            Animator bossAnimator = null;
            if (avatar != null)
            {
                bossAnimator = avatar.GetComponent<Animator>();
            }

            // 見つからない場合は子オブジェクト全体から検索（フォールバック）
            if (bossAnimator == null)
            {
                bossAnimator = GetComponentInChildren<Animator>();
            }

            AnimatorUpdateMode originalUpdateMode = AnimatorUpdateMode.Normal;
            
            if (bossAnimator != null)
            {
                originalUpdateMode = bossAnimator.updateMode;
                bossAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

                SoundService.Instance.PlaySe("decide");
                // アニメーションを再生
                bossAnimator.Play("EnemyDeath");
            }
            else
            {
                Debug.LogWarning("Enemy: Boss Animator not found!");
            }

            // アニメーション終了まで待機
            if (bossAnimator != null)
            {
                yield return null; // 1フレーム待機してアニメーションが開始されるのを待つ
                yield return new WaitForAnimation(bossAnimator, 0);
            }
            else
            {
                // Animatorがない場合は一定時間待機（フォールバック）
                yield return new WaitForSecondsRealtime(1.0f);
            }

            // アニメーション終了後にupdateModeを元に戻す
            if (bossAnimator != null)
            {
                bossAnimator.updateMode = originalUpdateMode;
                this.gameObject.SetActive(false);
            }

            // 通常の死亡処理を続行
            isInKnockback = false;
            StopAllCoroutines();
            spawner.Despawn(this);

            if (GameManager.Instance.onScreenEnemy.ContainsKey(this.enemyId))
            {
                int value = GameManager.Instance.onScreenEnemy[this.enemyId];
                if (value > 0)
                {
                    GameManager.Instance.onScreenEnemy[this.enemyId]--;
                }
            }
            Time.timeScale = 1.0f;

            // ゲームクリア
            GameManager.Instance.GameClear();
        }

        public bool OnWeaponTrigger(int damage, string soundId, float knockbackLength = 0f, float knockbackTime = 0f)
        {
            if (this.IsDead)
            {
                return false;
            }

            var isCritical = false;// Random.Range(0, 4) == 0;
            this.Hit(damage, isCritical);
            Knockback(knockbackLength * GameManager.Instance.buffKnockBackLengthMulti, 
                knockbackTime * GameManager.Instance.buffKnockBackTimeMulti);
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

        private void Knockback(float knockBackStrength, float knockbackDuration)
        {
            if (isInKnockback) return;
            isInKnockback = true;
            StartCoroutine(KnockbackRoutine(knockBackStrength, knockbackDuration));
        }

        private IEnumerator KnockbackRoutine(float knockbackStrength, float knockbackDuration)
        {
            var movement = GetComponent<SengokuSurvivors.IEnemyMovement>();
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
