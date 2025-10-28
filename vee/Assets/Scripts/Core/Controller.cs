using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs
{
    public class Controller : MonoBehaviour
    {
        public class ViewContext
        {
            // nop
        }
        public virtual IEnumerator OnViewLoaded(ViewContext viewContext)
        {
            yield break;
        }

        public virtual void OnViewAdded()
        {
            // nop
        }
    }
}
