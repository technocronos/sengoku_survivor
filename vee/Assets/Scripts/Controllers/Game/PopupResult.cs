using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;
using TMPro;

namespace Vs.Controllers.Game
{
    public sealed class PopupResult : MonoBehaviour
    {
        private System.Action callbackOnce;

        [SerializeField]
        private GameObject WinTitle;
        [SerializeField]
        private GameObject LoseTitle;
        [SerializeField]
        private TextMeshProUGUI score;

        public class GameResult
        {
            public static readonly int Win = 1;
            public static readonly int Lose = 2;

            public int Result { get; set; }

            public GameResult(int result)
            {
                this.Result = result;
            }
        }

        public void Show(GameResult result, System.Action callbackOnce = null)
        {
            this.callbackOnce = callbackOnce;
            this.gameObject.SetActive(true);
            Time.timeScale = 0.0f;
            WinTitle.SetActive(false);
            LoseTitle.SetActive(false);

            SoundService.Instance.StopBgm();
            score.text = GameManager.Instance.totalScore.ToString();

            if (result.Result == GameResult.Win)
            {
                WinTitle.SetActive(true);
                SoundService.Instance.PlaySe("se_congrats");
            }
            else
            {
                LoseTitle.SetActive(true);
                SoundService.Instance.PlaySe("se_retire");
            }
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
            Time.timeScale = 1.0f;

            SoundService.Instance.PlayBgm("bgm1");
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

        public void OnButtonToTitle()
        {
            var context = new Controllers.MyPage.MyPage.Context();
            ViewService.Instance.ChangeView(context);
        }
    }
}
