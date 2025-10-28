using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public class ListItemSkill : MonoBehaviour
    {
        public event System.Action<int> ListItemClicked = _ => { };

        [SerializeField]
        private UnityEngine.UI.Image bg;

        [SerializeField]
        private UnityEngine.UI.Text nameText;

        [SerializeField]
        private UnityEngine.UI.Text descriptionText;

        [SerializeField]
        private UnityEngine.UI.Image icon;

        [SerializeField]
        private GameObject newLabel;

        [SerializeField]
        private GameObject[] stars;

        private int index;

        public void Initialize(int index)
        {
            this.index = index;
        }

        public void SetName(string name)
        {
            this.nameText.text = name;
        }

        public void SetDescription(string description)
        {
            this.descriptionText.text = description;
        }

        public void SetLevel(int level)
        {
            foreach (var i in this.stars)
            {
                i.gameObject.SetActive(false);
            }
            for (var i = 0; i < level; i++)
            {
                if (i < this.stars.Length)
                {
                    this.stars[i].gameObject.SetActive(true);
                }
            }
            
            var color = level == 6 ? Color.red : Color.black;
            color.a = 0.5f;
            this.bg.color = color;
        }

        public void SetSprite(Sprite sprite)
        {
            this.icon.sprite = sprite;
        }

        public void OnClicked()
        {
            this.ListItemClicked.Invoke(this.index);
        }
    }
}
