using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance { get { 
                if (instance == null)
                {
                    instance = FindAnyObjectByType<T>();
                }
                return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                GameObject.Destroy(this.gameObject);
                return;
            }
            OnAwake();
        }

        protected virtual void OnAwake()
        {

        }
    }
}
