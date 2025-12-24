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
            // Resourcesフォルダから読み込む（拡張子を削除）
            var spath = System.IO.Path.ChangeExtension(path, null);
            var asset = Resources.Load<TextAsset>(spath);
            if (asset != null)
            {
                return asset.text;
            }
            
            // フォールバック: StreamingAssetsから読み込む（開発用）
            var fallbackPath = System.IO.Path.Combine(Application.streamingAssetsPath, path);
            if (System.IO.File.Exists(fallbackPath))
            {
                return System.IO.File.ReadAllText(fallbackPath);
            }
            
            return null;
        }
    }
}
