using System.Collections;
using System.Collections.Generic;

namespace Vs.Backend.Structs
{
    [System.Serializable]
    public sealed class Equipment
    {
        public int EquipmentSeqId;
        public int EquipmentId;
        public int Rarity;
        public int Level;
        public int Rank;
    }
}
