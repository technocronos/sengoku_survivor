using UnityEngine;
namespace SengokuSurvivors
{
    public class EnemyFlying1 : MonoBehaviour, IEnemyMovement
    {
        private Vector3 currPos;
        private float moveRightSign = 1f;
        private float speed = 4f;
        private float speedDispersion = 0.1f;
        private float downSpeedCoeff = 0.15f;

        private bool isKnockedBack = false;

        public void Initialize()
        {
            
        }

        public void SetKnockbackState(bool isKnockedBack)
        {
            this.isKnockedBack = isKnockedBack;
        }

        void Start()
        {
            if (Camera.main.WorldToViewportPoint(transform.position).x > 0.5f)
                moveRightSign = 1f;
            else
                moveRightSign = -1f;

            speed += Random.Range(-speedDispersion, speedDispersion);
        }

        void Update()
        {
            if (isKnockedBack) return;

            if (moveRightSign > 0 && transform.position.x > 4.4f) { moveRightSign = -1f; }
            else if (moveRightSign < 0 && transform.position.x < -4.4f) { moveRightSign = 1f; }

            transform.position += Time.deltaTime * speed * 
                (moveRightSign * Vector3.right + downSpeedCoeff * Vector3.down).normalized;
        }
    }
}