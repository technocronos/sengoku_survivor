using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MyGame;

namespace Vs.Backend
{
    public sealed class MstDatas
    {
        [System.Serializable]
        public struct Pair
        {
            public string Key;
            public string Value;
        }

        private static MstDatas instance;
        public static MstDatas Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MstDatas();
                }
                return instance;
            }
        }

        private Dictionary<string, List<JsonObject>> msts = new Dictionary<string, List<JsonObject>>();

        public IEnumerator Initialize(Pair[] spreadSheets, System.Action<float> onProgress)
        {
            for (var i = 0; i < spreadSheets.Length; i++)
            {
                var sheet = spreadSheets[i];
                // yield return this.Method(sheet.Key, sheet.Value);
                yield return null;
                onProgress.Invoke((float)i / (spreadSheets.Length - 1));
            }
        }

        private IEnumerator Method(string mstName, string sheetId)
        {
            var request = UnityWebRequest.Get($"https://docs.google.com/spreadsheets/d/{sheetId}/gviz/tq?tqx=out:csv&sheet={mstName}");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"error: {mstName}");
                Debug.Log(request.error);
                yield break;
            }
            var list = this.Read(request.downloadHandler.text);
            this.msts.Add(mstName, list);
        }

        public List<JsonObject> Get(string mstName)
        {
            if (!this.msts.ContainsKey(mstName))
            {
                var asset = AssetService.Instance.LoadText($"Csv/{mstName}.csv");
                UnityEngine.Assertions.Assert.IsNotNull(asset, mstName);
                var list = this.Read(asset);
                this.msts.Add(mstName, list);
            }
            return this.msts[mstName];
        }

        private List<JsonObject> Read(string csv)
        {
            var reader = new CSVReader();
            var rows = reader.Read(csv);
            var list = new List<JsonObject>();
            foreach (var row in rows)
            {
                var dic = new JsonObject();
                foreach (var i in row)
                {
                    var key = i.Key.Replace("\"", "");
                    dic[key] = new JsonObject(i.Value);
                }
                list.Add(dic);
            }
            return list;
        }
    }
}
