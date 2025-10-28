using System.Collections;
using System.Collections.Generic;
using MyGame;

namespace Vs.Api
{
    public static class Goods
    {
        public static JsonObject Get()
        {
            return Backend.DbModels.Goods.Instance.Read();
        }

        public static JsonObject Get(int shopId)
        {
            return Backend.DbModels.Goods.Instance.Read(shopId);
        }

        public static JsonObject Buy(int shopId, int goodsId)
        {
            var result = Backend.DbModels.Goods.Instance.Buy(shopId, goodsId);
            DbService.Instance.Save();
            return result;
        }
    }
}
