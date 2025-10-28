using System.Collections;
using System.Collections.Generic;
using MyGame;

namespace Vs.Api
{
    public static class Stats
    {
        public static JsonObject Get()
        {
            return Backend.DbModels.Stats.Instance.Read();
        }
    }
}
