using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class BoxSpawner : MonoBehaviour
    {
        [SerializeField]
        private Transform world;

        [SerializeField]
        private Vector2 mapSize;

        [SerializeField]
        private float scale = 100;

        [SerializeField]
        private int count = 10;

        [SerializeField]
        private Box boxPrefab;

        private float elapsed;

        private readonly Queue<Box> boxCache = new Queue<Box>();

        private void Start()
        {
            for (var i = 0; i < this.count; i++)
            {
                var px = Random.Range(-1.0f, 1.0f) * this.mapSize.x / this.scale;
                var py = Random.Range(-1.0f, 1.0f) * this.mapSize.y / this.scale;
                var pos = new Vector3(px, py, 0);
                pos += pos.normalized * 5.0f;
                this.Spawn(pos);
            }
        }

        private void Update()
        {
            this.elapsed += Time.deltaTime;
            if (this.elapsed >= 60.0f)
            {
                this.elapsed -= 60.0f;
                for (var i = 0; i < this.count; i++)
                {
                    var px = Random.Range(-1.0f, 1.0f) * this.mapSize.x / this.scale;
                    var py = Random.Range(-1.0f, 1.0f) * this.mapSize.y / this.scale;
                    var pos = new Vector3(px, py, 0);
                    this.Spawn(pos);
                }
            }
        }

        private void Spawn(Vector3 pos)
        {
            Box box;
            if (boxCache.Count == 0)
            {
                box = Instantiate(this.boxPrefab, pos, Quaternion.identity, this.world);
            }
            else
            {
                box = boxCache.Dequeue();
                box.gameObject.SetActive(true);
            }
            
            GameManager.Instance.RegisterBox(box);
            box.SetSpawner(this);
        }

        public void Despawn(Box box)
        {
            GameManager.Instance.DeregisterBox(box);
            box.gameObject.SetActive(false);
            boxCache.Enqueue(box);
        }
    }
}
