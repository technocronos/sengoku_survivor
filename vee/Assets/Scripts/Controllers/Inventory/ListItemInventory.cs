using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Inventory
{
    public sealed class ListItemInventory : MonoBehaviour
    {
        public event System.Action<int> Clicked = _ => { };

        [SerializeField]
        private UnityEngine.UI.Image icon;

        [SerializeField]
        private UnityEngine.UI.Text quantityText;

        private int index;

        public void Initialize(int index)
        {
            this.index = index;
        }

        public void SetSprite(Sprite sprite)
        {
            this.icon.sprite = sprite;
        }

        public void SetQuantity(int value)
        {
            this.quantityText.text = $"×{value}";
        }

        public void OnClicked()
        {
            this.Clicked.Invoke(this.index);
        }
    }
}
