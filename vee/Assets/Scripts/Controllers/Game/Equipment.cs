using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public class Equipment
    {
        public int ItemId;
        public ItemCategory Category;
        public string Name;
        public string Description;
        public Sprite ItemIcon;
        public int Level;
    }

    public enum ItemCategory
    {
        None = 0,
        Weapon = 1,
        Accessory = 2,
        item = 3
    }

    public sealed class Weapon : Equipment
    {
        public int Atk;
        public int Speed;
        public float CoolTime;
        public int Count;

        public float KnockbackTime = 0f;
        public float KnockbackLength = 0f;
    }

    public sealed class Accessory : Equipment
    {
        public string EffectId;
        public int EffectValue;
    }

    public sealed class Items : Equipment
    {
        public string Value;
    }

}
