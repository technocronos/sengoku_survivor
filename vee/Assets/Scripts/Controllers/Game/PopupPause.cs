using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.Game
{
    public sealed class PopupPause : MonoBehaviour
    {
        public void Show(List<Skill> current)
        {
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
        }

        public void OnResetButtonClicked()
        {
            this.Hide();
            var context = new Controllers.Game.Game.Context();
            ViewService.Instance.ChangeView(context);
        }
    }
}
