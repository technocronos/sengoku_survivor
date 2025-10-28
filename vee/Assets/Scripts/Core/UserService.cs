using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs
{
    public sealed class UserService : SingletonMonoBehaviour<UserService>
    {
        public event System.Action Updated = () => { };

        public int Stamina = 0;
        public int StaminaMax = 0;
        public int Coins = 0;
        public int Gems = 0;

        public void Set(JsonObject raw)
        {
            if (raw.ContainsKey("user"))
            {
                var user = raw["user"];
                this.Stamina = user["stamina"];
                this.StaminaMax = user["stamina_max"];
                this.Coins = user["coins"];
                this.Gems = user["gems"];
            }
            this.Updated.Invoke();
        }
    }
}
