using System.Collections;
using System.Collections.Generic;
using MyGame;

namespace Vs.Api
{
    public static class Equipments
    {
        public static JsonObject Get()
        {
            return Backend.DbModels.Equipments.Instance.Read();
        }

        public static JsonObject LevelUp(int equipmentSeqId)
        {
            var result = Backend.DbModels.Equipments.Instance.LevelUp(equipmentSeqId);
            DbService.Instance.Save();
            return result;
        }

        public static JsonObject LevelUpAll(int equipmentSeqId)
        {
            var result = Backend.DbModels.Equipments.Instance.LevelUpAll(equipmentSeqId);
            DbService.Instance.Save();
            return result;
        }

        public static JsonObject Merge(int equipmentSeqId, params int[] equipmentSeqIds)
        {
            var result = Backend.DbModels.Equipments.Instance.Merge(equipmentSeqId, equipmentSeqIds);
            DbService.Instance.Save();
            return result;
        }
    }
}
