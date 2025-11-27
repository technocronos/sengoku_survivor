using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector3 dir;
    private int damage = 10;
    public void Setup(Vector3 dir)
    {
        this.dir = dir; 
    }

    void Update()
    {
        var pos = Camera.main.WorldToViewportPoint(transform.position);
        if (pos.x > 1f || pos.x < 0 || pos.y > 1 || pos.y < 0) Remove();
        transform.position += Time.deltaTime * dir;
    }

    public void Remove()
    {
        Destroy(gameObject); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<Vs.Controllers.Game.Player>();
        if (player == null) return;
        
        player.Damage(damage);
        Remove();
    }
}
