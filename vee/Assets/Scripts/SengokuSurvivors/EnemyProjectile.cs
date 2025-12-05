using UnityEngine;
using Vs.Controllers.Game;

public class EnemyProjectile : MonoBehaviour
{
    private Vector3 dir;
    private int damage = 10;
    private int enemyId = 90000001;

    public void Start()
    {
        if (GameManager.Instance.onScreenEnemy.ContainsKey(this.enemyId))
        {
            GameManager.Instance.onScreenEnemy[this.enemyId]++;
        }
        else
        {
            GameManager.Instance.onScreenEnemy.Add(this.enemyId, 1);
        }
    }


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

        if (GameManager.Instance.onScreenEnemy.ContainsKey(this.enemyId))
        {
            int value = GameManager.Instance.onScreenEnemy[this.enemyId];
            if (value > 0)
            {
                GameManager.Instance.onScreenEnemy[this.enemyId]--;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<Vs.Controllers.Game.Player>();
        if (player == null) return;
        
        player.Damage(damage);
        Remove();
    }
}
