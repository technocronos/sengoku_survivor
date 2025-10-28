using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs
{
    public sealed class DbService : SingletonMonoBehaviour<DbService>
    {
        private const string Key = "savedata";

        [SerializeField]
        private Backend.MstDatas.Pair[] spreadSheets;

        public bool Created { get { return PlayerPrefs.HasKey(DbService.Key); } }
        public Backend.Structs.Db Db { get; private set; }

        public IEnumerator Initialize(System.Action<float> onProgress)
        {
            yield return Backend.MstDatas.Instance.Initialize(this.spreadSheets, onProgress);
            if (!Created)
            {
                this.Create();
            }
            this.Load();
        }

        public void Create()
        {
            this.Db = new Backend.Structs.Db();
            this.Db.Initialize();
            this.Save();
        }

        public void Save()
        {
            var str = JsonUtility.ToJson(this.Db);
            PlayerPrefs.SetString(DbService.Key, str);
            PlayerPrefs.Save();
        }

        public void Load()
        {
            var str = PlayerPrefs.GetString(DbService.Key);
            this.Db = JsonUtility.FromJson<Backend.Structs.Db>(str);
        }
    }
}
