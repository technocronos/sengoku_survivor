using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class ItemFood : Item
    {
        [SerializeField]
        private int value = 10;

        protected override void OnComplete()
        {
            GameManager.Instance.Recover(this.value);
        }
    }
}
