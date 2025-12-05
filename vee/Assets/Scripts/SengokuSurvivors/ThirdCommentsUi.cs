using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Vs;
using System.Collections.Generic;

namespace SengokuSurvivors
{
    public class ThirdCommentsUi : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect scrollView;
        [SerializeField]
        private Transform commentsContainer;
        [SerializeField]
        private TMP_Text commentPrefab;

        private int maxTexts = 10;
        private readonly Queue<TMP_Text> texts = new Queue<TMP_Text>();

        private void Awake()
        {
            var a = FindAnyObjectByType<ThirdController>();
            if (a != null) a.CommentsUi = this;
        }

        public void AddComment(string text)
        {
            TMP_Text a;
            if (texts.Count > maxTexts - 1)
            {
                a = texts.Dequeue();
                a.transform.SetParent(null);
                a.transform.SetParent(commentsContainer);
            }
            else
            {
                a = Instantiate(commentPrefab, commentsContainer);
            }

            a.gameObject.SetActive(true);
            texts.Enqueue(a);
            a.text = text;
            scrollView.verticalNormalizedPosition = 0f;
            SoundService.Instance.PlaySe("se_getprice");
        }
    }
}