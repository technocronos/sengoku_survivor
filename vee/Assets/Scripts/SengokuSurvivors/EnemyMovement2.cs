using System.Collections;
using UnityEngine;
namespace SengokuSurvivors
{
    public class EnemyMovement2 : MonoBehaviour, IEnemyMovement
    {
        private Vector3 currPos;
        private float moveRightSign = 1f;
        private float speed = 4f;
        private float speedDispersion = 0.1f;
        private float downSpeedCoeff = 0.1f;

        void Start()
        {
            GetComponent<Vs.Controllers.Game.Enemy>().UseBaseMoving = false;
            if (Camera.main.WorldToViewportPoint(transform.position).x > 0.5f)
                moveRightSign = 1f;
            else
                moveRightSign = -1f;

            speed += Random.Range(-speedDispersion, speedDispersion);
            StartCoroutine(MovingRoutine());
        }

        private IEnumerator MovingRoutine()
        {
            float targetPosViewportY = 0.9f;
            float targetPosWorldX = 4f;
            var yMax = Camera.main.transform.position.y + Mathf.Abs(Camera.main.transform.position.z) * Mathf.Tan(Mathf.PI / 3);
            var yMin = Camera.main.transform.position.y;
            var newTargetPos = new Vector3(targetPosWorldX, Mathf.Lerp(yMin, yMax, targetPosViewportY), 0f);
            
            transform.position = newTargetPos;

            while (true)
            {
                yield return null;
                if (Time.timeScale < 0f + float.Epsilon) continue;

                yMax = Camera.main.transform.position.y + Mathf.Abs(Camera.main.transform.position.z) * Mathf.Tan(Mathf.PI / 3);
                yMin = Camera.main.transform.position.y;

                newTargetPos = new Vector3(targetPosWorldX, Mathf.Lerp(yMin, yMax, targetPosViewportY), 0f);
                transform.position += (newTargetPos - transform.position).normalized * Time.deltaTime * 3f;
                if (Camera.main.WorldToViewportPoint(transform.position).y > 0.8f) targetPosViewportY = 0.1f;
                else if (Camera.main.WorldToViewportPoint(transform.position).y < 0.2f) targetPosViewportY = 0.9f;
            }
        }
    }
}