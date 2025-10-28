using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyGame;

namespace Vs.Backend.DbModels
{
    public sealed class Equipments : DbModel<Equipments>
    {
        private int AutoIncrement()
        {
            var rows = DbService.Instance.Db.Equipments;
            return (rows.Count == 0 ? 0 : rows.Max(i => i.EquipmentSeqId)) + 1;
        }

        protected override JsonObject Cache()
        {
            var results = new List<JsonObject>();
            return results;
        }

        public int Create(int equipmentId)
        {
            var rows = DbService.Instance.Db.Equipments;
            var row = new Structs.Equipment();
            row.EquipmentSeqId = this.AutoIncrement();
            row.EquipmentId = equipmentId;
            row.Level = 1;
            rows.Add(row);

            this.Dirty();
            return row.EquipmentSeqId;
        }

        public List<JsonObject> Read()
        {
            var cardEquipments = DbService.Instance.Db.CardEquipments;
            var user = DbService.Instance.Db.Users[0];
            var items = DbService.Instance.Db.Items;
            var cached = this.GetCached().ToArray();
            foreach (var row in cached)
            {
                var equipmentSeqId = (int)row["equipment_seq_id"];
                var cardEquipment = cardEquipments.Find(i => i.EquipmentSeqId == equipmentSeqId);
                row["card_seq_id"] = cardEquipment != null ? cardEquipment.CardSeqId : -1;

                row["is_short_coins"] = user.Coins < row["require_coins"];

                var itemId = GetRequireItemId(row["slot"]);
                var item = items.Find(i => i.ItemId == itemId);
                row["is_short_items"] = item == null || item.Quantity < row["require_item_quantity"];
                row["item_quantity"] = item != null ? item.Quantity : 0;
            }
            return cached;
        }

        public List<JsonObject> Update(int equipmentSeqId)
        {
            this.Dirty();
            return this.Read();
        }

        public List<JsonObject> Delete(int equipmentSeqId)
        {
            this.Dirty();
            return this.Read();
        }

        public JsonObject LevelUp(int equipmentSeqId)
        {
            var rows = DbService.Instance.Db.Equipments;
            var row = rows.Find(i => i.EquipmentSeqId == equipmentSeqId);

            var raws = MstDatas.Instance.Get("equipment_mst");
            var raw = raws.Find(i => i["equipment_id"] == row.EquipmentId);

            var coins = GetRequireCoins(row.Level, row.Rarity);
            var itemId = GetRequireItemId(raw["slot"]);
            var itemQuantity = GetRequireItemQuanity(row.Level, row.Rarity);

            DbModels.Users.Instance.TakeCoins(coins);
            DbModels.Items.Instance.Sub(itemId, itemQuantity);

            row.Level++;

            this.Dirty();
            var json = new JsonObject();
            json["cards"] = DbModels.Cards.Instance.Read();
            json["equipments"] = this.Read();
            return json;
        }

        public JsonObject LevelUpAll(int equipmentSeqId)
        {
            var rows = DbService.Instance.Db.Equipments;
            var row = rows.Find(i => i.EquipmentSeqId == equipmentSeqId);

            var raws = MstDatas.Instance.Get("equipment_mst");
            var raw = raws.Find(i => i["equipment_id"] == row.EquipmentId);

            while (true)
            {
                var coins = GetRequireCoins(row.Level, row.Rarity);
                var itemId = GetRequireItemId(raw["slot"]);
                var itemQuantity = GetRequireItemQuanity(row.Level, row.Rarity);

                var user = DbModels.Users.Instance.TakeCoins(coins);
                var items = DbModels.Items.Instance.Sub(itemId, itemQuantity);

                row.Level++;

                var nextCoins = GetRequireCoins(row.Level, row.Rarity);
                var nextQuantity = GetRequireItemQuanity(row.Level, row.Rarity);
                var item = items.Find(i => i["item_id"] == itemId);
                if (nextCoins > user["coins"] || nextQuantity > item["quantity"])
                {
                    break;
                }
            }

            this.Dirty();
            var json = new JsonObject();
            json["cards"] = DbModels.Cards.Instance.Read();
            json["equipments"] = this.Read();
            return json;
        }

        public JsonObject Merge(int equipmentSeqId, params int[] equipmentSeqIds)
        {
            var rows = DbService.Instance.Db.Equipments;

            foreach (var id in equipmentSeqIds)
            {
                rows.RemoveAll(i => i.EquipmentSeqId == id);
            }

            var row = rows.Find(i => i.EquipmentSeqId == equipmentSeqId);
            if (row.Rarity < 3)
            {
                row.Rarity++;
            }
            else
            {
                row.Rank++;
            }

            this.Dirty();
            var json = new JsonObject();
            json["cards"] = DbModels.Cards.Instance.Read();
            json["equipments"] = this.Read();
            return json;
        }

        private static int GetRequireCoins(int level, int rarity)
        {
            return level * (rarity + 1) * 100;
        }

        private static int GetRequireItemId(int slot)
        {
            switch (slot)
            {
                case 0: return 10101001;
                case 1: return 10101002;
                case 2: return 10101003;
                case 3: return 10101004;
                case 4: return 10101005;
                case 5: return 10101006;
                default: return -1;
            }
        }

        private static int GetRequireItemQuanity(int level, int rarity)
        {
            return level * (rarity + 1);
        }
    }
}
