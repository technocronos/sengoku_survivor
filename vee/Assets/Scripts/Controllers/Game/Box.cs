using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class Box : MonoBehaviour
    {
        public bool IsDead { get; private set; }

        private void Awake()
        {
            GameManager.Instance.RegisterBox(this);
        }

        private void OnParticleCollision(GameObject go)
        {
            if (this.IsDead)
            {
                return;
            }
            this.Death();
        }

        private void OnParticleTriggerMT(GameObject go)
        {
            if (this.IsDead)
            {
                return;
            }
            this.Death();
        }

        public void Death()
        {
            if (this.IsDead)
            {
                return;
            }
            this.IsDead = true;
            ItemSpawner.Instance.Spawn(this.transform.position);
            GameManager.Instance.DeregisterBox(this);
            GameObject.Destroy(this.gameObject);
        }

        private void OnDestroy()
        {
            GameManager.Instance.DeregisterBox(this);
        }
    }
}
