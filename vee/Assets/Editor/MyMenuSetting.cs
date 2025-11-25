using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class MyMenuSetting : EditorWindow
{
    [MenuItem("MyMenu/削除/PlayerPrefsキャッシュ全削除")]
    static void PlayerPrefsDelete()
    {
        PlayerPrefs.DeleteAll();
        Delete(Application.persistentDataPath);
        Delete(Application.temporaryCachePath);
    }

    //Assetsディレクトリ以下にあるTestディレクトリを削除
    /// <summary>
    /// 指定したディレクトリとその中身を全て削除する
    /// </summary>
    public static void Delete(string targetDirectoryPath)
    {
        if (!Directory.Exists(targetDirectoryPath))
        {
            return;
        }

        Debug.Log(targetDirectoryPath + "フォルダの中を空にします");
        //ディレクトリ以外の全ファイルを削除
        string[] filePaths = Directory.GetFiles(targetDirectoryPath);
        foreach (string filePath in filePaths)
        {
            File.SetAttributes(filePath, FileAttributes.Normal);
            File.Delete(filePath);
        }

        //ディレクトリの中のディレクトリも再帰的に削除
        string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
        foreach (string directoryPath in directoryPaths)
        {
            Delete(directoryPath);
        }

        //中が空になったらディレクトリ自身も削除
        Directory.Delete(targetDirectoryPath, false);
    }

    /// <summary>
    /// メイン画面からPlayします.
    /// </summary>
    [MenuItem("MyMenu/Scene/MainPlay")]
    static void PlayTitle()
    {
        Change("Assets/Embed/Bootstrap.unity");
        EditorApplication.isPlaying = true;
    }

    static void Change(string scene)
    {
        bool isCancel = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        if (!isCancel) return;

        EditorSceneManager.OpenScene(scene);
    }

    [MenuItem("MyMenu/Switch to Release")]
    static void SwitchToRelease()
    {
        string[] defines;
        List<string> definesNew = new List<string>();
        PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone, out defines);
        for (int i = 0; i <  defines.Length; i++)
        {
            if (defines[i] == "DEBUG") continue;
            definesNew.Add(defines[i]);
        }
        definesNew.Add("RELEASE");
        PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone, definesNew.ToArray());
    }

    [MenuItem("MyMenu/Switch to Debug")]
    static void SwitchToDebug()
    {
        string[] defines;
        List<string> definesNew = new List<string>();
        PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone, out defines);
        for (int i = 0; i < defines.Length; i++)
        {
            if (defines[i] == "RELEASE") continue;
            definesNew.Add(defines[i]);
        }
        definesNew.Add("DEBUG");
        PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone, definesNew.ToArray());
    }
}
