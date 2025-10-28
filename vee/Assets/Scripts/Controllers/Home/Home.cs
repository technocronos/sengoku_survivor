using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Home
{
    public sealed class Home : Controller
    {
        public sealed class Context : ViewContext
        {
            // nop
        }

        public override IEnumerator OnViewLoaded(ViewContext viewContext)
        {
            SoundService.Instance.PlayBgm("menu");
            var response = Api.Stats.Get();
            UserService.Instance.Set(response);
            yield break;
        }

        public void OnPlayButtonClicked()
        {
            var context = new Controllers.Game.Game.Context();
            ViewService.Instance.ChangeView(context);
        }

        public void OnInventoryButtonClicked()
        {
            var context = new Controllers.Inventory.Inventory.Context();
            ViewService.Instance.ChangeView(context);
        }
    }
}
