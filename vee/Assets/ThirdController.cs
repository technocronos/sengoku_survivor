using UnityEngine;
using UNCHAIN.ThirdSdk;
using System.Collections;
using Vs.Controllers.Game;

[RequireComponent(typeof(UNCHAIN.ThirdSdk.ThirdConnector))]
public class ThirdController : MonoBehaviour
{

    public string StreamId { set { streamId = value; } }
    private string streamId;
    public ThirdConnector ThirdConnector { get; private set; }
    [System.NonSerialized]
    public ThirdCommentsUi CommentsUi;

#if DEBUG
    private string url = @"https://dev.live-ctl.com";
    private string wsurl = @"wss://dev.live-ctl.com";
    private string appId = "019aa5a7-eaf9-74f2-be28-66f51ecbc243";
    private string apiKey = "4c1c654d340ac6c6c67b97cf22d7ebb145ee7174516310d317776f2fad0ab3c1";
#else
    private string url = @"https://live-ctl.com";
    private string wsurl = @"wss://live-ctl.com";
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
        string actionName = "";
        switch (data.commandKey)
        {
            case "dummy1":
                GameManager.Instance.Player.RecoverHp(10);
                actionName = "小回復";
                break;
            case "dummy2":
                GameManager.Instance.Player.RecoverHp(20);
                actionName = "中回復";
                break;
            case "dummy3":
                GameManager.Instance.Player.RecoverHp(30);
                actionName = "大回復";
                break;
            default:
                break;
        }
        CommentsUi.AddComment(string.Format("[{0}] {1} ({2})", GameManager.Instance.GetTimeText(), actionName, data.displayName));
    }

    public void OnErrorMessageReceived(string message)
    {
        Debug.Log(message);
    }
}
