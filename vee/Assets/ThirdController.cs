using UnityEngine;
using UNCHAIN.ThirdSdk;
using System.Collections;
using Vs.Controllers.Game;
using SengokuSurvivors;

[RequireComponent(typeof(UNCHAIN.ThirdSdk.ThirdConnector))]
public class ThirdController : MonoBehaviour
{

    public string StreamId { set { streamId = value; } }
    private string streamId;
    public ThirdConnector ThirdConnector { get; private set; }
    [System.NonSerialized]
    public ThirdCommentsUi CommentsUi;
    [System.NonSerialized]
    public ThirdBuffsController BuffsController;


#if DEBUG
    private string url = @"https://dev.live-ctl.com";
    private string wsurl = @"wss://dev.live-ctl.com";
    private string appId = "019aa5a7-eaf9-74f2-be28-66f51ecbc243";
    private string apiKey = "4c1c654d340ac6c6c67b97cf22d7ebb145ee7174516310d317776f2fad0ab3c1";
#else
    private string url = @"https://dev.live-ctl.com";//@"https://live-ctl.com";
    private string wsurl = @"wss://dev.live-ctl.com";//@"wss://live-ctl.com";
    private string appId = "019aa5a7-eaf9-74f2-be28-66f51ecbc243";
    private string apiKey = "4c1c654d340ac6c6c67b97cf22d7ebb145ee7174516310d317776f2fad0ab3c1";
#endif

    private void Awake()
    {
        ThirdConnector = GetComponent<ThirdConnector>();
        ThirdConnector.url = url;
        ThirdConnector.wsurl = wsurl;
        ThirdConnector.appId = appId;
        ThirdConnector.apiKey = apiKey;
    }

    public void Connect()
    {
        if (string.IsNullOrWhiteSpace(streamId)) return;
        this.StartCoroutine(this.ConnectCoroutine(streamId));
    }


    private IEnumerator ConnectCoroutine(string streamId)
    {
        yield return this.ThirdConnector.Connect(streamId);
    }

    public void Disconnect()
    {
        this.ThirdConnector.Disconnect();
    }

    public void OnConnected()
    {
        Debug.Log("connected.");
    }

    public void OnDisconnected()
    {
        Debug.Log("disconnected.");
    }

    public void OnMessageReceived(ThirdResponse data)
    {
        Debug.Log($"{data.txId}, {data.streamId}, {data.actionId}, {data.quantity}, {data.commandKey}, {data.displayName}");
        string text = "";
        bool isAction = false;
        switch (data.commandKey)
        {
            case "dummy1":
                GameManager.Instance.Player.RecoverHp(10);
                text = "HP回復"; //"小回復";
                isAction = true;
                break;
            case "dummy2":
                //GameManager.Instance.Player.RecoverHp(20);
                //actionName = "中回復";
                text = "スピードアップ(10s)";
                BuffsController.AddSpeedupBuff();
                isAction = true;
                break;
            case "dummy3":
                //GameManager.Instance.Player.RecoverHp(30);
                //actionName = "大回復";
                text = "ノックバック強化(10s)";
                BuffsController.AddKnockbackBuff();
                isAction = true;
                break;
            default:
                break;
            case "CHAT_TEXT_QUESTION_DOG":
                text = string.Format("{0}", "好きな犬種を教えて！");
                break;
            case "CHAT_TEXT_888":
                text = string.Format("{0}", "88888888");
                break;
            case "CHAT_TEXT_FIRST_VISIT":
                text = string.Format("{0}", "初見です");
                break;
            case "CHAT_TEXT_LIKE":
                text = string.Format("{0}", "いいね！");
                break;
            case "DUMMY_ITEM1":
                text = string.Format("{0}: {1}", data.displayName, "好きな犬種を教えて！");
                break;
            case "DUMMY_ITEM2":
                text = string.Format("{0}: {1}", data.displayName, "88888888");
                break;
            case "DUMMY_ITEM3":
                text = string.Format("{0}: {1}", data.displayName, "初見です");
                break;
            case "DUMMY_ITEM4":
                text = string.Format("{0}: {1}", data.displayName, "いいね！");
                break;
        }
        if (isAction) CommentsUi.AddComment(string.Format("[{0}] {1} ({2})", GameManager.Instance.GetTimeText(), text, data.displayName));
        else CommentsUi.AddComment(text);
    }

    public void OnErrorMessageReceived(string message)
    {
        Debug.Log(message);
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        OnMessageReceived(new ThirdResponse() { commandKey = "dummy2", displayName = "test user" });
    //        OnMessageReceived(new ThirdResponse() { commandKey = "dummy3", displayName = "test user" });
    //    }
    //}
}
