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
            GameObject.Instantiate(this.boxPrefab, pos, Quaternion.identity, this.world);
        }
    }
}
