using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs
{
    public sealed class AssetService : SingletonMonoBehaviour<AssetService>
    {
        public string LoadText(string path)
        {
// #if DEVELOPMENT_BUILD
            var spath = System.IO.Path.Combine(Application.streamingAssetsPath, path);
            return System.IO.File.ReadAllText(spath);
// #else
//             var spath = System.IO.Path.ChangeExtension(path, null);
//             var asset = Resources.Load<TextAsset>(spath);
//             return asset != null ? asset.text : null;
// #endif
        }
    }
}
