using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.EquipmentMerger
{
    public sealed class EquipmentMerger : Controller
    {
        public sealed class Context : ViewContext
        {
            // nop
        }

        [SerializeField]
        private Components.ListItemEquipment[] icons;

        [SerializeField]
        private UnityEngine.UI.Text nameText;

        [SerializeField]
        private UnityEngine.UI.Text levelText;

        [SerializeField]
        private UnityEngine.UI.Text statsText;

        [SerializeField]
        private UnityEngine.UI.Text[] requireSlotTexts;

        [SerializeField]
        private UnityEngine.UI.Button mergeButton;

        [SerializeField]
        private Transform Content;

        [SerializeField]
        private Components.ListItemEquipment listItemPrefab;

        private List<JsonObject> equipments;
        private List<JsonObject> filtered;
        private List<Components.ListItemEquipment> listItems = new List<Components.ListItemEquipment>();

        private int[] equipmentSeqIds = new int[3];

        public override IEnumerator OnViewLoaded(ViewContext viewContext)
        {
            this.equipments = Api.Equipments.Get();
            this.filtered = this.equipments;
            this.Refresh(this.filtered);

            for (var index = 0; index < this.icons.Length; index++)
            {
                var icon = this.icons[index];
                icon.Initialize(index);
                icon.Unset();
            }

            this.requireSlotTexts[0].gameObject.SetActive(false);
            this.requireSlotTexts[1].gameObject.SetActive(false);
            yield break;
        }

        private void Refresh(List<JsonObject> list)
        {
            foreach (var i in this.listItems)
            {
                GameObject.Destroy(i.gameObject);
            }
            this.listItems.Clear();

            for (var index = 0; index < list.Count; index++)
            {
                var raw = list[index];

                var go = GameObject.Instantiate(this.listItemPrefab, this.Content);
                go.Clicked += this.OnListItemClicked;
                go.Initialize(index);
                go.Set();
                go.SetLevel(raw["level"]);
                go.SetRarity(raw["rarity"]);
                go.SetRank(raw["rank"]);
                this.listItems.Add(go);

                var sprite = ItemsAndEquipmentResourcesCache.Instance.GetEquipmentSprite(raw["equipment_id"]);
                go.SetSprite(sprite);
            }
        }

        private void OnListItemClicked(int index)
        {
            if (this.equipmentSeqIds[0] == 0)
            {
                this.On1st(index);
            }
            else if (this.equipmentSeqIds[1] == 0)
            {
                this.On2nd(index);
            }
            else
            {
                this.On3rd(index);
            }
        }

        private void On1st(int index)
        {
            var raw = this.filtered[index];
            var slot = (int)raw["slot"];
            var rarity = (int)raw["rarity"];
            this.equipmentSeqIds[0] = raw["equipment_seq_id"];

            SetIcon(this.icons[0], raw);

            this.nameText.text = raw["name"];
            this.levelText.text = $"最大レベル {raw["level_max"]} → {raw["next_level_max"]} ";
            var label = raw["atk"] > 0 ? "攻撃力" : "HP";
            this.statsText.text = $"{label} {raw["atk"]} → {raw["next_atk"]}";

            this.icons[1].SetRarity(rarity);
            this.requireSlotTexts[0].gameObject.SetActive(true);
            this.requireSlotTexts[0].text = GetSlotName(slot);

            this.icons[2].SetRarity(rarity);
            this.requireSlotTexts[1].gameObject.SetActive(true);
            this.requireSlotTexts[1].text = GetSlotName(slot);

            this.filtered = this.equipments.FindAll(i =>
                i["slot"] == slot &&
                i["rarity"] == rarity &&
                !System.Array.Exists(this.equipmentSeqIds, j => j == i["equipment_seq_id"])
            );
            this.Refresh(this.filtered);
        }

        private void On2nd(int index)
        {
            var raw = this.filtered[index];
            var equipmentSeqId = (int)raw["equipment_seq_id"];
            var slot = (int)raw["slot"];
            var rarity = (int)raw["rarity"];

            this.equipmentSeqIds[1] = equipmentSeqId;
            SetIcon(this.icons[1], raw);

            this.filtered = this.equipments.FindAll(i =>
                i["slot"] == slot &&
                i["rarity"] == rarity &&
                !System.Array.Exists(this.equipmentSeqIds, j => j == i["equipment_seq_id"])
            );
            this.Refresh(this.filtered);
        }

        private void On3rd(int index)
        {
            var raw = this.filtered[index];
            var equipmentSeqId = (int)raw["equipment_seq_id"];

            this.equipmentSeqIds[2] = equipmentSeqId;
            SetIcon(this.icons[2], raw);
            this.mergeButton.interactable = true;
        }

        public void OnMergeButtonClicked()
        {
            var response = Api.Equipments.Merge(this.equipmentSeqIds[0], this.equipmentSeqIds[1], this.equipmentSeqIds[2]);
            AlertService.Instance.Show("合成しました。", onOk: () =>
            {
                var context = new Controllers.Equipment.Equipment.Context();
                ViewService.Instance.ChangeView(context);
            });
        }

        private static void SetIcon(Components.ListItemEquipment icon, JsonObject raw)
        {
            var go = icon;
            go.Set();
            go.SetLevel(raw["level"]);
            go.SetRank(raw["rank"]);

            var sprite = ItemsAndEquipmentResourcesCache.Instance.GetEquipmentSprite(raw["equipment_id"]);
            go.SetSprite(sprite);
        }

        private static string GetSlotName(int slot)
        {
            switch (slot)
            {
                case 0: return "武器";
                case 1: return "服";
                case 2: return "ネックレス";
                case 3: return "ベルト";
                case 4: return "グローブ";
                case 5: return "シューズ";
                default: return "";
            }
        }
    }
}
