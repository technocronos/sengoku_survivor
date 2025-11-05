using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SengokuSurvivors
{
    public class SlashController : MonoBehaviour, IPlayerAttack
    {
        public Animator AttackEffectAnimator;
        [System.NonSerialized]
        public bool isAnimationPlaying = false;

        private int damage = 20;
        private float cooldown = 2f;
        private int katanaWeaponId = 901;
        private float weaponSizeMulti = 1f;

        private void Start()
        {
            
            StartCoroutine(MainRoutine());
        }

        private IEnumerator MainRoutine()
        {
            while (true)
            {
                yield return null;
                var weaponData = Vs.Controllers.Game.GameManager.Instance.SkillManager
                    .GetCurrentSkills().Find(i => i.SkillId == katanaWeaponId);
                damage = weaponData.Atk;
                cooldown = weaponData.CoolTime / 1000f * weaponData.CoolTimeMulti;
                weaponSizeMulti = weaponData.SizeMulti;
                transform.localScale = new Vector3(weaponSizeMulti, weaponSizeMulti, 1);

                List<Collider2D> results = new();
                var nn = GetComponent<Collider2D>().Overlap(results);
                for (int i = 0; i < nn; i++)
                {
                    var enemy = results[i].GetComponent<Vs.Controllers.Game.Enemy>();
                    if (enemy == null) continue;
                    enemy.OnWeaponTrigger(damage, "");
                }

                AttackEffectAnimator.Play("Slash");
                isAnimationPlaying = true;
                while (isAnimationPlaying)
                {
                    yield return null;
                }
                yield return new WaitForSecondsRealtime(cooldown);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isAnimationPlaying) return;
            var enemy = collision.gameObject.GetComponent<Vs.Controllers.Game.Enemy>();
            if (enemy != null) enemy.OnWeaponTrigger(damage, "");
        }
    }
}
