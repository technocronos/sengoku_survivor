using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Vs.Controllers.MyPage
{
    public sealed class MyPage : Controller
    {
        [SerializeField]
        private Button ButtonPlay;
        [SerializeField]
        private Button ButtonSettings;
        [SerializeField]
        private Button BtnThird;
        [SerializeField]
        private Button BtnHelp;
        [SerializeField]
        private Button BtnTitle;

        [SerializeField]
        private Button ButtonQuit;
        [SerializeField]
        private GameObject MenuSettings;
        [SerializeField]
        private GameObject MenuThird;
        [SerializeField]
        private GameObject MenuHelp;
        [SerializeField]
        private Button ButtonCloseMenuSettings;
        [SerializeField]
        private Button ButtonCloseHelp;
        [SerializeField]
        private TMP_InputField InputFieldStreamId;

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
            SoundService.Instance.PlayBgm("menu");
            var response = Api.Stats.Get();
            UserService.Instance.Set(response);
        }

        private void Awake()
        {
            ButtonPlay.onClick.AddListener(OnButtonPlay);
            ButtonSettings.onClick.AddListener(OnButtonSettings);
            BtnHelp.onClick.AddListener(OnButtonHelp);
            BtnTitle.onClick.AddListener(OnButtonTitle); 
            BtnThird.onClick.AddListener(OnButtonThird);

            ButtonQuit.onClick.AddListener(OnButtonQuit);
            ButtonCloseMenuSettings.onClick.AddListener(OnButtonCloseSettings);
            ButtonCloseHelp.onClick.AddListener(OnButtonCloseHelp);

            MenuSettings.gameObject.SetActive(false);
            InputFieldStreamId.onValueChanged.AddListener(OnInputFieldChanged);
            thirdController = FindAnyObjectByType<ThirdController>();

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
            var context = new Game.Game.Context();
            ViewService.Instance.ChangeView(context);
            SoundService.Instance.PlaySe("get_item");
        }
        
        private void OnButtonThird()
        {
            SoundService.Instance.PlaySe("get_item");
            MenuThird.SetActive(true);
            InputFieldStreamId.Select();
            InputFieldStreamId.caretPosition = InputFieldStreamId.text.Length;
        }

        private void OnButtonSettings()
        {
            SoundService.Instance.PlaySe("get_item");
            MenuSettings.SetActive(true);
        }

        private void OnButtonHelp()
        {
            SoundService.Instance.PlaySe("get_item");
            MenuHelp.SetActive(true);
        }
        private void OnButtonTitle()
        {
            SoundService.Instance.PlaySe("get_item");
            var context = new Home.Home.Context();
            ViewService.Instance.ChangeView(context);
        }

        public void OnButtonCloseSettings()
        {
            SoundService.Instance.PlaySe("get_item");
            MenuSettings.SetActive(false);
        }
        public void OnButtonCloseThird()
        {
            SoundService.Instance.PlaySe("get_item");
            MenuThird.SetActive(false);
        }

        public void OnButtonCloseHelp()
        {
            SoundService.Instance.PlaySe("get_item");
            MenuHelp.SetActive(false);
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
