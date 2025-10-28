using System.Collections;
using System.Collections.Generic;
using MyGame;

namespace Vs.Backend.DbModels
{
    public sealed class Users : DbModel<Users>
    {
        protected override JsonObject Cache()
        {
            var results = new List<JsonObject>();
            var rows = DbService.Instance.Db.Users;
            foreach (var row in rows)
            {
                var json = new JsonObject();
                json["user_id"] = row.UserId;
                json["stamina"] = row.Stamina;
                json["stamina_max"] = row.StaminaMax;
                json["coins"] = row.Coins;
                json["gems"] = row.Gems;
                results.Add(json);
            }
            return results;
        }

        public JsonObject Create()
        {
            var userId = 0;

            var rows = DbService.Instance.Db.Users;
            var row = new Structs.User();
            row.UserId = userId;
            row.Stamina = 100;
            row.StaminaMax = 100;
            row.Coins = 10000;
            row.Gems = 10000;
            rows.Add(row);

            DbModels.Cards.Instance.Create(0);

            return userId;
        }

        public List<JsonObject> Read()
        {
            return this.GetCached();
        }

        public JsonObject Read(int userId)
        {
            var cached = this.Read();
            return cached.Find(i => i["user_id"] == userId);
        }

        public JsonObject Login()
        {
            var rows = DbService.Instance.Db.Users;
            if (rows.Count == 0)
            {
                this.Create();
            }
            this.Dirty();
            return this.Read(0);
        }

        public JsonObject AddStamina(int value)
        {
            var row = DbService.Instance.Db.Users[0];
            row.Stamina += value;

            this.Dirty();
            return this.Read(0);
        }

        public JsonObject TakeStamina(int value)
        {
            var row = DbService.Instance.Db.Users[0];
            row.Stamina -= value;

            this.Dirty();
            return this.Read(0);
        }

        public JsonObject AddCoins(int value)
        {
            var row = DbService.Instance.Db.Users[0];
            row.Coins += value;

            this.Dirty();
            return this.Read(0);
        }

        public JsonObject TakeCoins(int value)
        {
            var row = DbService.Instance.Db.Users[0];
            row.Coins -= value;

            this.Dirty();
            return this.Read(0);
        }

        public JsonObject AddGems(int value)
        {
            var row = DbService.Instance.Db.Users[0];
            row.Gems += value;

            this.Dirty();
            return this.Read(0);
        }

        public JsonObject TakeGems(int value)
        {
            var row = DbService.Instance.Db.Users[0];
            row.Gems -= value;

            this.Dirty();
            return this.Read(0);
        }
    }
}
