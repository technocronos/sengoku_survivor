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

        private readonly Queue<Damage> damageTextCache = new Queue<Damage>();

        public void Spawn(Vector3 position, int damage, bool isCritical)
        {
            var prefab = isCritical ? this.damageCriPrefab : this.damagePrefab;
            var go = (damageTextCache.Count > 0) ? damageTextCache.Dequeue() : Instantiate(prefab, world);
            go.transform.position = position;
            go.transform.rotation = Quaternion.identity;
            go.Show(this, damage);
        }

        public void Despawn(Damage damageText)
        {
            damageTextCache.Enqueue(damageText);
            damageText.gameObject.SetActive(false);
        }
    }
}
