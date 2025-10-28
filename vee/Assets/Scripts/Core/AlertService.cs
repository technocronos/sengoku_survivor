using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs
{
    public sealed class AlertService : SingletonMonoBehaviour<AlertService>
    {
        [SerializeField]
        private GameObject window;

        [SerializeField]
        private UnityEngine.UI.Text message;

        [SerializeField]
        private UnityEngine.UI.Button okButton;

        [SerializeField]
        private UnityEngine.UI.Button yesButton;

        [SerializeField]
        private UnityEngine.UI.Button noButton;

        private System.Action onOkOnce;
        private System.Action onYesOnce;
        private System.Action onNoOnce;

        public void Show(string message, System.Action onOk = null, System.Action onYes = null, System.Action onNo = null)
        {
            this.window.gameObject.SetActive(true);
            SoundService.Instance.PlaySe("open");
            this.message.text = message;

            this.onOkOnce = onOk;
            this.onYesOnce = onYes;
            this.onNoOnce = onNo;

            var ynMode = this.onYesOnce != null;
            this.okButton.gameObject.SetActive(!ynMode);
            this.yesButton.gameObject.SetActive(ynMode);
            this.noButton.gameObject.SetActive(ynMode);
        }

        public void OnOkButtonClicked()
        {
            this.window.gameObject.SetActive(false);
            if (this.onOkOnce != null)
            {
                var temp = this.onOkOnce;
                this.onOkOnce = null;
                temp.Invoke();
            }
        }

        public void OnYesButtonClicked()
        {
            this.window.gameObject.SetActive(false);
            if (this.onYesOnce != null)
            {
                var temp = this.onYesOnce;
                this.onYesOnce = null;
                temp.Invoke();
            }
        }

        public void OnNoButtonClicked()
        {
            this.window.gameObject.SetActive(false);
            if (this.onNoOnce != null)
            {
                var temp = this.onNoOnce;
                this.onNoOnce = null;
                temp.Invoke();
            }
        }
    }
}
