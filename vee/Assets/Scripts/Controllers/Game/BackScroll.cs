using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class BackScroll : MonoBehaviour
    {
        [SerializeField]
        private float scale = 20.0f;

        [SerializeField]
        private Renderer renderer;

        private void Update()
        {
            this.renderer.sharedMaterial.SetTextureOffset("_MainTex", this.transform.position / this.scale);
        }
    }
}
