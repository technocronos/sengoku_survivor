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
                var a = Instantiate(projectilePref, transform.position, Quaternion.identity, this.transform.parent);
                a.transform.Rotate(Vector3.right, -30f);
            }
        }
    }
}