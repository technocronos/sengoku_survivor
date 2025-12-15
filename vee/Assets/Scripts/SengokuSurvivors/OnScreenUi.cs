using MyGame;
using SengokuSurvivors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UNCHAIN.ThirdSdk;
using UnityEngine;
using UnityEngine.UI;
using Vs.Backend;
using Vs.Controllers.Game;

public class OnScreenUi : MyGame.SingletonMonoBehaviour<OnScreenUi>
{
    [SerializeField]
    private TMP_Text currentHealthText;
    [SerializeField]
    private TMP_Text currentExpText;
    [SerializeField]
    private TMP_Text currentLevelText;
    
    [SerializeField]
    private TMP_Text weaponDebugLabel1;
    [SerializeField]
    private TMP_Text weaponDebugLabel2;
    [SerializeField]
    private TMP_Text weaponDebugLabel3;

    [SerializeField]
    private Slider expSlider;
    [SerializeField]
    private Slider hpSlider;
    [SerializeField]
    private Slider hpFadeSlider;

    [SerializeField]
    private Image gauge;

    [SerializeField]
    private Button DebugButton1;
    [SerializeField]
    private Button DebugButton2;
    [SerializeField]
    private Button DebugButton3;

    [SerializeField]
    private TextMeshProUGUI TextOnScreenEnemy;

    [SerializeField]
    private Button DebugMenuButton;
    [SerializeField]
    private Button DebugMenuCloseButton;
    [SerializeField]
    private GameObject DebugMenuContainer;
    [SerializeField]
    private Transform DebugMenuButtonsContainer;
    [SerializeField]
    private GameObject DebugStatsContainer;
    [SerializeField]
    private EquipmentHud equipmentHud;

    private List<Button> DebugMenuButtons = new List<Button>();
    private Vs.Controllers.Game.Player Player;
    private SlashController Katana;
    private ProjectileController Projectile;
    private readonly StringBuilder sb = new StringBuilder();
    private bool ShowDebugMenu = false;
    private ThirdController thirdController;

    private void Awake()
    {
        Player = FindAnyObjectByType<Vs.Controllers.Game.Player>();
        Projectile = FindAnyObjectByType<ProjectileController>();
        Katana = FindAnyObjectByType<SlashController>();
        DebugStatsContainer.SetActive(false);

        currentHealthText.text = "";
        currentExpText   .text = "";
        currentLevelText .text = "";
        weaponDebugLabel1.text = "";
        weaponDebugLabel2.text = "";
        weaponDebugLabel3.text = "";
        DebugButton1.onClick.AddListener(() => {
            ToggleStats();
        });

        DebugMenuContainer.SetActive(false);
#if DEVELOP
        ShowDebugMenu = true;
#else
        ShowDebugMenu = false;
#endif

        DebugMenuButton.gameObject.SetActive(ShowDebugMenu);
        DebugMenuCloseButton.gameObject.SetActive(false);

        DebugMenuCloseButton.onClick.AddListener(() =>{
            DebugMenuContainer.SetActive(false);
            DebugMenuButton.gameObject.SetActive(true);
            DebugMenuCloseButton.gameObject.SetActive(false);
        });
        DebugMenuButton.onClick.AddListener(() =>{
            DebugMenuContainer.SetActive(true);
            DebugMenuButton.gameObject.SetActive(false);
            DebugMenuCloseButton.gameObject.SetActive(true);
            UpdateDebugButtons();
        });

        thirdController = FindAnyObjectByType<ThirdController>();
    }

    private void Start()
    {
        StartCoroutine(UnscaledRoutine());
    }

    private IEnumerator UnscaledRoutine()
    {
        while(true)
        {
            yield return null;
            if (hpFadeSlider.value != hpSlider.value)
            {
                hpFadeSlider.value += (hpSlider.value - hpFadeSlider.value) * 2f * Time.unscaledDeltaTime;
            }
        }
    }

    public void SetOnScreenEnemy(Dictionary<int,int> onScreenEnemy)
    {
        TextOnScreenEnemy.text = "";

        if (onScreenEnemy == null || onScreenEnemy.Count == 0)
        {
            return;
        }

        sb.Clear();

        var enemyMst = MstDatas.Instance.Get("enemy_mst");        
        float litateTotal = 0;
        foreach (var kvp in onScreenEnemy.OrderBy(x => x.Key))
        {
            var name = "";

            // enemy_mstからirritate値を取得
            var enemyData = enemyMst.Find(i => i["enemy_id"] == kvp.Key);
            if (enemyData != null)
            {
                var irritate = (int)enemyData["irritate"];
                // 出現数にirritate/1000をかけて重みづけ
                litateTotal += kvp.Value * (irritate / 1000.0f);
                name = (string)enemyData["name"];
            }
            else
            {
                // enemy_mstにデータがない場合は出現数のまま
                litateTotal += kvp.Value;
                name = "その他";
            }

            sb.AppendLine($"{name}:{kvp.Value}");
        }

        sb.AppendLine($"");
        sb.AppendLine($"イライラpt：{litateTotal}");

        TextOnScreenEnemy.text = sb.ToString();
    }

