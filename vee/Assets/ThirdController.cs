using UnityEngine;
using UNCHAIN.ThirdSdk;
using System.Collections;
using UnityEditor;
using Vs.Controllers.Game;

public class ThirdController : MonoBehaviour
{

    public string StreamId { set { streamId = value; } }
    private string streamId;
    public ThirdConnector ThirdConnector { get; private set; }
    [System.NonSerialized]
    public ThirdCommentsUi CommentsUi;

    private void Awake()
    {
        ThirdConnector = GetComponent<ThirdConnector>();
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
