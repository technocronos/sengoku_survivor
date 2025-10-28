using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.Loading
{
    public sealed class Loading : Controller
    {
        public sealed class Context : ViewContext
        {
            // nop
        }

        [SerializeField]
        private UnityEngine.UI.Text loadingText;

        public override IEnumerator OnViewLoaded(ViewContext viewContext)
        {
            yield break;
        }

        public override void OnViewAdded()
        {
            this.StartCoroutine(this.Play());
        }

        private IEnumerator Play()
        {
            yield return DbService.Instance.Initialize(progress =>
            {
                this.loadingText.text = $"{progress * 100:0.00}%";
            });

            Api.Users.Login();

            var context = new Controllers.Game.Game.Context();
            //var context = new Controllers.Home.Home.Context();
            ViewService.Instance.ChangeView(context);
        }
    }
}
