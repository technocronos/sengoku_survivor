using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public class SkillIcon : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Image image;

        [SerializeField]
        private UnityEngine.UI.Text lvText;

        public void Awake()
        {
            this.image.gameObject.SetActive(false);
            this.lvText.gameObject.SetActive(false);
        }

        public void SetSprite(Sprite sprite)
        {
            this.image.gameObject.SetActive(true);
            this.image.sprite = sprite;
        }

        public void SetLevel(int level)
        {
            this.lvText.gameObject.SetActive(true);
            this.lvText.text = $"L{level}";
        }
    }
}
