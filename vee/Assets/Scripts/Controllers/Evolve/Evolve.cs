using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Evolve
{
    public sealed class Evolve : Controller
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
