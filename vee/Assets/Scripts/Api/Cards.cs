using System.Collections;
using System.Collections.Generic;
using MyGame;

namespace Vs.Api
{
    public static class Cards
    {
        public static JsonObject Get()
        {
            return Backend.DbModels.Cards.Instance.Read();
        }

        public static JsonObject Get(int cardSeqId)
        {
            return Backend.DbModels.Cards.Instance.Read(cardSeqId);
        }

        public static JsonObject Equip(int cardSeqId, int slot, int equipmentSeqId)
        {
            var result = Backend.DbModels.Cards.Instance.Equip(cardSeqId, slot, equipmentSeqId);
            DbService.Instance.Save();
            return result;
        }

        public static JsonObject UnEquip(int cardSeqId)
        {
            var result = Backend.DbModels.Cards.Instance.UnEquip(cardSeqId);
            DbService.Instance.Save();
            return result;
        }

        public static JsonObject UnEquip(int cardSeqId, int slot)
        {
            var result = Backend.DbModels.Cards.Instance.UnEquip(cardSeqId, slot);
            DbService.Instance.Save();
            return result;
        }
    }
}
