using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector3 dir;
    private int damage = 10;
    void Start()
    {
        if (Random.Range(0f, 1f) > 0.5f)//50%確率でプレイヤーに向けて投げる
            dir = (FindAnyObjectByType<Vs.Controllers.Game.Player>().transform.position - transform.position).normalized;
        else
            dir = (new Vector3(-1f, Random.Range(-1f, 2f), 0f)).normalized;//ある程度ランダム方向に投げる
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
