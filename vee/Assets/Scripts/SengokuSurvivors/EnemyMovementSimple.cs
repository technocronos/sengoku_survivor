using System.Collections;
using UnityEngine;
namespace SengokuSurvivors
{
    public class EnemyMovementSimple : MonoBehaviour, IEnemyMovement
    {
        private bool isKnockedBack = false;
        public float Spd = 0.5f;

        public void SetKnockbackState(bool isKnockedBack)
        {
            this.isKnockedBack = isKnockedBack;
        }

        public void SetRandomSpd(float spd, float dispersion = 0.5f)
        {
            spd = Random.Range(spd - dispersion, spd + dispersion);
            if (spd < 0) spd = 0;
            this.Spd = spd;
        }

        private void Update()
        {
            if (isKnockedBack) return;
            var player = Vs.Controllers.Game.GameManager.Instance.Player;
            var dir = Vector3.down; //player.transform.position - this.transform.position;
            var pos = this.transform.position;
            pos += this.Spd * Time.deltaTime * dir.normalized;
            this.transform.position = pos;
        }

        public void Initialize()
        {
            SetRandomSpd(Spd);
        }
    }
}