using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentHud : MonoBehaviour
{
    public EquipmentHudIcon[] weaponSlots;
    public EquipmentHudIcon[] itemSlots;

    public void UpdateEquipmentView()
    {
        var allEquipment = Vs.Controllers.Game.GameManager.Instance.EquipmentManager.GetCurrentSkills();
        int wp = 0, it = 0;
        foreach(var slot in weaponSlots)
        {
            slot.gameObject.SetActive(false);
        }
        foreach (var slot in itemSlots)
        {
            slot.gameObject.SetActive(false);
        }
        foreach (var entry in allEquipment)
        {
            var iconName = entry.Key;
            //todo: get icon from cache or resources by iconName

            if (entry.Value.Category == Vs.Controllers.Game.ItemCategory.Accessory)//item
            {
                if (it >= itemSlots.Length) continue;
                itemSlots[it].gameObject.SetActive(true);
                itemSlots[it].EquipmentIcon.sprite = entry.Value.ItemIcon;
                itemSlots[it].EquipmentLvlText.text = string.Format("Lv {0}\n", entry.Value.Level);
                it++;
            }
            else if (entry.Value.Category == Vs.Controllers.Game.ItemCategory.Weapon)//weapon
            {
                if (wp >= weaponSlots.Length) continue;
                weaponSlots[wp].gameObject.SetActive(true);
                weaponSlots[wp].EquipmentIcon.sprite = entry.Value.ItemIcon;
                weaponSlots[wp].EquipmentLvlText.text = string.Format("Lv {0}\n", entry.Value.Level);
                wp++;
            }
        }
    }
}
