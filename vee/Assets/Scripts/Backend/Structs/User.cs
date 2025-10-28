using System.Collections;
using System.Collections.Generic;

namespace Vs.Backend.Structs
{
    [System.Serializable]
    public sealed class User
    {
        public int UserId;
        public int Stamina;
        public int StaminaMax;
        public int Coins;
        public int Gems;
    }
}
