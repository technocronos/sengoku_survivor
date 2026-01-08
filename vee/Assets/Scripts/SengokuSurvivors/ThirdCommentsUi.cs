using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Vs;
using System.Collections;
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
        private GameObject commentPrefab;

        private int maxTexts = 9;
        private readonly Queue<GameObject> texts = new Queue<GameObject>();

        private void Awake()
        {
            var a = FindAnyObjectByType<ThirdController>();
            if (a != null) a.CommentsUi = this;

            commentPrefab.gameObject.SetActive(false);
        }

        public void AddComment(string text, bool isAction = false)
        {
            GameObject obj;

            if (GetScrollViewHeight() < GetCommentsContainerHeight())
            {
                obj = texts.Dequeue();
                obj.transform.SetParent(null);
                obj.transform.SetParent(commentsContainer);
            }
            else
            {
                obj = Instantiate(commentPrefab, commentsContainer);
            }

            obj.gameObject.SetActive(true);
            texts.Enqueue(obj);

            var txt = obj.transform.Find("Text").GetComponent<TextMeshProUGUI>();

            txt.text = text;
            
            // Imageコンポーネントの色を設定
            var image = obj.GetComponent<Image>();
            if (image != null)
            {
                Color color;
                if (isAction)
                {
                    // isAction=trueの場合、#FF3838（赤色）に設定
                    ColorUtility.TryParseHtmlString("#FF3838", out color);
                }
                else
                {
                    // isAction=falseの場合、#005DFF（青色）に設定
                    ColorUtility.TryParseHtmlString("#005DFF", out color);
                }
                // 既存のアルファ値を保持
                color.a = image.color.a;
                image.color = color;
            }
            
            // レイアウト更新後にスクロール位置を設定（見切れを防ぐため）
            StartCoroutine(ScrollToBottomAfterLayout());
            
            SoundService.Instance.PlaySe("mousou_comment");
        }

        /// <summary>
        /// レイアウト更新後にスクロールを最下部に移動
        /// </summary>
        private IEnumerator ScrollToBottomAfterLayout()
        {
            // レイアウト更新を強制
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(commentsContainer.GetComponent<RectTransform>());
            
            // 1フレーム待つ（レイアウト更新を確実にする）
            yield return null;
            
            // スクロール位置を最下部に設定
            if (scrollView != null)
            {
                scrollView.verticalNormalizedPosition = 0f;
            }
        }

        /// <summary>
        /// scrollViewの高さを取得
        /// </summary>
        public float GetScrollViewHeight()
        {
            if (scrollView == null) return 0f;
            var rectTransform = scrollView.GetComponent<RectTransform>();
            if (rectTransform == null) return 0f;
            return rectTransform.rect.height;
        }

        /// <summary>
        /// commentsContainerの高さを取得
        /// </summary>
        public float GetCommentsContainerHeight()
        {
            if (commentsContainer == null) return 0f;
            var rectTransform = commentsContainer.GetComponent<RectTransform>();
            if (rectTransform == null) return 0f;
            return rectTransform.rect.height;
        }
    }
}