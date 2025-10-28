using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.Game
{
    public sealed class ItemSpawner : SingletonMonoBehaviour<ItemSpawner>
    {
        [SerializeField]
        private Transform world;

        [System.Serializable]
        private class Pair
        {
            public string Key;
            public Item Value;
        }

        [SerializeField]
        private Pair[] prefabs;

        public void Spawn(Vector3 position)
        {
            var prefab = this.prefabs[Random.Range(0, this.prefabs.Length)];
            this.Spawn(position, prefab.Key);
        }

        public void Spawn(Vector3 position, string type)
        {
            var prefab = System.Array.Find(this.prefabs, i => i.Key == type);
            GameObject.Instantiate(prefab.Value, position, Quaternion.identity, this.world);
        }
    }
}
