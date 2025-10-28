using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Components
{
    public sealed class Header : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Text staminaText;

        [SerializeField]
        private UnityEngine.UI.Text coinsText;

        [SerializeField]
        private UnityEngine.UI.Text gemsText;

        private void Start()
        {
            UserService.Instance.Updated += this.OnRefreshed;
            this.OnRefreshed();
        }

        private void OnDestroy()
        {
            UserService.Instance.Updated -= this.OnRefreshed;
        }

        private void OnRefreshed()
        {
            var user = UserService.Instance;
            this.staminaText.text = $"{user.Stamina}/{user.StaminaMax}";
            this.coinsText.text = $"{user.Coins}";
            this.gemsText.text = $"{user.Gems}";
        }
    }
}
