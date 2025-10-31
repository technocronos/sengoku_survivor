using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SengokuSurvivors
{
    public class SlashController : MonoBehaviour, IPlayerAttack
    {
        public Animator AttackEffectAnimator;
        [System.NonSerialized]
        public bool isAnimationPlaying = false;
        private void Start()
        {
            
            StartCoroutine(MainRoutine());
        }

        private IEnumerator MainRoutine()
        {
            while (true)
            {
                yield return null;
                yield return new WaitForSecondsRealtime(1f);
                
                List<Collider2D> results = new();
                var nn = GetComponent<Collider2D>().Overlap(results);
                for (int i = 0; i < nn; i++)
                {
                    var enemy = results[i].GetComponent<Vs.Controllers.Game.Enemy>();
                    if (enemy == null) continue;
                    enemy.OnWeaponTrigger(20, "");
                }

                AttackEffectAnimator.Play("Slash");
                isAnimationPlaying = true;
                while (isAnimationPlaying)
                {
                    yield return null;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isAnimationPlaying) return;
            var enemy = collision.gameObject.GetComponent<Vs.Controllers.Game.Enemy>();
            if (enemy != null) enemy.OnWeaponTrigger(20, "");
        }
    }
}
