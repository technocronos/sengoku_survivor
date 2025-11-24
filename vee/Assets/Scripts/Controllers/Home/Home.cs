using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Vs.Controllers.Home
{
    public sealed class Home : Controller
    {
        [SerializeField]
        private Button ButtonPlay;
        [SerializeField]
        private Button ButtonSettings;
        [SerializeField]
        private Button ButtonQuit;
        [SerializeField]
        private GameObject MenuSettings;
        [SerializeField]
        private Button ButtonCloseMenuSettings;
        [SerializeField]
        private TMP_InputField InputFieldStreamId;

        private ThirdController thirdController;

        public sealed class Context : ViewContext
        {
            // nop
        }

        public override IEnumerator OnViewLoaded(ViewContext viewContext)
        {
            yield return null;
            SoundService.Instance.PlayBgm("menu");
            var response = Api.Stats.Get();
            UserService.Instance.Set(response);
        }

        private void Awake()
        {
            ButtonPlay.onClick.AddListener(OnButtonPlay);
            ButtonSettings.onClick.AddListener(OnButtonSettings);
            ButtonQuit.onClick.AddListener(OnButtonQuit);
            ButtonCloseMenuSettings.onClick.AddListener(OnButtonCloseSettings);
            MenuSettings.gameObject.SetActive(false);
            InputFieldStreamId.onValueChanged.AddListener(OnInputFieldChanged);
            thirdController = FindAnyObjectByType<ThirdController>();
        }

        private void OnButtonPlay()
        {
            var context = new Game.Game.Context();
            ViewService.Instance.ChangeView(context);
            SoundService.Instance.PlaySe("get_item");
        }

        private void OnButtonSettings()
        {
            SoundService.Instance.PlaySe("get_item");
            MenuSettings.SetActive(true);
        }

        private void OnButtonCloseSettings()
        {
            SoundService.Instance.PlaySe("get_item");
            MenuSettings.SetActive(false);
        }

        private void OnButtonQuit() 
        {
            SoundService.Instance.PlaySe("get_item");
            Application.Quit();
        }

        private void OnInputFieldChanged(string value)
        {
            thirdController.StreamId = value;
        }

    }
}