    public int GetIrritatePoint(Dictionary<int, int> onScreenEnemy)
    {
        if (onScreenEnemy == null || onScreenEnemy.Count == 0)
        {
            return 0;
        }
        float litateTotal = 0;
        var enemyMst = MstDatas.Instance.Get("enemy_mst");
        foreach (var kvp in onScreenEnemy)
        {
            // enemy_mstからirritate値を取得
            var enemyData = enemyMst.Find(i => i["enemy_id"] == kvp.Key);
            if (enemyData != null)
            {
                var irritate = (int)enemyData["irritate"];
                // 出現数にirritate/1000をかけて重みづけ
                litateTotal += kvp.Value * (irritate / 1000.0f);
            }
            else
            {
                // enemy_mstにデータがない場合は出現数のまま
                litateTotal += kvp.Value;
            }
        }
        return Mathf.FloorToInt(litateTotal);
    }

    public void SetCurrHp(int hp, int maxHp)
    {
        currentHealthText.text = string.Format("{0}/{1}", hp, maxHp);
        hpSlider.value = (float)hp / maxHp;
        //hpSlider.gameObject.SetActive(hpSlider.value < 1f - float.Epsilon);

        Debug.Log("SetCurrHp");
        var rect = gauge.transform.GetComponent<RectTransform>().rect;
        float gauge_width = gauge.transform.GetComponent<RectTransform>().rect.width; ;

        gauge_width = 143;

        int posx = (int)(((hp * 1.0f) / maxHp) * gauge_width);
        gauge.transform.localPosition = new Vector3(posx - gauge_width, 0, 0);

    }

    public void SetExp(int currExp, int maxExp)
    {
        currentExpText.text = string.Format("{0}/{1}", currExp, maxExp);
        expSlider.value = (float)currExp / maxExp;
    }

    public void SetCurrLevel(int value)
    {
        currentLevelText.text = "どうせつすう " + (value * 100).ToString("N0");
    }

    public void UpdateEquipmentView()
    {
        equipmentHud.UpdateEquipmentView();
        //string text = "";
        //var allEquipment = Vs.Controllers.Game.GameManager.Instance.EquipmentManager.GetCurrentSkills();
        //foreach (var entry in allEquipment)
        //{
        //    text += string.Format("{0} Lvl {1}\n", entry.SkillTypes[0].Name, entry.SkillTypes[0].Level);
        //    //foreach (var skillType in entry.SkillTypes)
        //    //{
        //    //    if (skillType.Value.Level > 0)
        //    //        text += string.Format("{0} Lvl {1}\n", skillType.Value.Name, skillType.Value.Level);
        //    //    else
        //    //        text += string.Format("{0}\n", skillType.Value.Name);
        //    //}
        //}

        //weaponDebugLabel1.text = text;
    }

    private void Update()
    {
        UpdateDebugStatsView();
        if (Input.GetKeyDown(KeyCode.Escape)) Vs.Controllers.Game.GameManager.Instance.OnPauseClicked();

#if DEVELOP
        ThirdResponse response = new ThirdResponse();
        response.txId = "019adf3c-dad1-7732-a70c-eb07f35f8a18";
        response.streamId = "019ade66-0868-72eb-804d-9117fa2d091c";
        response.actionId = "019ade76-7619-71f9-b75c-3e4d02dfe456";
        response.displayName = "デバッグ";
        response.commandKey = "";
        response.quantity = "1";

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            response.commandKey = "HEAL_SMALL";
        }else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            response.commandKey = "HEAL_MEDIUM";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            response.commandKey = "HEAL_LARGE";
        }else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            response.commandKey = "SPEED_BOOST_ITEM";
        }else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            response.commandKey = "KNOCKBACK_BOOST_ITEM";
        }else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            response.commandKey = "CHAT_TEXT_QUESTION_DOG";
        }else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            response.commandKey = "CHAT_TEXT_888";
        }else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            response.commandKey = "CHAT_TEXT_FIRST_VISIT";
        }else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            response.commandKey = "CHAT_TEXT_LIKE";
        }else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            response.commandKey = "NAMECHAT_TEXT_QUESTION_DOG";
        }else if (Input.GetKeyDown(KeyCode.Q))
        {
            response.commandKey = "NAMECHAT_TEXT_888";
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            response.commandKey = "NAMECHAT_TEXT_FIRST_VISIT";
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            response.commandKey = "NAMECHAT_TEXT_LIKE";
        }


        if (response.commandKey != "")
            thirdController.OnMessageReceived(response);

