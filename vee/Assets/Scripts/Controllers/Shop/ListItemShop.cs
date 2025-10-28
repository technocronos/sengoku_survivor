using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Shop
{
    public sealed class ListItemShop : MonoBehaviour
    {
        public event System.Action<int> Clicked = _ => { };

        [SerializeField]
        private UnityEngine.UI.Image icon;

        [SerializeField]
        private UnityEngine.UI.Text nameText;

        [SerializeField]
        private UnityEngine.UI.Text priceText;

        private int index;

        public void Initialize(int index)
        {
            this.index = index;
        }

        public void SetSprite(Sprite sprite)
        {
            // this.icon.sprite = sprite;
        }

        public void SetName(string name)
        {
            this.nameText.text = name;
        }

        public void SetPrice(int price)
        {
            this.priceText.text = $"{price}";
        }

        public void SetCurrency(string currency)
        {
        }

        public void SetQuantity(int quantity)
        {
        }

        public void OnClicked()
        {
            this.Clicked.Invoke(this.index);
        }
    }
}
