using SengokuSurvivors;
using System.Collections;
using UNCHAIN.ThirdSdk;
using UnityEngine;
using UnityEngine.UIElements;
using Vs.Controllers.Game;

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


#if DEVELOP
    private string url = @"https://dev.live-ctl.com";
    private string wsurl = @"wss://dev.live-ctl.com";
    private string appId = "019aa5a7-eaf9-74f2-be28-66f51ecbc243";
    private string apiKey = "4c1c654d340ac6c6c67b97cf22d7ebb145ee7174516310d317776f2fad0ab3c1";
#else
    private string url = @"https://live-ctl.com";
    private string wsurl = @"wss://live-ctl.com";
    private string appId = "019b2a6a-9539-70f8-9669-6120acc67d09";
    private string apiKey = "099d1d88c14221b7b2e39f12cfb501dd01131517768b84f7eb6ec7695e74942e";
#endif

    private void Awake()
    {
        ThirdConnector = GetComponent<ThirdConnector>();
        ThirdConnector.url = url;
        ThirdConnector.wsurl = wsurl;
        ThirdConnector.appId = appId;
        ThirdConnector.apiKey = apiKey;
        
        // 接続状態の変化を監視
        ThirdConnector.Connected.AddListener(OnConnected);
        ThirdConnector.Disconnected.AddListener(OnDisconnected);
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

    /// <summary>
    /// Thirdに接続されているかどうかを確認
    /// </summary>
    public bool IsConnected()
    {
        return this.ThirdConnector != null && this.ThirdConnector.IsConnected();
    }

    public void OnConnected()
    {
        Debug.Log("connected.");
        // OnScreenUiに接続状態の変化を通知
        if (OnScreenUi.Instance != null)
        {
            OnScreenUi.Instance.UpdateThirdConnectionStatus();
        }
    }

    public void OnDisconnected()
    {
        Debug.Log("disconnected.");
        // OnScreenUiに接続状態の変化を通知
        if (OnScreenUi.Instance != null)
        {
            OnScreenUi.Instance.UpdateThirdConnectionStatus();
        }
    }

    public void OnMessageReceived(ThirdResponse data)
    {
        Debug.Log($"{data.txId}, {data.streamId}, {data.actionId}, {data.quantity}, {data.commandKey}, {data.displayName}");
        bool isAction = true;
        bool isAuthorComment = false;

        string symbol = $"{data.commandKey}";
        string text =GameManager.Instance.GetTextFromMst(symbol);

        int item_id;
        int enemy_id;
        Vector3 position = DropManager.Instance.getRandumPosition();
        Vector3 enemy_position = EnemySpawner.Instance.getRandumPosition();


        switch (data.commandKey)
        {
            case "HEAL_SMALL":
                item_id = 10100101;

                DropManager.Instance.DropItem(position, item_id);
                isAuthorComment = true;
                break;
            case "HEAL_MEDIUM":
                item_id = 10100102;

                DropManager.Instance.DropItem(position, item_id);
                isAuthorComment = true;
                break;
            case "HEAL_LARGE":
                item_id = 10100103;

                DropManager.Instance.DropItem(position, item_id);
                isAuthorComment = true;
                break;
            case "SPEED_BOOST_ITEM":
                BuffsController.AddSpeedupBuff();
                isAuthorComment = true;
                break;
            case "KNOCKBACK_BOOST_ITEM":
                BuffsController.AddKnockbackBuff();
                isAuthorComment = true;
                break;
            case "CHAT_TEXT_QUESTION_DOG":
                break;
            case "CHAT_TEXT_888":
                break;
            case "CHAT_TEXT_FIRST_VISIT":
                break;
            case "CHAT_TEXT_LIKE":
                break;
            case "NAMECHAT_TEXT_QUESTION_DOG":
                isAuthorComment = true;
                break;
            case "NAMECHAT_TEXT_888":
                isAuthorComment = true;
                break;
            case "NAMECHAT_TEXT_FIRST_VISIT":
                isAuthorComment = true;
                break;
            case "NAMECHAT_TEXT_LIKE":
                isAuthorComment = true;
                break;
            case "ENEMY_SEND_1":
                enemy_id = 10100013;
                EnemySpawner.Instance.LimitSpawn(enemy_id, (int)enemy_position.y, (int)enemy_position.x, data.displayName);
                isAuthorComment = true;
                break;
            case "ENEMY_SEND_2":
                enemy_id = 10100011;
                EnemySpawner.Instance.LimitSpawn(enemy_id, (int)enemy_position.y, (int)enemy_position.x, data.displayName);
                isAuthorComment = true;
                break;
            case "ENEMY_SEND_3":
                enemy_id = 10100014;
                EnemySpawner.Instance.LimitSpawn(enemy_id, (int)enemy_position.y, (int)enemy_position.x, data.displayName);
                isAuthorComment = true;
                break;
            case "EXPLOSION":
                GameManager.Instance.SpawnExplosion();
                isAuthorComment = true;
                break;
            default:
                break;
        }
        if(isAuthorComment)
            CommentsUi.AddComment(string.Format("[{0}] {1} ({2})", GameManager.Instance.GetTimeText(), text, data.displayName), isAction, isAuthorComment);
        else
            CommentsUi.AddComment(string.Format("[{0}] {1}", GameManager.Instance.GetTimeText(), text), isAction, isAuthorComment);
    }

    public void OnErrorMessageReceived(string message)
    {
        Debug.Log(message);
    }
}
