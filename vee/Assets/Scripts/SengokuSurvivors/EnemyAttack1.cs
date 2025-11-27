using System.Collections;
using UnityEngine;

namespace SengokuSurvivors
{
    public class EnemyAttack1 : MonoBehaviour, IEnemyAttack
    {
        public EnemyProjectile projectilePref;
        private void Start()
        {
            StartCoroutine(MainRoutine());
        }

        private IEnumerator MainRoutine()
        {
            while(true)
            {
                yield return null;
                if (Time.timeScale < float.Epsilon) continue;
                yield return new WaitForSeconds(Random.Range(1f, 5f));
                GetComponent<EnemyMovement2>().StopForAttack();
                yield return new WaitForSeconds(0.5f);
                //todo: 玉のキャッシュ
                var a = Instantiate(projectilePref, transform.position, Quaternion.identity, this.transform.parent);
                a.transform.Rotate(Vector3.right, -30f);
                Vector3 dir;
                if (Random.Range(0f, 1f) > 0.5f)//50%確率でプレイヤーに向けて投げる
                    dir = (FindAnyObjectByType<Vs.Controllers.Game.Player>().transform.position - transform.position).normalized;
                else
                    dir = (new Vector3(-1f, Random.Range(-1f, 2f), 0f)).normalized;//ある程度ランダム方向に投げる
                a.Setup(dir);
            }
        }
    }
}