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

    private void Awake()
    {
        currentHealthText.text = "";
        currentExpText   .text = "";
        currentLevelText .text = "";
        weaponDebugLabel1.text = "";
        weaponDebugLabel2.text = "";
        weaponDebugLabel3.text = "";
    }

    public void SetCurrHp(int hp)
    {
        currentHealthText.text = hp.ToString();
    }

    public void SetCurrExp(int value)
    {
        currentExpText.text = string.Format("{0}/100", value);
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
                text += string.Format("\n{0} Lvl{1}", skillType.Value.Name, skillType.Value.Level);
            }
        }

        weaponDebugLabel1.text = text;
    }
}