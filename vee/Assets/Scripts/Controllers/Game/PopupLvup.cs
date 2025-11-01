using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.Game
{
    public sealed class PopupLvup : MonoBehaviour
    {
        public event System.Action<int> Selected = _ => { };

        [SerializeField]
        private ListItemSkill[] listItems;

        private List<JsonObject> rows;

        public void Show(List<JsonObject> rows)
        {
            this.gameObject.SetActive(true);
            Time.timeScale = 0;
            this.rows = rows;
            for (var i = 0; i < 3; i++)
            {
                var listItem = this.listItems[i];
                var row = rows[i];
                listItem.Initialize(i);
                listItem.SetName(row["name"]);
                listItem.SetDescription(row["description"]);

                // var sprite = Resources.Load<Sprite>($"Skills/{raw["image_id"]}");
                // listItem.SetSprite(sprite);
            }
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
            Time.timeScale = 1;
        }

        public void OnClicked(int index)
        {
            this.Hide();

            var raw = this.rows[index];
            this.Selected.Invoke(raw["skill_id"]);
        }
    }
}
