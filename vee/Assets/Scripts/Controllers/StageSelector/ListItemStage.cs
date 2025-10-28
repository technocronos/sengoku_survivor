using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.StageSelector
{
    public sealed class ListItemStage : MonoBehaviour
    {
        public event System.Action<int> Clicked = _ => { };

        [SerializeField]
        private UnityEngine.UI.Image icon;

        private int index;

        public void Initialize(int index)
        {
            this.index = index;
        }

        public void SetSprite(Sprite sprite)
        {
            this.icon.sprite = sprite;
        }

        public void OnClicked()
        {
            this.Clicked.Invoke(this.index);
        }
    }
}
