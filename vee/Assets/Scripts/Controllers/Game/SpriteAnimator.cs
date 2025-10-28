using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class SpriteAnimator : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer renderer;

        [SerializeField]
        private Sprite[] sprites;

        public bool autoPlay = true;

        private bool isPlay;
        private int index;
        private float elapsed;

        private void Start()
        {
            this.isPlay = this.autoPlay;
        }

        private void Update()
        {
            if (!this.isPlay)
            {
                return;
            }
            this.elapsed += Time.deltaTime;
            if (this.elapsed <= 0.1f)
            {
                return;
            }
            this.elapsed -= 0.1f;
            this.index = (this.index + 1) % this.sprites.Length;
            this.renderer.sprite = this.sprites[this.index];
        }

        public void Play()
        {
            this.isPlay = true;
        }

        public void Stop()
        {
            this.isPlay = false;
        }
    }
}
