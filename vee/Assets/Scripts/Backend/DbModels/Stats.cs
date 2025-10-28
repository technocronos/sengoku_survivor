using System.Collections;
using System.Collections.Generic;
using MyGame;

namespace Vs.Backend.DbModels
{
    public sealed class Stats : DbModel<Stats>
    {
        public JsonObject Read()
        {
            var user = DbModels.Users.Instance.Read(0);

            var json = new JsonObject();
            json["user"] = user;
            return json;
        }
    }
}
