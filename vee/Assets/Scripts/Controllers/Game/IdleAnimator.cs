using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class IdleAnimator : MonoBehaviour
    {
        private void Update()
        {
            var scale = this.transform.localScale;
            scale.y = 1 + 0.02f * Mathf.Sin(Time.time * 10);
            this.transform.localScale = scale;
        }
    }
}
