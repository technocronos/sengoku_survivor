using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class ItemGem : Item
    {
        [SerializeField]
        private int value = 2;

        protected override void OnComplete()
        {
            // GameManager.Instance.AddExp(this.value);
        }
    }
}
