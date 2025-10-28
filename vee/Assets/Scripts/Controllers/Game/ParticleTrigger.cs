using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class ParticleTrigger : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem partilceSystem;

        private void Update()
        {
            var player = GameManager.Instance.Player;
            var particles = new ParticleSystem.Particle[this.partilceSystem.main.maxParticles];
            this.partilceSystem.GetParticles(particles);

            var list = new List<GameObject>();
            list.AddRange(GameManager.Instance.Enemies.ConvertAll(i => i.gameObject));
            list.AddRange(GameManager.Instance.Boxes.ConvertAll(i => i.gameObject));

            foreach (var particle in particles)
            {
                if (particle.remainingLifetime <= 0)
                {
                    continue;
                }
                var pos = player.transform.position + particle.position;
                var targets = list.FindAll(i => Vector3.Distance(i.transform.position, pos) <= 0.3f);
                foreach (var target in targets)
                {
                    target.SendMessage("OnParticleTriggerMT", this.partilceSystem.gameObject);
                }
            }
        }
    }
}
