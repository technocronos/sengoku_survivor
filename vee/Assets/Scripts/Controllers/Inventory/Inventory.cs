using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.Inventory
{
    public sealed class Inventory : Controller
    {
        public sealed class Context : ViewContext
        {
            // nop
        }

        [SerializeField]
        private Transform Content;

        [SerializeField]
        private ListItemInventory listItemPrefab;

        private List<JsonObject> items;

        public override IEnumerator OnViewLoaded(ViewContext viewContext)
        {
            this.items = Api.Items.Get();
            this.Refresh(this.items);
            yield break;
        }

        private void Refresh(List<JsonObject> list)
        {
            for (var index = 0; index < list.Count; index++)
            {
                var raw = list[index];

                var go = GameObject.Instantiate(this.listItemPrefab, this.Content);
                go.Initialize(index);
                go.SetQuantity(raw["quantity"]);

                var sprite = ItemsAndEquipmentResourcesCache.Instance.GetItemSprite(raw["item_id"]);
                go.SetSprite(sprite);
            }
        }
    }
}
