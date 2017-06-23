using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UniFramework.Net;


public class AssetBundleBuildInfo
{
    public bool IsInAssetDataBase
    {
        get
        {
            if (IsMenifest) return true;
            string[] names = AssetDatabase.GetAllAssetBundleNames();
            string t = System.Array.Find<string>(names, o => o.Equals(AssetBundleName));
            return t != null;
        }
    }

    public string AssetBundleName
    {
        get
        {
            return EnvUtil.MakeRelative(fileInfo.FullName, AssetBundleBuilder.GetAssetBundlePath(buildTarget, outputPath)).Replace(".manifest", "");
        }
    }

    public string FullPath
    {
        get
        {
            return fileInfo.FullName;
        }
    }

    public bool IsMenifest
    {
        get
        {
            return AssetBundleBuilder.GetPlatformForAssetBundles(buildTarget).Equals(AssetBundleName);
        }
    }

    public bool IsPublish
    {
        get
        {
            if (IsMenifest)
            {
                return true;
            }
            if (!IsInAssetDataBase)
            {
                return false;
            }
            return isPublish;
        }
        set
        {
            isPublish = value;
        }
    }

    public bool IsChanged
    {
        get;
        set;
    }
    public bool IsNew
    {
        get;
        set;
    }

    private bool isPublish = true;
    private System.IO.FileInfo fileInfo;
    private string outputPath;
    private BuildTarget buildTarget;

    public AssetBundleBuildInfo(System.IO.FileInfo info, BuildTarget buildTarget, string outputPath)
    {
        this.fileInfo = info;
        this.IsChanged = false;
        this.isPublish = true;
        this.IsNew = true;
        this.outputPath = outputPath;
        this.buildTarget = buildTarget;
    }

    public void DeleteFile()
    {
        System.IO.File.Delete(System.IO.Path.Combine(AssetBundleBuilder.GetAssetBundlePath(buildTarget, outputPath), this.AssetBundleName));
        System.IO.File.Delete(fileInfo.FullName);
    }

}

public class AssetBundleBuilderWindow : EditorWindow
{
    enum WindowType
    {
        Menu,
        List,
    }

    private Vector2 listscrollPosition;
    private Vector2 menuscrollPosition;

    private Rect menuRect;
    private Rect listRect;

    private List<AssetBundleBuildInfo> assetBundleList;

    [MenuItem("Build/Window/AssetBundleBuilder")]
    public static AssetBundleBuilderWindow ShowWindow()
    {
        var w = GetWindow(typeof(AssetBundleBuilderWindow));
        w.Show();
        return w as AssetBundleBuilderWindow;
    }

    public void OnDestroy()
    {
    }

    public void OnEnable()
    {
        CachedFileInfo();
    }

    public void OnGUI()
    {
        menuRect = new Rect(0.0f, 0.0f, 220f, this.position.height);
        listRect = new Rect(menuRect.x + menuRect.width, 0.0f, this.position.width - menuRect.width, this.position.height);

        BeginWindows();
        GUILayout.Window((int)WindowType.Menu, menuRect, OnGUIMenu, "Menu");
        GUILayout.Window((int)WindowType.List, listRect, OnGUIList, "AssetBundles");
        EndWindows();
    }

