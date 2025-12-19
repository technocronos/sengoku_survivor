using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;
using TMPro;

namespace Vs.Controllers.Game
{
    public sealed class PopupResult : MonoBehaviour
    {
        private System.Action callbackOnce;

        [SerializeField]
        private GameObject WinTitle;
        [SerializeField]
        private GameObject LoseTitle;
        [SerializeField]
        private TextMeshProUGUI score;
        [SerializeField]
        private TextMeshProUGUI time;

        [SerializeField]
        private EquipmentHudIcon[] weaponSlots;
        [SerializeField]
        private EquipmentHudIcon[] itemSlots;

        public class GameResult
        {
            public static readonly int Win = 1;
            public static readonly int Lose = 2;

            public int Result { get; set; }

            public GameResult(int result)
            {
                this.Result = result;
            }
        }

        public void Show(GameResult result, System.Action callbackOnce = null)
        {
            this.callbackOnce = callbackOnce;
            this.gameObject.SetActive(true);
            Time.timeScale = 0.0f;
            WinTitle.SetActive(false);
            LoseTitle.SetActive(false);

            SoundService.Instance.StopBgm();
            score.text = GameManager.Instance.totalScore.ToString();
            time.text = GameManager.Instance.timeText.text;

            UpdateEquipmentView();

            if (result.Result == GameResult.Win)
            {
                WinTitle.SetActive(true);
                SoundService.Instance.PlaySe("se_congrats");
            }
            else
            {
                LoseTitle.SetActive(true);
                SoundService.Instance.PlaySe("se_retire");
            }
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
            Time.timeScale = 1.0f;

            SoundService.Instance.PlayBgm("bgm2");
        }

        public void OnClicked()
        {
            this.Hide();
            if (this.callbackOnce != null)
            {
                this.callbackOnce.Invoke();
                this.callbackOnce = null;
            }
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
                    itemSlots[it].EquipmentLvlText.text = isMaxLevel ? "Lv MAX\n" : string.Format("Lv {0}\n", entry.Value.Level);
                    it++;
                }
                else if (entry.Value.Category == Vs.Controllers.Game.ItemCategory.Weapon)//weapon
                {
                    if (wp >= weaponSlots.Length) continue;
                    weaponSlots[wp].gameObject.SetActive(true);
                    weaponSlots[wp].EquipmentIcon.sprite = entry.Value.ItemIcon;

                    bool isMaxLevel = !weaponsMst.Exists(j => (j["level"] == entry.Value.Level + 1) && (j["item_id"] == entry.Value.ItemId));
                    weaponSlots[wp].EquipmentLvlText.text = isMaxLevel ? "Lv MAX\n" : string.Format("Lv {0}\n", entry.Value.Level);
                    wp++;
                }
            }
        }
        public void OnButtonToTitle()
        {
            var context = new Controllers.MyPage.MyPage.Context();
            ViewService.Instance.ChangeView(context);
        }
    }
}
