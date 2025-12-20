using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class Explosion : MonoBehaviour
    {
        // 敵へのダメージ値
        private int enemyDamage = 30;
        // プレイヤーへのダメージ値
        private int playerDamage = 10; 

        [SerializeField]
        private string animationName = "explosion"; // アニメーション名

        private float explosionRadius; // 爆発の範囲（CircleCollider2Dから自動取得）
        private Animator animator;
        private HashSet<GameObject> damagedObjects = new HashSet<GameObject>(); // 既にダメージを与えたオブジェクトを記録

        private void Awake()
        {
            // CircleCollider2Dから半径を自動取得
            CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
            if (circleCollider != null)
            {
                explosionRadius = circleCollider.radius;
            }
            else
            {
                Debug.LogWarning("Explosion: CircleCollider2D component not found! Using default radius.");
                explosionRadius = 2.0f;
            }
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
            if (animator != null)
            {
                SoundService.Instance.PlaySe("se_explosionshort");
                
                // アニメーションを再生
                animator.Play(animationName);
                // アニメーション終了まで待機してから破棄
                StartCoroutine(WaitForAnimationAndDestroy());
            }
            else
            {
                Debug.LogWarning("Explosion: Animator component not found!");
            }

            // 範囲内の敵とプレイヤーにダメージを与える
            DealDamageInRadius();
        }

        private void DealDamageInRadius()
        {
            // 範囲内のすべてのCollider2Dを取得
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

            foreach (Collider2D collider in colliders)
            {
                // 既にダメージを与えたオブジェクトはスキップ
                if (damagedObjects.Contains(collider.gameObject))
                {
                    continue;
                }

                // 敵にダメージ
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null && !enemy.IsDead)
                {
                    enemy.OnWeaponTrigger(enemyDamage, "damage_explosion", 0f, 0f);
                    damagedObjects.Add(collider.gameObject);
                    continue;
                }

                // プレイヤーにダメージ
                Player player = collider.GetComponent<Player>();
                if (player != null)
                {
                    player.Damage(playerDamage);
                    damagedObjects.Add(collider.gameObject);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 既にダメージを与えたオブジェクトはスキップ
            if (damagedObjects.Contains(collision.gameObject))
            {
                return;
            }

            // 敵にダメージ
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead)
            {
                enemy.OnWeaponTrigger(enemyDamage, "damage_explosion", 0f, 0f);
                damagedObjects.Add(collision.gameObject);
                return;
            }

            // プレイヤーにダメージ
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.Damage(playerDamage);
                damagedObjects.Add(collision.gameObject);
            }
        }

        private IEnumerator WaitForAnimationAndDestroy()
        {
            if (animator != null)
            {
                yield return null; // 1フレーム待機してアニメーションが開始されるのを待つ
                yield return new WaitForAnimation(animator, 0); // アニメーション終了まで待機
            }
            else
            {
                // Animatorがない場合は一定時間待機（フォールバック）
                yield return new WaitForSeconds(1.0f);
            }

            // アニメーション終了後にオブジェクトを破棄
            Destroy(gameObject);
        }
    }
}
