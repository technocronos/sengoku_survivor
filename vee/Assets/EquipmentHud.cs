using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentHud : MonoBehaviour
{
    public EquipmentHudIcon[] weaponSlots;
    public EquipmentHudIcon[] itemSlots;

    public void UpdateEquipmentView()
    {
        string text = "";
        var allEquipment = Vs.Controllers.Game.GameManager.Instance.SkillManager.GetCurrentSkills();
        int wp = 0, it = 0;
        foreach (var entry in allEquipment)
        {
            var iconName = entry.SkillId;
            //todo: get icon from cache or resources by iconName

            if (entry.Category == 201)//item
            {
                if (it >= itemSlots.Length) continue;
                itemSlots[it].EquipmentIcon.sprite = null; //sprite by iconName
                itemSlots[it].EquipmentLvlText.text = string.Format("Lvl {0}\n", entry.SkillTypes[0].Level);
                //todo: do icons on level up too (and itembox too)
                it++;
            }
            else//weapon
            {
                if (wp >= weaponSlots.Length) continue;
                weaponSlots[wp].EquipmentIcon.sprite = null; //sprite by iconName
                weaponSlots[wp].EquipmentLvlText.text = string.Format("Lvl {0}\n", entry.SkillTypes[0].Level);
                wp++;
            }
            text += string.Format("{0} Lvl {1}\n", entry.SkillTypes[0].Name, entry.SkillTypes[0].Level);
        }
    }
}
