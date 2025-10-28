using System.Collections;
using System.Collections.Generic;
using MyGame;

namespace Vs.Backend.DbModels
{
    public sealed class Items : DbModel<Items>
    {
        protected override JsonObject Cache()
        {
            var results = new List<JsonObject>();
            var raws = MstDatas.Instance.Get("item_mst");
            var rows = DbService.Instance.Db.Items;
            foreach (var row in rows)
            {
                var raw = raws.Find(i => i["item_id"] == row.ItemId);

                var json = new JsonObject();
                json["item_id"] = row.ItemId;
                json["quantity"] = row.Quantity;
                json["name"] = raw["name"];
                results.Add(json);
            }
            return results;
        }

        public List<JsonObject> Read()
        {
            return this.GetCached();
        }

        public List<JsonObject> Update(int itemId)
        {
            this.Dirty();
            return this.Read();
        }

        public List<JsonObject> Delete(int itemId)
        {
            this.Dirty();
            return this.Read();
        }

        public List<JsonObject> Add(int itemId, int quantity)
        {
            var rows = DbService.Instance.Db.Items;
            var row = rows.Find(i => i.ItemId == itemId);
            if (row == null)
            {
                row = new Structs.Item();
                row.ItemId = itemId;
                rows.Add(row);
            }
            row.Quantity += quantity;

            this.Dirty();
            return this.Read();
        }

        public List<JsonObject> Sub(int itemId, int quantity)
        {
            var rows = DbService.Instance.Db.Items;
            var row = rows.Find(i => i.ItemId == itemId);
            row.Quantity -= quantity;

            this.Dirty();
            return this.Read();
        }
    }
}
