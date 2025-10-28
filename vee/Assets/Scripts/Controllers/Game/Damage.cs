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

        public void Show(int damage)
        {
            this.text.text = damage.ToString();
            GameObject.Destroy(this.gameObject, this.duration);
        }
    }
}
