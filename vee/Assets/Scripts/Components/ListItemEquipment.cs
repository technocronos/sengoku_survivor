using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Components
{
    public sealed class ListItemEquipment : MonoBehaviour
    {
        public event System.Action<int> Clicked = _ => { };

        [SerializeField]
        private UnityEngine.UI.Image icon;

        [SerializeField]
        private UnityEngine.UI.Image iconBg;

        [SerializeField]
        private UnityEngine.UI.Text levelText;

        [SerializeField]
        private UnityEngine.UI.Text rankText;

        private int index;
        private bool isSet;

        public void Initialize(int index)
        {
            this.index = index;
        }

        public void Set()
        {
            this.isSet = true;
            this.icon.gameObject.SetActive(true);
            this.levelText.gameObject.SetActive(true);
            this.rankText.gameObject.SetActive(true);
        }

        public void Unset()
        {
            this.isSet = false;
            this.icon.gameObject.SetActive(false);
            this.levelText.gameObject.SetActive(false);
            this.rankText.gameObject.SetActive(false);
            this.iconBg.color = Color.white;
        }

        public void SetSprite(Sprite sprite)
        {
            this.icon.sprite = sprite;
        }

        public void SetRarity(int value)
        {
            this.iconBg.color = Utils.GetRarityColor(value);
        }

        public void SetLevel(int value)
        {
            this.levelText.text = $"L{value}";
        }

        public void SetRank(int value)
        {
            this.rankText.text = value > 0 ? $"{value}" : "";
        }

        public void OnClicked()
        {
            if (this.isSet)
            {
                this.Clicked.Invoke(this.index);
            }
        }
    }
}
