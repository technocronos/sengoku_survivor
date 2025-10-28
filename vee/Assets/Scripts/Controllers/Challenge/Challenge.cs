using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Challenge
{
    public sealed class Challenge : Controller
    {
        public sealed class Context : ViewContext
        {
            // nop
        }

        public override IEnumerator OnViewLoaded(ViewContext viewContext)
        {
            yield break;
        }
    }
}
