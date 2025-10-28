using System.Collections;
using System.Collections.Generic;
using MyGame;

namespace Vs.Backend.Structs
{
    [System.Serializable]
    public sealed class Db
    {
        public List<Structs.User> Users;
        public List<Structs.Card> Cards;
        public List<Structs.Item> Items;
        public List<Structs.Equipment> Equipments;
        public List<Structs.CardEquipment> CardEquipments;

        public void Initialize()
        {
            this.Cards = new List<Structs.Card>();
            this.Users = new List<Structs.User>();
            this.Items = new List<Structs.Item>();
            this.Equipments = new List<Structs.Equipment>();
            this.CardEquipments = new List<Structs.CardEquipment>();
        }
    }
}
