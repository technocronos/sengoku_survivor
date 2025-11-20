using UnityEngine;

namespace SengokuSurvivors
{
    public class ArrowProjectile : MonoBehaviour, IPlayerAttackDamageDealer
    {
        private Vector3 movingDir = Vector3.up;
        private float speed = 5f;
        private ProjectileController bowController;

        private void Awake()
        {
            bowController = FindAnyObjectByType<ProjectileController>();
        }

        public void StartMoving(Vector3 start, Vector3 dir)
        {
            transform.position = start;
            movingDir = dir;
        }

        private void Update()
        {
            transform.position += speed * Time.deltaTime * movingDir;
            var posOnScreen = Camera.main.WorldToViewportPoint(transform.position);
            if (posOnScreen.x > 1 || posOnScreen.x < 0 || posOnScreen.y < 0 || posOnScreen.y > 1)
            {
                bowController.RemoveProjectile(this);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var target = collision.gameObject.GetComponent<Vs.Controllers.Game.Enemy>();
            if (target == null) return;
            if (target.OnWeaponTrigger(bowController.ArrowDamage, ""))
            {
                bowController.RemoveProjectile(this);
            }
        }
    }
}