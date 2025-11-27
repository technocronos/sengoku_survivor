using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

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
    static void Release()
    {
        var symbols = GetSymbols();

        Predicate<string> predicate = FindStr;
        symbols.RemoveAll(predicate);

        symbols.Add("RELEASE");
        symbols.Remove("DEVELOP");

        SetSymbols(symbols);
    }

    [MenuItem("MyMenu/Switch to Debug")]
    static void Develop()
    {
        var symbols = GetSymbols();

        Predicate<string> predicate = FindStr;
        symbols.RemoveAll(predicate);

        symbols.Add("DEVELOP");
        symbols.Remove("RELEASE");

        SetSymbols(symbols);
    }

    private static bool FindStr(string str)
    {
        if (str == "DEVELOP" || str == "RELEASE")
            return true;
        else
            return false;
    }

    // 設定されているシンボル定義を取得する
    static List<string> GetSymbols()
    {
        return PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';').ToList();
    }

    // シンボル定義をセットする
    static void SetSymbols(List<string> symbols)
    {
        var symbolStr = string.Empty;
        symbols.ForEach(s => symbolStr += s + ";");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbolStr);
    }
}
