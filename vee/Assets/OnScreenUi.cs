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

    private void Awake()
    {
        currentHealthText.text = "";
        currentExpText   .text = "";
        currentLevelText .text = "";
        weaponDebugLabel1.text = "";
        weaponDebugLabel2.text = "";
        weaponDebugLabel3.text = "";
    }

    private void Update()
    {
        if (hpFadeSlider.value != hpSlider.value)
        {
            hpFadeSlider.value += (hpSlider.value - hpFadeSlider.value) * 2f * Time.deltaTime;
        }
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