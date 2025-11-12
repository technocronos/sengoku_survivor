using UnityEngine;
using UnityEngine.UI;
using TMPro;


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

    private void Awake()
    {
        currentHealthText.text = "";
        currentExpText   .text = "";
        currentLevelText .text = "";
        weaponDebugLabel1.text = "";
        weaponDebugLabel2.text = "";
        weaponDebugLabel3.text = "";
        DebugButton1.onClick.AddListener(() => { 
            Vs.Controllers.Game.GameManager.Instance.AddSkill(901, 1);
            Vs.Controllers.Game.GameManager.Instance.AddSkill(901, 2);
            Vs.Controllers.Game.GameManager.Instance.AddSkill(901, 3);
        });
        DebugButton2.onClick.AddListener(() => {
            var skillId = 903;
            var type = Vs.Controllers.Game.GameManager.Instance.SkillManager.IsBaseSkillObtained(skillId) ? 1 : 0;
            Vs.Controllers.Game.GameManager.Instance.AddSkill(skillId, type); 
        });
        DebugButton3.onClick.AddListener(() => {
            var skillId = 902;
            var type = Vs.Controllers.Game.GameManager.Instance.SkillManager.IsBaseSkillObtained(skillId) ? 1 : 0;
            Vs.Controllers.Game.GameManager.Instance.AddSkill(skillId, type);
        });

        DebugButton1.gameObject.SetActive(false);
        DebugButton2.gameObject.SetActive(false);
        DebugButton3.gameObject.SetActive(false);
        DebugMenuButton.gameObject.SetActive(true);
        DebugMenuCloseButton.gameObject.SetActive(false);

        DebugMenuCloseButton.onClick.AddListener(() =>{
            DebugButton1.gameObject.SetActive(false);
            DebugButton2.gameObject.SetActive(false);
            DebugButton3.gameObject.SetActive(false);
            DebugMenuButton.gameObject.SetActive(true);
            DebugMenuCloseButton.gameObject.SetActive(false);
        });
        DebugMenuButton.onClick.AddListener(() =>{
            DebugButton1.gameObject.SetActive(true);
            DebugButton2.gameObject.SetActive(true);
            DebugButton3.gameObject.SetActive(true);
            DebugMenuButton.gameObject.SetActive(false);
            DebugMenuCloseButton.gameObject.SetActive(true);
        });
    }

    private void Update()
    {
        if (hpFadeSlider.value != hpSlider.value)
        {
            hpFadeSlider.value += (hpSlider.value - hpFadeSlider.value) * 2f * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    public void SetCurrHp(int hp, int maxHp)
    {
        currentHealthText.text = string.Format("{0}/{1}", hp, maxHp);
        hpSlider.value = (float)hp / maxHp;
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
}