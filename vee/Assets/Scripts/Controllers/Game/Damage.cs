using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class Damage : MonoBehaviour
    {
        [SerializeField]
        private float duration = 1.0f;

        [SerializeField]
        private UnityEngine.UI.Text text;

        private DamageSpawner spawner;

        public void Show(DamageSpawner spawner, int damage)
        {
            gameObject.SetActive(true);
            this.spawner = spawner;
            this.text.text = damage.ToString();
            StartCoroutine(ShowRoutine());
        }

        private IEnumerator ShowRoutine()
        {
            yield return new WaitForSeconds(duration);
            spawner.Despawn(this);
        }
    }
}
