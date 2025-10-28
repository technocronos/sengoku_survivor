using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.Game
{
    public sealed class PopupGameOver : MonoBehaviour
    {
        private System.Action callbackOnce;

        public void Show(System.Action callbackOnce = null)
        {
            this.callbackOnce = callbackOnce;
            this.gameObject.SetActive(true);
            Time.timeScale = 0.0f;
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
            Time.timeScale = 1.0f;
        }

        public void OnClicked()
        {
            this.Hide();
            if (this.callbackOnce != null)
            {
                this.callbackOnce.Invoke();
                this.callbackOnce = null;
            }
        }
    }
}
