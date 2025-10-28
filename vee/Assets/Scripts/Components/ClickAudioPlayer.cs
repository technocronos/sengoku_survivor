using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Components
{
    public sealed class ClickAudioPlayer : MonoBehaviour
    {
        [SerializeField]
        private string audioName = null;

        private void Start()
        {
            var button = this.GetComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(this.OnClicked);
        }

        public void OnClicked()
        {
            SoundService.Instance.PlaySe(this.audioName);
        }
    }
}
