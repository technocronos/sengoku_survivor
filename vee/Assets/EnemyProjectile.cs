using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector3 dir;
    void Start()
    {
        dir = (new Vector3(-1f, Random.Range(-1f, 2f), 0f)).normalized;
    }

    void Update()
    {
        var pos = Camera.main.WorldToViewportPoint(transform.position);
        if (pos.x > 1f || pos.x < 0 || pos.y > 1 || pos.y < 0) Destroy(gameObject);
        transform.position += Time.deltaTime * dir;
    }
}
