using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs
{
    public sealed class Engine : SingletonMonoBehaviour<Engine>
    {
        private void Start()
        {
            var context = new Controllers.Loading.Loading.Context();
            ViewService.Instance.ChangeView(context);
        }
    }
}
