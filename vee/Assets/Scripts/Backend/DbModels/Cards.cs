using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyGame;

namespace Vs.Backend.DbModels
{
    public sealed class Cards : DbModel<Cards>
    {
        private int AutoIncrement()
        {
            var rows = DbService.Instance.Db.Cards;
            return (rows.Count == 0 ? 0 : rows.Max(i => i.CardSeqId)) + 1;
        }

        protected override JsonObject Cache()
        {
            var results = new List<JsonObject>();
            var rows = DbService.Instance.Db.Cards;
            foreach (var row in rows)
            {
                var json = new JsonObject();
                json["card_seq_id"] = row.CardSeqId;
                json["card_id"] = row.CardId;
                json["base_atk"] = 100;
                json["base_hp"] = 100;
                results.Add(json);
            }
            return results;
        }

        private JsonObject CalcStats(JsonObject raw)
        {
            var atk = (int)raw["base_atk"];
            var hp = (int)raw["base_hp"];

            var equipments = DbModels.Equipments.Instance.Read();
            foreach (var equipment in equipments)
            {
                atk += equipment["atk"];
                hp += equipment["hp"];
            }
            raw["atk"] = atk;
            raw["hp"] = hp;
            return raw;
        }

        public int Create(int cardId)
        {
            var rows = DbService.Instance.Db.Cards;
            var row = new Structs.Card();
            row.CardSeqId = this.AutoIncrement();
            row.CardId = cardId;
            rows.Add(row);

            this.Dirty();
            return row.CardSeqId;
        }

        public List<JsonObject> Read()
        {
            var cached = this.GetCached().ToArray();
            foreach (var raw in cached)
            {
                this.CalcStats(raw);
            }
            return cached;
        }

        public JsonObject Read(int cardSeqId)
        {
            var cached = this.Read();
            return cached.Find(i => i["card_seq_id"] == cardSeqId);
        }

        public List<JsonObject> Update(int cardSeqId)
        {
            this.Dirty();
            return this.Read();
        }

        public List<JsonObject> Delete(int cardSeqId)
        {
            this.Dirty();
            return this.Read();
        }

        public JsonObject Equip(int cardSeqId, int slot, int equipmentSeqId)
        {
            var rows = DbService.Instance.Db.CardEquipments;
            var row = rows.Find(i => i.CardSeqId == cardSeqId && i.Slot == slot);
            if (row == null)
            {
                row = new Structs.CardEquipment();
                row.CardSeqId = cardSeqId;
                row.Slot = slot;
                rows.Add(row);
            }
            row.EquipmentSeqId = equipmentSeqId;

            this.Dirty();
            var json = new JsonObject();
            json["card"] = this.Read(cardSeqId);
            json["equipments"] = DbModels.Equipments.Instance.Read();
            return json;
        }

        public JsonObject UnEquip(int cardSeqId)
        {
            var rows = DbService.Instance.Db.CardEquipments;
            rows.RemoveAll(i => i.CardSeqId == cardSeqId);

            this.Dirty();
            var json = new JsonObject();
            json["card"] = this.Read(cardSeqId);
            json["equipments"] = DbModels.Equipments.Instance.Read();
            return json;
        }

        public JsonObject UnEquip(int cardSeqId, int slot)
        {
            var rows = DbService.Instance.Db.CardEquipments;
            rows.RemoveAll(i => i.CardSeqId == cardSeqId && i.Slot == slot);

            this.Dirty();
            var json = new JsonObject();
            json["card"] = this.Read(cardSeqId);
            json["equipments"] = DbModels.Equipments.Instance.Read();
            return json;
        }
    }
}
