using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.Game
{
    public sealed class GemSpawner : SingletonMonoBehaviour<GemSpawner>
    {
        [SerializeField]
        private Transform world;

        [SerializeField]
        private ItemGem[] gemPrefabs;

        public void Spawn(Vector3 position, int level)
        {
            GameObject.Instantiate(this.gemPrefabs[level - 1], position, Quaternion.identity, this.world);
        }
    }
}
