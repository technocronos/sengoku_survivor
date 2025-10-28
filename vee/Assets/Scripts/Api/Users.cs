using System.Collections;
using System.Collections.Generic;
using MyGame;

namespace Vs.Api
{
    public static class Users
    {
        public static JsonObject Login()
        {
            var result = Backend.DbModels.Users.Instance.Login();
            DbService.Instance.Save();
            return result;
        }
    }
}
