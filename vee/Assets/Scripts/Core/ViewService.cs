using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyGame;

namespace Vs
{
    public sealed class ViewService : SingletonMonoBehaviour<ViewService>
    {
        [SerializeField]
        private float duration = 0.2f;

        [SerializeField]
        private UnityEngine.UI.Image image;

        public string CurrentViewName { get; private set; }

        public void ChangeView(Controller.ViewContext context)
        {
            this.StartCoroutine(this.InnerChangeView(context));
        }

        private IEnumerator InnerChangeView(Controller.ViewContext context)
        {
            yield return this.FadeOut();
            if (!string.IsNullOrEmpty(this.CurrentViewName))
            {
                var prev = this.GetController(this.CurrentViewName);
                yield return this.InnerRemoveView(prev);
            }
            var match = Regex.Match(context.GetType().FullName, @".*.Controllers\.(?<name>.*)\.");
            this.CurrentViewName = match.Groups["name"].Value;
            yield return SceneManager.LoadSceneAsync(this.CurrentViewName, LoadSceneMode.Additive);
            var current = this.GetController(this.CurrentViewName);
            yield return current.OnViewLoaded(context);
            yield return this.FadeIn();
            current.OnViewAdded();
        }

        public void RemoveView(Controller controller)
        {
            this.StartCoroutine(this.InnerRemoveView(controller));
        }

        private IEnumerator InnerRemoveView(Controller controller)
        {
            yield return SceneManager.UnloadSceneAsync(controller.name);
        }

        private Controller GetController(string viewName)
        {
            return GameObject.Find(viewName).GetComponent<Controller>();
        }

        private IEnumerator FadeOut()
        {
            this.image.gameObject.SetActive(true);
            var color = this.image.color;
            color.a = 0.0f;
            while (color.a < 1.0f)
            {
                yield return null;
                this.image.color = color;
                color.a += Time.deltaTime / this.duration;
            }
            color.a = 1.0f;
            this.image.color = color;
        }

        private IEnumerator FadeIn()
        {
            this.image.gameObject.SetActive(true);
            var color = this.image.color;
            color.a = 1.0f;
            while (color.a > 0.0f)
            {
                yield return null;
                this.image.color = color;
                color.a -= Time.deltaTime / this.duration;
            }
            color.a = 0.0f;
            this.image.color = color;
            this.image.gameObject.SetActive(false);
        }
    }
}
