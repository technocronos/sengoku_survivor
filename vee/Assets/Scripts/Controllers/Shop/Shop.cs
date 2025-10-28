using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.Shop
{
    public sealed class Shop : Controller
    {
        private const int ShopId1 = 101;
        private const int ShopId2 = 201;
        private const int ShopId3 = 301;

        public sealed class Context : ViewContext
        {
            // nop
        }

        [SerializeField]
        private Transform[] Contents;

        [SerializeField]
        private ListItemShop listItemPrefab;

        private List<JsonObject> list = new List<JsonObject>();

        public override IEnumerator OnViewLoaded(ViewContext viewContext)
        {
            this.list = Api.Goods.Get();
            this.Refresh(this.list);
            yield break;
        }

        private void Refresh(List<JsonObject> list)
        {
            for (var index = 0; index < list.Count; index++)
            {
                var raw = list[index];

                var contents = null as Transform;
                var shopId = (int)raw["shop_id"];
                switch (shopId)
                {
                    case ShopId1: contents = this.Contents[0]; break;
                    case ShopId2: contents = this.Contents[1]; break;
                    case ShopId3: contents = this.Contents[2]; break;
                }

                var go = GameObject.Instantiate(this.listItemPrefab, contents);
                go.Clicked += this.OnListItemClicked;
                go.Initialize(index);
                go.SetName(raw["name"]);
                go.SetPrice(raw["coins"] > 0 ? raw["coins"] : raw["gems"]);
                go.SetCurrency(raw["coins"] > 0 ? "coins" : "gems");
                go.SetQuantity(raw["quantity"]);

                var path = "";
                if (raw["item_id"] > 0)
                {
                    path = $"Items/{raw["item_id"]}";
                }
                else if (raw["equipment_id"] > 0)
                {
                    path = $"Equipments/{raw["equipment_id"]}";
                }

                var sprite = Resources.Load<Sprite>(path);
                go.SetSprite(sprite);
            }
        }

        private void OnListItemClicked(int index)
        {
            var user = UserService.Instance;
            var raw = this.list[index];

            if (raw["coins"] > user.Coins)
            {
                AlertService.Instance.Show("コインが足りません");
                return;
            }
            if (raw["gems"] > user.Gems)
            {
                AlertService.Instance.Show("ジェムが足りません");
                return;
            }

            AlertService.Instance.Show($"{raw["name"]}\nを購入しますか？", onYes: () =>
            {
                var response = Api.Goods.Buy(raw["shop_id"], raw["goods_id"]);
                UserService.Instance.Set(response);
            });
        }
    }
}
