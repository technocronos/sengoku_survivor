using System.Collections;
using System.Collections.Generic;
using MyGame;

namespace Vs.Api
{
    public static class Items
    {
        public static JsonObject Get()
        {
            return Backend.DbModels.Items.Instance.Read();
        }
    }
}
