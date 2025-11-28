using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SengokuSurvivors
{
    public class BuffIcon : MonoBehaviour
    {
        [SerializeField]
        private Image buffIcon;
        [SerializeField]
        private TMP_Text buffCounter;

        private void Awake()
        {
            buffIcon.gameObject.SetActive(false);
            buffCounter.gameObject.SetActive(false);
        }

        public void UpdateIcon(Sprite icon, int count)
        {
            buffIcon.gameObject.SetActive(count > 0);
            buffIcon.sprite = icon;
            buffCounter.gameObject.SetActive(count > 1);
            buffCounter.text = count.ToString();
        }
    }
}