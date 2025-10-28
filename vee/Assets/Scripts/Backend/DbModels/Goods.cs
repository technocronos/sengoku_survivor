using System.Collections;
using System.Collections.Generic;
using MyGame;

namespace Vs.Backend.DbModels
{
    public sealed class Goods : DbModel<Goods>
    {
        protected override JsonObject Cache()
        {
            var results = new List<JsonObject>();
            var raws = MstDatas.Instance.Get("item_mst");
            foreach (var raw in raws)
            {
                var itemId = (int)raw["item_id"];

                var json = new JsonObject();
                json["shop_id"] = 101;
                json["goods_id"] = itemId;
                json["quantity"] = 1;
                json["coins"] = raw["coins"];
                json["item_id"] = itemId;
                json["name"] = raw["name"];
                results.Add(json);
            }
            return results;
        }

        public List<JsonObject> Read()
        {
            return this.GetCached().ToArray();
        }

        public List<JsonObject> Read(int shopId)
        {
            var cached = this.Read();
            return cached.FindAll(i => i["shop_id"] == shopId);
        }

        public JsonObject Buy(int shopId, int goodsId)
        {
            var raws = MstDatas.Instance.Get("item_mst");
            var raw = raws.Find(i => i["item_id"] == goodsId);
            DbModels.Users.Instance.TakeCoins(raw["coins"]);
            DbModels.Items.Instance.Add(raw["item_id"], 1);

            var json = new JsonObject();
            json["user"] = DbModels.Users.Instance.Read()[0];
            return json;
        }
    }
}
