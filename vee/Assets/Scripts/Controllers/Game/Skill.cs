using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class Skill
    {
        public int SkillId;
        public int Category;
        public int Atk;
        public int Speed;
        public int CoolTime;
        public float CoolTimeMulti = 1;
        public int LifeTime;
        public int Projectile;
        public int Count;
        public int Size;
        public float SizeMulti = 1;
        public string EffectId;
        public int EffectValue;
        public readonly Dictionary<int, SkillType> SkillTypes = new Dictionary<int, SkillType>();
    }

    public sealed class SkillType
    {
        public string Name;
        public int Level;
    }
}