#endif
    }

    public void UpdateDebugStatsView()
    {
        if (!DebugStatsContainer.activeSelf) return;
        sb.Clear();
        sb.AppendLine(string.Format("<MOVEMENT>:"));
        sb.Append(string.Format("Current speed(local):x= {0:0.00} y= {1:0.00}\n", Player.DebugData.CurrentMoveSpeed.x, Player.DebugData.CurrentMoveSpeed.y));
        sb.Append(string.Format("Max speed X: down= {0} up= {1}\n", Player.DebugData.MaxSpeedYDownUp.x, Player.DebugData.MaxSpeedYDownUp.y));
        sb.Append(string.Format("Max speed Y: left= {0} right= {1}\n", Player.DebugData.MaxSpeedXLeftRight.x, Player.DebugData.MaxSpeedXLeftRight.y));
        sb.Append(string.Format("Acceleration horizontal: {0}\n", Player.DebugData.AccelerationHor));
        sb.Append(string.Format("Acceleration vert: up= {0} down= {1}\n", Player.DebugData.AccelerationVert.x, Player.DebugData.AccelerationVert.y));
        sb.Append(string.Format("Friction: {0}\n", Player.DebugData.Friction));
        sb.Append(string.Format("Sqr Friction: {0}\n", Player.DebugData.SqrFriction));
        sb.Append(string.Format("<STATS>:\n"));
        sb.Append(string.Format("HP:{0}/{1}\n", Player.DebugData.Hp.x, Player.DebugData.Hp.y));
        sb.Append(string.Format("Max speed (raw): {0}\n", Player.DebugData.MaxSpeedParameter));
        sb.Append(string.Format("<KATANA:>\n"));
        sb.Append(string.Format("Damage: {0}\n", Katana.Damage));
        sb.Append(string.Format("Cooldown: {0}\n", Katana.Cooldown));
        sb.Append(string.Format("<SHURIKEN:>\n"));
        sb.Append(string.Format("Damage: {0}\n", Projectile.ShurikenDamage));
        sb.Append(string.Format("Cooldown: {0}\n", Projectile.CooldownShuriken));
        sb.Append(string.Format("Count: {0}\n", Projectile.ShurikenCount));
        sb.Append(string.Format("<ARROW:>\n"));
        sb.Append(string.Format("Damage: {0}\n", Projectile.ArrowDamage));
        sb.Append(string.Format("Cooldown: {0}\n", Projectile.CooldownArrow));
        sb.Append(string.Format("Count: {0}\n", Projectile.ArrowCount));
        weaponDebugLabel2.text = sb.ToString();
    }

    public void UpdateDebugButtons()
    {
        if (!DebugMenuContainer.activeSelf) return;
        DebugMenuButtons.Clear();
        for (int i = 0; i < DebugMenuButtonsContainer.childCount; i++)
            DebugMenuButtons.Add(DebugMenuButtonsContainer.GetChild(i).GetComponent<Button>());

        var allEquipment = Vs.Controllers.Game.GameManager.Instance.EquipmentManager.GetSelectableSkillsAll();
        int buttonIndex = 0;
        foreach (var entry in allEquipment)
        {
            ItemCategory category = (ItemCategory)(int)entry["category"];
            if (category == ItemCategory.item) continue;

            var ii = buttonIndex;
            DebugMenuButtons[buttonIndex].gameObject.SetActive(true);
            DebugMenuButtons[buttonIndex].onClick.RemoveAllListeners();
            DebugMenuButtons[buttonIndex].onClick.AddListener(()=> OnDebugButton(ii));
            DebugMenuButtons[buttonIndex].GetComponentInChildren<TMP_Text>().text = $"{entry["name"]}";
            buttonIndex++;
        }
        for (int i = buttonIndex; i < DebugMenuButtons.Count; i++)
        {
            DebugMenuButtons[i].gameObject.SetActive(false);
        }
    }

    private void OnDebugButton(int index)
    {
        var allEquipment = Vs.Controllers.Game.GameManager.Instance.EquipmentManager.GetSelectableSkillsAll();
        var row = allEquipment[index];
        var skillId = row["item_id"];
        Vs.Controllers.Game.GameManager.Instance.AddSkill(skillId);
    }

    private void ToggleStats()
    {
        DebugStatsContainer.SetActive(!DebugStatsContainer.activeSelf);
    }
}