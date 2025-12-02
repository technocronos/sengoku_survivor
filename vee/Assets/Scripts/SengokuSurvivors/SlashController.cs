using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SengokuSurvivors
{
    public class SlashController : MonoBehaviour, IPlayerAttackController, IPlayerAttackDamageDealer
    {
        public Animator AttackEffectAnimator;
        [System.NonSerialized]
        public bool isAnimationPlaying = false;

        public int Damage { get { return damage; } }
        public float Cooldown { get { return cooldown; } }

        private int damage = 20;
        private float cooldown = 2f;
        private int weaponId = 901;
        private string weaponUseAnim = "Slash";
        private string soundId = "damage_slash_1";//"damage_slash_2";
        private float knockbackTime = 0f;
        private float knockbackLength = 0f;

        private void Start()
        {
            StartCoroutine(MainRoutine());
        }

        private IEnumerator MainRoutine()
        {
            while (true)
            {
                yield return null;
                UpdateWeaponParameters();

                List<Collider2D> results = new();
                var nn = GetComponent<Collider2D>().Overlap(results);
                for (int i = 0; i < nn; i++)
                {
                    var enemy = results[i].GetComponent<Vs.Controllers.Game.Enemy>();
                    DamageEnemy(enemy);
                }

                AttackEffectAnimator.Play(weaponUseAnim);
                isAnimationPlaying = true;
                Vs.SoundService.Instance.PlaySe(soundId);
                while (isAnimationPlaying)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(cooldown);
            }
        }

        private void UpdateWeaponParameters()
        {
            var weaponData = Vs.Controllers.Game.GameManager.Instance.EquipmentManager
                .GetCurrentSkillWithId(weaponId) as Vs.Controllers.Game.Weapon;
            if (weaponData == null) return;

            damage = weaponData.Atk;
            cooldown = weaponData.CoolTime;
            knockbackTime = weaponData.KnockbackTime;
            knockbackLength = weaponData.KnockbackLength;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isAnimationPlaying) return;
            var enemy = collision.gameObject.GetComponent<Vs.Controllers.Game.Enemy>();
            DamageEnemy(enemy);
        }

        private void DamageEnemy(Vs.Controllers.Game.Enemy enemy)
        {
            if (enemy == null) return;
            enemy.OnWeaponTrigger(damage, "", knockbackLength, knockbackTime);
        }
    }
}
