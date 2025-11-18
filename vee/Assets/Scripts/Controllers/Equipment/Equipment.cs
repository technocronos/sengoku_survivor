using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.Equipment
{
    public sealed class Equipment : Controller
    {
        public sealed class Context : ViewContext
        {
            public int CardSeqId = 1;
        }

        [SerializeField]
        private UnityEngine.UI.Text atkText;

        [SerializeField]
        private UnityEngine.UI.Text hpText;

        [SerializeField]
        private Components.ListItemEquipment[] cardEquipmentIcons;

        [SerializeField]
        private PopupEquipment popup;

        [SerializeField]
        private Transform Content;

        [SerializeField]
        private Components.ListItemEquipment listItemPrefab;

        private JsonObject card;
        private List<JsonObject> equipments;
        private List<Components.ListItemEquipment> listItems = new List<Components.ListItemEquipment>();

        private int cardSeqId;
        private int slot;
        private int equipmentSeqId;
        private bool isEquip;

        #region ListItemCache
        private Queue<Components.ListItemEquipment> listItemsCache = new Queue<Components.ListItemEquipment>();
        private Components.ListItemEquipment GetNewItem()
        {
            Components.ListItemEquipment item;
            if (listItemsCache.Count == 0)
            {
                item = Instantiate(this.listItemPrefab, this.Content);
                item.Clicked += this.OnListItemClicked;
            }
            else
            {
                item = listItemsCache.Dequeue();
            }
            item.gameObject.SetActive(true);
            return item;
        }
        private void RemoveItem(Components.ListItemEquipment item)
        {
            listItemsCache.Enqueue(item);
            
            item.gameObject.SetActive(false);
        }
        #endregion


        public override IEnumerator OnViewLoaded(ViewContext viewContext)
        {
            this.popup.EquipButtonClicked += this.OnEquipButtonClicked;
            this.popup.LevelUpButtonClicked += this.OnLevelUpButtonClicked;
            this.popup.LevelUpAllButtonClicked += this.OnLevelUpAllButtonClicked;

            var context = viewContext as Context;
            this.cardSeqId = context.CardSeqId;

            for (var slot = 0; slot < this.cardEquipmentIcons.Length; slot++)
            {
                var go = this.cardEquipmentIcons[slot];
                go.Clicked += this.OnEquipmentClicked;
            }

            this.card = Api.Cards.Get(this.cardSeqId);
            this.RefreshCard(this.card);

            this.equipments = Api.Equipments.Get();
            this.Refresh(this.equipments);
            yield break;
        }

        private void RefreshCard(JsonObject card)
        {
            this.atkText.text = card["atk"];
            this.hpText.text = card["hp"];
        }

        private void Refresh(List<JsonObject> list)
        {
            foreach (var i in this.cardEquipmentIcons)
            {
                i.Unset();
            }

            foreach (var i in this.listItems)
            {
                RemoveItem(i);
            }
            this.listItems.Clear();

            for (var index = 0; index < list.Count; index++)
            {
                var raw = list[index];

                var go = null as Components.ListItemEquipment;

                if (raw["card_seq_id"] > 0)
                {
                    go = this.cardEquipmentIcons[raw["slot"]];
                }
                else
                {
                    go = GetNewItem();
                    this.listItems.Add(go);
                }
                go.Initialize(index);
                go.Set();
                go.SetLevel(raw["level"]);
                go.SetRarity(raw["rarity"]);
                go.SetRank(raw["rank"]);

                var sprite = ItemsAndEquipmentResourcesCache.Instance.GetEquipmentSprite(raw["equipment_id"]);
                go.SetSprite(sprite);
            }
        }

        public void OnMergeButtonClicked()
        {
            var context = new Controllers.EquipmentMerger.EquipmentMerger.Context();
            ViewService.Instance.ChangeView(context);
        }

        private void OnEquipmentClicked(int index)
        {
            var raw = this.equipments[index];
            this.slot = raw["slot"];
            this.equipmentSeqId = raw["equipment_seq_id"];
            this.isEquip = false;
            this.popup.Show(raw);
        }

        private void OnListItemClicked(int index)
        {
            var raw = this.equipments[index];
            this.slot = raw["slot"];
            this.equipmentSeqId = raw["equipment_seq_id"];
            this.isEquip = true;
            this.popup.Show(raw);
        }

        private void OnEquipButtonClicked()
        {
            this.popup.Hide();

            var response = null as JsonObject;
            if (this.isEquip)
            {
                response = Api.Cards.Equip(this.cardSeqId, this.slot, this.equipmentSeqId);
            }
            else
            {
                response = Api.Cards.UnEquip(this.cardSeqId, this.slot);
            }

            this.RefreshCard(response["card"]);
            this.Refresh(response["equipments"]);
        }

        private void OnLevelUpButtonClicked()
        {
            var equipment = this.equipments.Find(i => i["equipment_seq_id"] == this.equipmentSeqId);
            if (equipment["is_short_coins"])
            {
                AlertService.Instance.Show("コインが足りません");
                return;
            }
            if (equipment["is_short_items"])
            {
                AlertService.Instance.Show("素材が足りません。");
                return;
            }

            this.popup.Hide();

            var response = Api.Equipments.LevelUp(this.equipmentSeqId);
            var cards = response["card"].ToArray();
            var card = cards.Find(i => i["card_seq_id"] == this.cardSeqId);

            this.RefreshCard(card);
            this.Refresh(response["equipments"]);
        }

        private void OnLevelUpAllButtonClicked()
        {
            var equipment = this.equipments.Find(i => i["equipment_seq_id"] == this.equipmentSeqId);
            if (equipment["is_short_coins"])
            {
                AlertService.Instance.Show("コインが足りません");
                return;
            }
            if (equipment["is_short_items"])
            {
                AlertService.Instance.Show("素材が足りません。");
                return;
            }

            this.popup.Hide();

            var response = Api.Equipments.LevelUpAll(this.equipmentSeqId);
            var cards = response["card"].ToArray();
            var card = cards.Find(i => i["card_seq_id"] == this.cardSeqId);

            this.RefreshCard(card);
            this.Refresh(response["equipments"]);
        }
    }
}
