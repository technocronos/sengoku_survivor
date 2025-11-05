using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Newtonsoft.Json;

namespace MyGame
{
    using JsonDictionary = Dictionary<string, JsonObject>;
    using JsonArray = List<JsonObject>;

    public sealed class JsonObject
    {
        private object value;

        public JsonObject()
        {
            this.value = new JsonDictionary();
        }

        public JsonObject(object value)
        {
            this.value = value;
        }

        public static implicit operator JsonObject(string i)
        {
            return new JsonObject(i);
        }

        public static implicit operator JsonObject(int i)
        {
            return new JsonObject(i);
        }

        public static implicit operator JsonObject(bool i)
        {
            return new JsonObject(i);
        }

        public static implicit operator JsonObject(JsonDictionary i)
        {
            return new JsonObject(i);
        }

        public static implicit operator JsonObject(JsonArray i)
        {
            return new JsonObject(i);
        }

        public static implicit operator string(JsonObject i)
        {
            return i.value.ToString();
        }

        public static implicit operator int(JsonObject i)
        {
            return i.ToInt();
        }

        public static implicit operator float(JsonObject i)
        {
            return i.ToFloat();
        }

        public static implicit operator bool(JsonObject i)
        {
            return i.ToBool();
        }

        public static implicit operator JsonDictionary(JsonObject i)
        {
            return i.value as JsonDictionary;
        }

        public static implicit operator JsonArray(JsonObject i)
        {
            return i.value as JsonArray;
        }

        public override string ToString()
        {
            return this.value.ToString();
        }

        public int ToInt()
        {
            return int.Parse(this.ToString());
        }

        public float ToFloat()
        {
            return float.Parse(this.ToString());
        }

        public bool ToBool()
        {
            var strValue = this.ToString();
            var result = false;
            var valid = bool.TryParse(strValue, out result);
            if (valid)
            {
                return result;
            }
            return strValue == "1";
        }

        public JsonDictionary ToDictionary()
        {
            return value as JsonDictionary;
        }

        public JsonArray ToArray()
        {
            return value as JsonArray;
        }

        public JsonObject this[string key]
        {
            get
            {
                var dic = this.ToDictionary();
                Assert.IsNotNull(dic, $"not dictionary.\n{StackTraceUtility.ExtractStackTrace()}");
                Assert.IsTrue(dic.ContainsKey(key), $"{key} is not contained.\n{StackTraceUtility.ExtractStackTrace()}");
                return dic[key];
            }
            set
            {
                var dic = this.ToDictionary();
                if (!dic.ContainsKey(key))
                {
                    dic.Add(key, value);
                    return;
                }
                dic[key] = value;
            }
        }

        public bool ContainsKey(string key)
        {
            return this.ToDictionary().ContainsKey(key);
        }

        public JsonObject GetValue(string key, JsonObject defaultValue)
        {
            if (!this.ContainsKey(key))
            {
                return defaultValue;
            }
            return this[key];
        }


        public string ToJson()
        {
            return JsonConvert.SerializeObject(this.value);
        }

        public static JsonObject FromJson(string json)
        {
            var obj = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            return new JsonObject(obj);
        }

        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
