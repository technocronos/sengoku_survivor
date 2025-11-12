using UnityEngine;

public class ShurikenProjectile : MonoBehaviour
{
    private Vector3 movingDir = Vector3.up;
    private float speed = 5f;
    private ProjectileController bowController;
    private Vs.Controllers.Game.Enemy target;

    private void Awake()
    {
        bowController = FindAnyObjectByType<ProjectileController>();
    }

    public void StartMoving(Vector3 start, Vector3 dir)
    {
        transform.position = start;
        movingDir = dir;
    }

    public void StartMoving(Vector3 start, Vs.Controllers.Game.Enemy target)
    {
        transform.position = start;
        this.target = target;
    }

    private void Update()
    {
        if (target == null)
        {
            bowController.RemoveProjectile(this);
            return;
        }
        movingDir = (target.transform.position - transform.position).normalized;
        transform.position += speed * Time.deltaTime * movingDir;
        var posOnScreen = Camera.main.WorldToViewportPoint(transform.position);
        if (posOnScreen.x > 1 || posOnScreen.x < 0 || posOnScreen.y < 0 || posOnScreen.y > 1)
        {
            bowController.RemoveProjectile(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Vs.Controllers.Game.Enemy>() != target) return;
        bowController.RemoveProjectile(this);
        target.OnWeaponTrigger(bowController.ShurikenDamage, "");
    }
}
