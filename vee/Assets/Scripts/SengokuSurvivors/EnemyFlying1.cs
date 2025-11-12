using UnityEngine;
namespace SengokuSurvivors
{
    public class EnemyFlying1 : MonoBehaviour, IEnemyMovement
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
        }

        void Update()
        {
            if (moveRightSign > 0 && Camera.main.WorldToViewportPoint(transform.position).x > 0.7f) { moveRightSign = -1f; }
            else if (moveRightSign < 0 && Camera.main.WorldToViewportPoint(transform.position).x < 0.3f) { moveRightSign = 1f; }

            transform.position += Time.deltaTime * speed * 
                (moveRightSign * Vector3.right + downSpeedCoeff * Vector3.down).normalized;
        }
    }
}