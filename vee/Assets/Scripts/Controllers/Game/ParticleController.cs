using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class ParticleController : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem particle;

        public int SkillId;
        public string[] SoundIds;

        public int Atk { get; private set; }
        public int Projectile { get; private set; }
        public int Count { get; private set; }
        public float Duration { get; private set; }
        public float LifeTime { get; private set; }
        public float Speed { get; private set; }
        public float Size { get; private set; }

        public void Activate()
        {
            this.gameObject.SetActive(true);
        }

        public void SetAtk(int value)
        {
            this.Atk = value;
        }

        public void SetProjectile(int value)
        {
            this.Projectile = value;
            var burst = this.particle.emission.GetBurst(0);
            burst.count = value;
            this.particle.emission.SetBurst(0, burst);
        }

        public void SetCount(int value)
        {
            this.Count = value;
            var burst = this.particle.emission.GetBurst(0);
            burst.cycleCount = value;
            this.particle.emission.SetBurst(0, burst);
        }

        public void SetDuration(int value)
        {
            this.Duration = value;
        }

        public void SetLifeTime(float value)
        {
            this.LifeTime = value;
        }

        public void SetSpeed(float value)
        {
            this.Speed = value;
        }

        public void SetSize(float value)
        {
            this.Size = value;
        }

        public void SetCalcedDuration(float value)
        {
            var main = this.particle.main;
            this.particle.Stop();
            main.duration = value;
            this.particle.Play();
        }

        public void SetCalcedLifeTime(float value)
        {
            var main = this.particle.main;
            main.startLifetime = value;
        }

        public void SetCalcedSpeed(float value)
        {
            var main = this.particle.main;
            if (main.startSpeed.constant > 0)
            {
                main.startSpeed = value;
            }
        }

        public void SetCalcedSize(float value)
        {
            var main = this.particle.main;
            main.startSize = value;
        }

        public string GetSoundId()
        {
            return this.SoundIds[Random.Range(0, this.SoundIds.Length)];
        }
    }
}
