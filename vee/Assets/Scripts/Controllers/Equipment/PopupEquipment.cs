using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.Equipment
{
    public sealed class PopupEquipment : Controller
    {
        public event System.Action EquipButtonClicked = () => { };
        public event System.Action LevelUpButtonClicked = () => { };
        public event System.Action LevelUpAllButtonClicked = () => { };

        [SerializeField]
        private UnityEngine.UI.Image icon;

        [SerializeField]
        private UnityEngine.UI.Image iconBg;

        [SerializeField]
        private UnityEngine.UI.Text nameText;

        [SerializeField]
        private UnityEngine.UI.Text levelText;

        [SerializeField]
        private UnityEngine.UI.Text statsText;

        [SerializeField]
        private UnityEngine.UI.Image statsIcon;

        [SerializeField]
        private UnityEngine.UI.Text descriptionText;

        [SerializeField]
        private UnityEngine.UI.Text coinsText;

        [SerializeField]
        private UnityEngine.UI.Image requireItemImage;

        [SerializeField]
        private UnityEngine.UI.Text requireItemQuantityText;

        [SerializeField]
        private UnityEngine.UI.Text equipButtonText;

        private readonly Dictionary<string, Sprite> statsTypesSpriteResourceCache = new Dictionary<string, Sprite>();

        public void Show(JsonObject raw)
        {
            this.gameObject.SetActive(true);

            var stats = raw["atk"] > 0 ? raw["atk"] : raw["hp"];

            this.nameText.text = raw["name"];
            this.levelText.text = $"{raw["level"]}/{raw["level_max"]}";
            this.statsText.text = $"+{stats}";
            this.descriptionText.text = raw["description"];
            this.equipButtonText.text = raw["card_seq_id"] >= 0 ? "装備解除" : "装備";

            var sprite = ItemsAndEquipmentResourcesCache.Instance.GetEquipmentSprite(raw["equipment_id"]);
            this.icon.sprite = sprite;
            this.iconBg.color = Utils.GetRarityColor(raw["rarity"]);

            var statsType = raw["atk"] > 0 ? "atk" : "hp";
            if (!statsTypesSpriteResourceCache.ContainsKey(statsType))
            {
                statsTypesSpriteResourceCache.Add(statsType, Resources.Load<Sprite>($"Stats/{statsType}"));
            }
            sprite = statsTypesSpriteResourceCache[statsType];
            this.statsIcon.sprite = sprite;
            this.coinsText.text = $"{raw["require_coins"]}/{UserService.Instance.Coins}";

            sprite = ItemsAndEquipmentResourcesCache.Instance.GetItemSprite(raw["require_item_id"]);
            this.requireItemImage.sprite = sprite;
            this.requireItemQuantityText.text = $"{raw["require_item_quantity"]}/{raw["item_quantity"]}";
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public void OnCloseButtonClicked()
        {
            this.Hide();
        }

        public void OnEquipButtonClicked()
        {
            this.EquipButtonClicked.Invoke();
        }

        public void OnLevelUpButtonClicked()
        {
            this.LevelUpButtonClicked.Invoke();
        }

        public void OnLevelUpAllButtonClicked()
        {
            this.LevelUpAllButtonClicked.Invoke();
        }
    }
}
