using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null)
            {
                GameObject.Destroy(this.gameObject);
                return;
            }
            instance = this.GetComponent<T>();
        }
    }
}
