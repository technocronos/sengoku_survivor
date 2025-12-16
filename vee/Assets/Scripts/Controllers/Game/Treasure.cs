using MyGame;
using SengokuSurvivors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class Treasure : Item
    {
        private int item_id;
        private int value;
        private List<JsonObject> item_mst;

        protected override void OnComplete()
        {
            GameManager.Instance.onScreenTreasureCount--;
            GameManager.Instance.Recover(this.value);
        }

        public void Setup(int item_id, DropManager dropManager)
        {
            GameManager.Instance.onScreenTreasureCount++;

            this.item_mst = Vs.Backend.MstDatas.Instance.Get("item_mst");
            var itemRow = this.item_mst.Find(row => row["item_id"] == item_id);

            this.dropManager = dropManager;
            this.item_id = item_id;
            this.value = itemRow["value"];
        }
    }
}
