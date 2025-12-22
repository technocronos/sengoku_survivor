using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace Vs.Controllers.Game
{
    public sealed class PopupLvup : MonoBehaviour
    {
        public event System.Action<int> Selected = _ => { };

        [SerializeField]
        private ListItemSkill[] listItems;

        [SerializeField]
        private EquipmentHudIcon[] weaponSlots;
        [SerializeField]
        private EquipmentHudIcon[] itemSlots;


        private List<JsonObject> rows;

        public void Show(List<JsonObject> rows)
        {
            this.gameObject.SetActive(true);
            Time.timeScale = 0;
            this.rows = rows;
            
            // 選択肢の数だけ表示（最大3つ）
            for (var i = 0; i < 3; i++)
            {
                var listItem = this.listItems[i];
                if (i < rows.Count)
                {
                    var row = rows[i];
                    listItem.gameObject.SetActive(true);
                    listItem.Initialize(i);
                    listItem.SetName(row["name"]);
                    listItem.SetDescription(row["description"]);
                    listItem.SetSprite(GameManager.Instance.EquipmentManager.GetSkillSprite(row["item_id"]));
                    
                    // クロージャの問題を回避するため、ローカル変数にコピー
                    int index = i;
                    Button button = listItem.GetComponent<Button>();
                    button.onClick.RemoveAllListeners(); // 既存のリスナーをクリア
                    button.onClick.AddListener(() => OnClicked(index));

                    // var sprite = Resources.Load<Sprite>($"Skills/{raw["image_id"]}");
                    // listItem.SetSprite(sprite);
                }
                else
                {
                    // 3番目以降は非表示
                    listItem.gameObject.SetActive(false);
                }
            }

            UpdateEquipmentView();
        }

        private int getEquipLevel(JsonObject item)
        {
            var allEquipment = Vs.Controllers.Game.GameManager.Instance.EquipmentManager.GetCurrentSkills();
            foreach (var entry in allEquipment)
            {
                if(entry.Value.ItemId == int.Parse(item["item_id"]))
                {
                    return entry.Value.Level;
                }
            }

            return 0;
        }

        public void UpdateEquipmentView()
        {
            var allEquipment = Vs.Controllers.Game.GameManager.Instance.EquipmentManager.GetCurrentSkills();
            int wp = 0, it = 0;
            foreach (var slot in weaponSlots)
            {
                slot.gameObject.SetActive(false);
            }
            foreach (var slot in itemSlots)
            {
                slot.gameObject.SetActive(false);
            }

            var weaponsMst = Vs.Backend.MstDatas.Instance.Get("weapons_mst");
            var accessoriesMst = Vs.Backend.MstDatas.Instance.Get("accessories_mst");

            foreach (var entry in allEquipment)
            {
                var iconName = entry.Key;
                //todo: get icon from cache or resources by iconName

                if (entry.Value.Category == Vs.Controllers.Game.ItemCategory.Accessory)//item
                {
                    if (it >= itemSlots.Length) continue;
                    itemSlots[it].gameObject.SetActive(true);
                    itemSlots[it].EquipmentIcon.sprite = entry.Value.ItemIcon;

                    bool isMaxLevel = !accessoriesMst.Exists(j => (j["level"] == entry.Value.Level + 1) && (j["item_id"] == entry.Value.ItemId));
                    itemSlots[it].EquipmentLvlText.text = isMaxLevel ? "MAX\n" : string.Format("{0}\n", entry.Value.Level);
                    it++;
                }
                else if (entry.Value.Category == Vs.Controllers.Game.ItemCategory.Weapon)//weapon
                {
                    if (wp >= weaponSlots.Length) continue;
                    weaponSlots[wp].gameObject.SetActive(true);
                    weaponSlots[wp].EquipmentIcon.sprite = entry.Value.ItemIcon;

                    bool isMaxLevel = !weaponsMst.Exists(j => (j["level"] == entry.Value.Level + 1) && (j["item_id"] == entry.Value.ItemId));
                    weaponSlots[wp].EquipmentLvlText.text = isMaxLevel ? "MAX\n" : string.Format("{0}\n", entry.Value.Level);
                    wp++;
                }
            }
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
            Time.timeScale = 1;
        }

        public void OnClicked(int index)
        {
            SoundService.Instance.PlaySe("se_getprice");

            this.Hide();

            var raw = this.rows[index];
            this.Selected.Invoke(raw["item_id"]);
        }
    }
}