    public void OnGUIList(int window)
    {

        if (assetBundleList == null)
        {
            EditorGUILayout.HelpBox("There is no directory", MessageType.Info);
        }
        else
        {

            if (assetBundleList.Count == 0)
            {
                EditorGUILayout.HelpBox("There is no files in directory", MessageType.Info);
            }
            else
            {
                listscrollPosition = EditorGUILayout.BeginScrollView(listscrollPosition, EditorStyles.helpBox);
                //					EditorGUILayout.BeginVertical (EditorStyles.helpBox);
                foreach (var ab in assetBundleList)
                {
                    OnGUIFileInfo(ab);
                }
                //					EditorGUILayout.EndVertical ();
                EditorGUILayout.EndScrollView();
            }
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();
    }

    private void PublishToStreaming()
    {
        AssetBundleBuilder.CopyToStreamingAssetBundle(EditorUserBuildSettings.activeBuildTarget, AssetBundleBuildSetting.RelativeOutputPath);
    }
    private void PublishToShellScript()
    {
        AssetBundleBuilder.DeleteStreamingAssetBundle();
        AssetBundleBuilder.ExecuteShellScript(AssetBundleBuildSetting.ShellScript,
            "AssetBundlePath",
            AssetBundleBuilder.GetAssetBundlePath(EditorUserBuildSettings.activeBuildTarget, AssetBundleBuildSetting.RelativeOutputPath)
        );
    }

    private void CachedFileInfo()
    {
        var di = new System.IO.DirectoryInfo(AssetBundleBuildSetting.RelativeOutputPath);
        if (!di.Exists)
            return;
        var files = di.GetFiles("*.manifest", System.IO.SearchOption.AllDirectories);
        assetBundleList = new List<AssetBundleBuildInfo>();
        foreach (var fi in files)
        {
            var info = new AssetBundleBuildInfo(fi, EditorUserBuildSettings.activeBuildTarget, AssetBundleBuildSetting.AbsoluteOutputPath);
            info.IsNew = false;
            assetBundleList.Add(info);
        }
    }

    private void OnGUIFileInfo(AssetBundleBuildInfo info)
    {
        EditorGUILayout.BeginHorizontal();
        if (!info.IsInAssetDataBase)
        {
            GUI.color = Color.red;
            GUI.enabled = false;
        }
        if (info.IsMenifest)
        {
            GUI.color = Color.green;
            GUI.enabled = false;
        }
        if (info.IsNew)
        {
            GUI.color = Color.yellow;
        }
        if (info.IsNew)
        {
            GUI.color = Color.cyan;
        }


        info.IsPublish = EditorGUILayout.ToggleLeft(info.AssetBundleName, info.IsPublish, EditorStyles.boldLabel, GUILayout.Width(150f));
        string relativePath = EnvUtil.MakeRelative(info.FullPath, AssetBundleBuildSetting.AbsoluteOutputPath);
        EditorGUILayout.LabelField(relativePath);
        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;
        GUI.enabled = true;

    }


    public void OnGUIMenu(int window)
    {
        menuscrollPosition = EditorGUILayout.BeginScrollView(menuscrollPosition);
        EditorGUILayout.LabelField("Build Target", CreateGUIStyle(11, new Color(0.8f, 0.8f, 0.8f, 1.0f), FontStyle.Bold));
        EditorGUILayout.LabelField(EditorUserBuildSettings.activeBuildTarget.ToString());
        EditorGUILayout.LabelField("Output Path", CreateGUIStyle(11, new Color(0.8f, 0.8f, 0.8f, 1.0f), FontStyle.Bold));

        GUI.enabled = false;
        EditorGUILayout.TextField(AssetBundleBuildSetting.RelativeOutputPath);
        GUI.enabled = true;
        if (GUILayout.Button("Find"))
        {

            string result = EditorUtility.OpenFolderPanel("Select Output Path", AssetBundleBuildSetting.RelativeOutputPath, "");
            if (!string.IsNullOrEmpty(result))
            {
                string relativePath = EnvUtil.MakeRelative(result, System.Environment.CurrentDirectory);
                AssetBundleBuildSetting.RelativeOutputPath = relativePath;
                CachedFileInfo();
            }
        }

        EditorGUILayout.Separator();
        GUI.color = Color.green;
        if (GUILayout.Button("Build"))
        {
            CachedFileInfo();
            List<string> changedList = AssetBundleBuilder.BuildAssetBundle(EditorUserBuildSettings.activeBuildTarget, AssetBundleBuildSetting.AbsoluteOutputPath);
            foreach (var c in changedList)
            {
                var ret = this.assetBundleList.Find(o => o.AssetBundleName.Equals(c));
                if (ret != null)
                {
                    ret.IsChanged = true;
                }
                else
                {
                    var path = AssetBundleBuilder.GetAssetBundlePath(EditorUserBuildSettings.activeBuildTarget, AssetBundleBuildSetting.AbsoluteOutputPath);
                    var fileInfo = new System.IO.FileInfo(path);
                    this.assetBundleList.Add(new AssetBundleBuildInfo(fileInfo, EditorUserBuildSettings.activeBuildTarget, AssetBundleBuildSetting.AbsoluteOutputPath));
                }
            }
        }
        GUI.color = Color.white;
        EditorGUILayout.LabelField("PublishSetting", CreateGUIStyle(11, new Color(0.8f, 0.8f, 0.8f, 1.0f), FontStyle.Bold));
        AssetBundleBuildSetting.PublishState = (AssetBundleBuildSetting.PublishStateEnum)EditorGUILayout.EnumPopup(AssetBundleBuildSetting.PublishState);
        if (AssetBundleBuildSetting.PublishState == AssetBundleBuildSetting.PublishStateEnum.ShellScript)
        {
            EditorGUILayout.LabelField("Shellscript");
            AssetBundleBuildSetting.ShellScript = EditorGUILayout.TextArea(AssetBundleBuildSetting.ShellScript, GUILayout.Height(100));
            EditorGUILayout.HelpBox("AssetBundle OutputPath is set in EnvironmentVariables 'AssetBundlePath' " + System.Environment.NewLine + " ex) $AssetBundlePath  ,%AssetBundlePath%", MessageType.Info);
        }
        GUI.color = Color.green;
        if (GUILayout.Button("Publish"))
        {
            foreach (var assetBundle in this.assetBundleList)
            {
                if (!assetBundle.IsInAssetDataBase)
                {
                    assetBundle.DeleteFile();
                }
            }

            if (AssetBundleBuildSetting.PublishState == AssetBundleBuildSetting.PublishStateEnum.ShellScript)
            {
                PublishToShellScript();
            }
            else if (AssetBundleBuildSetting.PublishState == AssetBundleBuildSetting.PublishStateEnum.Streaming)
            {
                PublishToStreaming();

            }
            CachedFileInfo();
        }
        GUI.color = Color.white;
        EditorGUILayout.EndScrollView();

    }

    public static GUIStyle CreateGUIStyle(int fontSize, Color color, FontStyle eFont)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = color;
        style.fontSize = fontSize;
        style.fontStyle = eFont;
        return style;
    }
}


