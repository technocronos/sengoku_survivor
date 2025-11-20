using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;
using UnityEngine.Scripting;

namespace Vs.Controllers.Game
{
    public sealed class PopupPause : MonoBehaviour
    {
        public void Show(List<Skill> current)
        {
            this.gameObject.SetActive(true);
            Time.timeScale = 0.0f;
            SoundService.Instance.PauseBgm();
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
            Time.timeScale = 1.0f;
            SoundService.Instance.UnpauseBgm();
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
