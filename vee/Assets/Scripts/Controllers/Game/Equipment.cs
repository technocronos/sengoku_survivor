using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public class Equipment
    {
        public int ItemId;
        public int Category;
        public string Name;
        public string Description;
        public Sprite ItemIcon;
        public int Level;
    }

    public sealed class Weapon : Equipment
    {
        public int Atk;
        public int Speed;
        public int CoolTime;
        public float CoolTimeMulti = 1;
        public int LifeTime;
        public int Projectile;
        public int Count;
        public int Size;
        public float SizeMulti = 1;

        public float KnockbackTime = 0f;
        public float KnockbackLength = 0f;
        public readonly Dictionary<int, SkillType> SkillTypes = new Dictionary<int, SkillType>();
    }

    public sealed class Accessory : Equipment
    {
        //public string EffectId;
        //public int EffectValue;
    }

    public sealed class SkillType
    {
        public string Name;
        public int Level;
        public string EffectId;
        public int EffectValue;
    }
}
