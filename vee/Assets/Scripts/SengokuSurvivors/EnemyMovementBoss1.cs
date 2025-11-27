using System.Collections;
using UnityEngine;
namespace SengokuSurvivors
{
    public class EnemyMovementBoss1 : MonoBehaviour, IEnemyMovement
    {
        private Vector3 currPos;
        private float xDir = 1f;
        private float speed = 0.8f;
        private bool flagStopForAttack = false;

        private bool isKnockedBack = false;
        private float stopLength = 2f;
        private float stoppedTime = 0f;
        public bool isStopped { get { return Time.time - stoppedTime < stopLength; } }

        void Start()
        {
            if (Camera.main.WorldToViewportPoint(transform.position).x > 0.5f)
                xDir = 1f;
            else
                xDir = -1f;

            //speed += Random.Range(-speedDispersion, speedDispersion);
            StartCoroutine(MovingRoutine());
        }

        public void StopForAttack()
        {
            flagStopForAttack=true;
        }

        private IEnumerator MovingRoutine()
        {
            
            float targetPosViewportY = 0.8f;
            var yMax = Camera.main.transform.position.y + Mathf.Abs(Camera.main.transform.position.z) * Mathf.Tan(Mathf.PI / 3);
            var yMin = Camera.main.transform.position.y;
            var newTargetPos = new Vector3(0.1f, Mathf.Lerp(yMin, yMax, targetPosViewportY), 0f);
            
            transform.position = newTargetPos;
            var lastPositionX = transform.position.x;
            stoppedTime = Time.time - stopLength;
            while (true)
            {
                yield return null;
                if (Time.timeScale < 0f + float.Epsilon) continue;
                //if (Time.time - birthTime > lifetime)
                //{
                //    GetComponent<Collider2D>().enabled = false;
                //    while (true)
                //    {
                //        yield return null;
                //        transform.position += Time.deltaTime * 10f * Vector3.down;//Enemy.cs画面の下でDestroyされるので今のところ下に移動だけでOK
                //    }
                //}

                yMax = Camera.main.transform.position.y + Mathf.Abs(Camera.main.transform.position.z) * Mathf.Tan(Mathf.PI / 3);//画面の上
                yMin = Camera.main.transform.position.y;//画面の下

                //if (xDir > 0 && Camera.main.WorldToViewportPoint(transform.position).x > 0.7f) { xDir = -1f; }
                //else if (xDir < 0 && Camera.main.WorldToViewportPoint(transform.position).x < 0.3f) { xDir = 1f; }
                if (xDir > 0 && transform.position.x > 4.1f) {
                    stoppedTime = Time.time;
                    xDir = -1f; }
                else if (xDir < 0 && transform.position.x < -4.1f) {
                    stoppedTime = Time.time;
                    xDir = 1f; }
                else if (Mathf.Sign(lastPositionX) * Mathf.Sign(transform.position.x) < 0f)
                {
                    stoppedTime = Time.time;
                }
                lastPositionX = transform.position.x;

                var viewportYpos = Camera.main.WorldToViewportPoint(transform.position).y;
                //if (viewportYpos > 0.9f) targetPosViewportY = 0f;
                //else if (viewportYpos < 0.85f) targetPosViewportY = 1f;
                //yDir = Mathf.Sign(Mathf.Lerp(yMin, yMax, targetPosViewportY) - transform.position.y);

//                viewportYpos += (targetPosViewportY - viewportYpos) * Time.deltaTime * speedViewportY;

                if (!isKnockedBack)
                {
                    var pos = transform.position;
                    pos.y = Mathf.Lerp(yMin, yMax, targetPosViewportY);
                    if (!isStopped) { pos.x += xDir * Time.deltaTime * speed; }
                    transform.position = pos;
                }

                
            }
        }

        public void SetKnockbackState(bool isKnockedBack)
        {
            this.isKnockedBack = isKnockedBack;
        }

        public void Initialize()
        {
            
        }
    }
}