using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class Game : Controller
    {
        public sealed class Context : ViewContext
        {
            // nop
        }

        public override IEnumerator OnViewLoaded(ViewContext viewContext)
        {
            SoundService.Instance.PlayBgm("bgm1");
            GameManager.Instance.Initialize();
            yield break;
        }

        public override void OnViewAdded()
        {
            GameManager.Instance.Play();
        }
    }
}
