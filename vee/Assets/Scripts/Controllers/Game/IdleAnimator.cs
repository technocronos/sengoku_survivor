using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class IdleAnimator : MonoBehaviour
    {
        private void Update()
        {
            var scale = this.transform.localPosition;
            scale.y = 0.2f + 0.02f * Mathf.Sin(Time.time * 10);
            this.transform.localPosition = scale;
        }
    }
}
