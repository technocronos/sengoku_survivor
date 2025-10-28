using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class ItemCoin : Item
    {
        [SerializeField]
        private int value = 10;

        protected override void OnComplete()
        {
            GameManager.Instance.AddCoins(this.value);
        }
    }
}
