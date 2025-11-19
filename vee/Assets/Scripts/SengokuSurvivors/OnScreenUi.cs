using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using SengokuSurvivors;

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
    private Button DebugButton1;
    [SerializeField]
    private Button DebugButton2;
    [SerializeField]
    private Button DebugButton3;

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

    private List<Button> DebugMenuButtons = new List<Button>();
    private Vs.Controllers.Game.Player Player;
    private SlashController Katana;
    private ProjectileController Projectile;

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
        DebugMenuButton.gameObject.SetActive(true);
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
        });
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
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        }
    }

    public void SetCurrHp(int hp, int maxHp)
    {
        currentHealthText.text = string.Format("{0}/{1}", hp, maxHp);
        hpSlider.value = (float)hp / maxHp;
        hpSlider.gameObject.SetActive(hpSlider.value < 1f - float.Epsilon);
    }

    public void SetExp(int currExp, int maxExp)
    {
        currentExpText.text = string.Format("{0}/{1}", currExp, maxExp);
        expSlider.value = (float)currExp / maxExp;
    }

    public void SetCurrLevel(int value)
    {
        currentLevelText.text = value.ToString();
    }

    public void UpdateEquipmentView()
    {
        string text = "";
        var allEquipment = Vs.Controllers.Game.GameManager.Instance.SkillManager.GetCurrentSkills();
        foreach (var entry in allEquipment)
        {
            foreach (var skillType in entry.SkillTypes)
            {
                if (skillType.Value.Level > 0)
                    text += string.Format("{0} Lvl {1}\n", skillType.Value.Name, skillType.Value.Level);
                else
                    text += string.Format("{0}\n", skillType.Value.Name);
            }
        }

        weaponDebugLabel1.text = text;
    }

    private void Update()
    {
        UpdateDebugStatsView();
        UpdateDebugButtons();
    }

    public void UpdateDebugStatsView()
    {
        if (!DebugStatsContainer.activeSelf) return;
        string text = "";
        text += string.Format("<MOVEMENT>:\n");
        text += string.Format("Current speed(local):x= {0:0.00} y= {1:0.00}\n", Player.DebugData.CurrentMoveSpeed.x, Player.DebugData.CurrentMoveSpeed.y);
        text += string.Format("Max speed X: down= {0} up= {1}\n", Player.DebugData.MaxSpeedYDownUp.x, Player.DebugData.MaxSpeedYDownUp.y);
        text += string.Format("Max speed Y: left= {0} right= {1}\n", Player.DebugData.MaxSpeedXLeftRight.x, Player.DebugData.MaxSpeedXLeftRight.y);
        text += string.Format("Acceleration horizontal: {0}\n", Player.DebugData.AccelerationHor);
        text += string.Format("Acceleration vert: up= {0} down= {1}\n", Player.DebugData.AccelerationVert.x, Player.DebugData.AccelerationVert.y);
        text += string.Format("Friction: {0}\n", Player.DebugData.Friction);
        text += string.Format("<STATS>:\n");
        text += string.Format("HP:{0}/{1}\n", Player.DebugData.Hp.x, Player.DebugData.Hp.y);
        text += string.Format("Max speed (raw): {0}\n", Player.DebugData.MaxSpeedParameter);
        text += string.Format("<KATANA:>\n");
        text += string.Format("Damage: {0}\n", Katana.Damage);
        text += string.Format("Cooldown: {0}\n", Katana.Cooldown);
        text += string.Format("Scale: {0}\n", Katana.Size);
        text += string.Format("<SHURIKEN:>\n");
        text += string.Format("Damage: {0}\n", Projectile.ShurikenDamage);
        text += string.Format("Cooldown: {0}\n", Projectile.CooldownShuriken);
        text += string.Format("Count: {0}\n", Projectile.ShurikenCount);
        text += string.Format("<ARROW:>\n");
        text += string.Format("Damage: {0}\n", Projectile.ArrowDamage);
        text += string.Format("Cooldown: {0}\n", Projectile.CooldownArrow);
        text += string.Format("Count: {0}\n", Projectile.ArrowCount);
        weaponDebugLabel2.text = text;
    }

    private void UpdateDebugButtons()
    {
        if (!DebugMenuContainer.activeSelf) return;
        DebugMenuButtons.Clear();
        for (int i = 0; i < DebugMenuButtonsContainer.childCount; i++)
            DebugMenuButtons.Add(DebugMenuButtonsContainer.GetChild(i).GetComponent<Button>());

        var allEquipment = Vs.Controllers.Game.GameManager.Instance.SkillManager.GetSelectableSkillsAll();
        int buttonIndex = 0;
        foreach (var entry in allEquipment)
        {
            var ii = buttonIndex;
            DebugMenuButtons[buttonIndex].gameObject.SetActive(true);
            DebugMenuButtons[buttonIndex].onClick.RemoveAllListeners();
            DebugMenuButtons[buttonIndex].onClick.AddListener(()=> OnDebugButton(ii));
            DebugMenuButtons[buttonIndex].GetComponentInChildren<TMP_Text>().text = $"{entry["name"]}\n{entry["type_name"]}";
            buttonIndex++;
        }
        for (int i = buttonIndex; i < DebugMenuButtons.Count; i++)
        {
            DebugMenuButtons[i].gameObject.SetActive(false);
        }
    }

    private void OnDebugButton(int index)
    {
        var allEquipment = Vs.Controllers.Game.GameManager.Instance.SkillManager.GetSelectableSkillsAll();
        var row = allEquipment[index];
        var skillId = row["skill_id"];
        var type = row["type"];
        Vs.Controllers.Game.GameManager.Instance.AddSkill(skillId, type);
    }

    private void ToggleStats()
    {
        DebugStatsContainer.SetActive(!DebugStatsContainer.activeSelf);
    }
}