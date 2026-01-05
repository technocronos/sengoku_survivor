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
        private Button ButtonThird;
        [SerializeField]
        private Button ButtonSettings;
        [SerializeField]
        private Button ButtonQuit;
        [SerializeField]
        private Button ButtonQuit2;
        [SerializeField]
        private GameObject MenuThird;
        [SerializeField]
        private GameObject MenuSettings;
        [SerializeField]
        private Button ButtonCloseMenuSettings;
        [SerializeField]
        private Button ButtonCloseThird;
        [SerializeField]
        private TMP_InputField InputFieldStreamId;
        [SerializeField]
        private Button BtnHelp;
        [SerializeField]
        private Button BtnTitle;
        [SerializeField]
        private GameObject MenuHelp;
        [SerializeField]
        private Button ButtonCloseHelp;

        [SerializeField]
        private GameObject StreamIdInputContainer;

        private ThirdController thirdController;

        public sealed class Context : ViewContext
        {
            // nop
        }

        public override IEnumerator OnViewLoaded(ViewContext viewContext)
        {
            yield return null;
            SoundService.Instance.PlayBgm("menu2");
            var response = Api.Stats.Get();
            UserService.Instance.Set(response);
        }

        private void Awake()
        {
            ButtonPlay.onClick.AddListener(OnButtonPlay);
            ButtonSettings.onClick.AddListener(OnButtonSettings);
            ButtonThird.onClick.AddListener(OnButtonThird);
            ButtonQuit.onClick.AddListener(OnButtonQuit);
            ButtonQuit2.onClick.AddListener(OnButtonQuit);
            BtnHelp.onClick.AddListener(OnButtonHelp);
            ButtonCloseMenuSettings.onClick.AddListener(OnButtonCloseSettings);
            ButtonCloseThird.onClick.AddListener(OnButtonCloseThird);
            MenuSettings.gameObject.SetActive(false);
            InputFieldStreamId.onValueChanged.AddListener(OnInputFieldChanged);
            thirdController = FindAnyObjectByType<ThirdController>();
            ButtonCloseHelp.onClick.AddListener(OnButtonCloseHelp);

            MenuSettings.SetActive(false);
            MenuHelp.SetActive(false);
            MenuThird.SetActive(false);
#if DEVELOP
            Debug.Log("DEVELOP build");
#else
            //StreamIdInputContainer.SetActive(false);
            Debug.Log("RELEASE build");
#endif
        }

        private void OnButtonPlay()
        {
            var context = new MyPage.MyPage.Context();
            ViewService.Instance.ChangeView(context);
            SoundService.Instance.PlaySe("get_item");
        }

        private void OnButtonSettings()
        {
            SoundService.Instance.PlaySe("get_item");
            MenuSettings.SetActive(true);
        }
        private void OnButtonThird()
        {
            SoundService.Instance.PlaySe("get_item");
            MenuThird.SetActive(true);
            InputFieldStreamId.Select();
            InputFieldStreamId.caretPosition = InputFieldStreamId.text.Length;
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

        private void OnButtonCloseThird()
        {
            SoundService.Instance.PlaySe("get_item");
            MenuThird.SetActive(false);

            var context = new MyPage.MyPage.Context();
            ViewService.Instance.ChangeView(context);
        }

        private void OnInputFieldChanged(string value)
        {
            thirdController.StreamId = value;
        }

        private void OnButtonHelp()
        {
            SoundService.Instance.PlaySe("get_item");
            MenuHelp.SetActive(true);
        }
        public void OnButtonCloseHelp()
        {
            SoundService.Instance.PlaySe("get_item");
            MenuHelp.SetActive(false);
        }
    }
}
