using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class ItemMagnet : Item
    {
        protected override void OnComplete()
        {
            GameManager.Instance.Magnet();
        }
    }
}
