using System.Collections;
using System.Collections.Generic;
using MyGame;

namespace Vs.Backend
{
    public class DbModel<T> where T : new()
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }

        private JsonObject cached;
        private bool isDirty = true;

        public void Dirty()
        {
            this.isDirty = true;
        }

        protected JsonObject GetCached()
        {
            if (this.isDirty)
            {
                this.cached = this.Cache();
                this.isDirty = false;
            }
            return this.cached;
        }

        protected virtual JsonObject Cache()
        {
            return null;
        }
    }
}
