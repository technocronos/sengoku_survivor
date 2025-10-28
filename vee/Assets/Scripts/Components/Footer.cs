using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Components
{
    public sealed class Footer : MonoBehaviour
    {
        public void OnHomeButtonClicked()
        {
            var context = new Controllers.Home.Home.Context();
            ViewService.Instance.ChangeView(context);
        }

        public void OnShopButtonClicked()
        {
            var context = new Controllers.Shop.Shop.Context();
            ViewService.Instance.ChangeView(context);
        }

        public void OnEquipButtonClicked()
        {
            var context = new Controllers.Equipment.Equipment.Context();
            ViewService.Instance.ChangeView(context);
        }

        public void OnChallengeButtonClicked()
        {
            var context = new Controllers.Challenge.Challenge.Context();
            ViewService.Instance.ChangeView(context);
        }

        public void OnEvolveButtonClicked()
        {
            var context = new Controllers.Evolve.Evolve.Context();
            ViewService.Instance.ChangeView(context);
        }
    }
}
