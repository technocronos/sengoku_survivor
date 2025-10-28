using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.Game
{
    public sealed class DamageSpawner : SingletonMonoBehaviour<DamageSpawner>
    {
        [SerializeField]
        private Transform world;

        [SerializeField]
        private Damage damagePrefab;

        [SerializeField]
        private Damage damageCriPrefab;

        public void Spawn(Vector3 position, int damage, bool isCritical)
        {
            var prefab = isCritical ? this.damageCriPrefab : this.damagePrefab;
            var go = GameObject.Instantiate(prefab, position, Quaternion.identity, this.world);
            go.Show(damage);
        }
    }
}
