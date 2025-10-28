using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.Game
{
    public sealed class EffectSpawner : SingletonMonoBehaviour<EffectSpawner>
    {
        [SerializeField]
        private Transform world;

        public void Spawn(Vector3 position)
        {
        }
    }
}
