using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs
{
    public static class Utils
    {
        public static Color GetRarityColor(int rarity)
        {
            return new Color[] {
                Color.white,
                Color.green,
                Color.blue,
                Color.magenta
            }[rarity];
        }
    }
}
