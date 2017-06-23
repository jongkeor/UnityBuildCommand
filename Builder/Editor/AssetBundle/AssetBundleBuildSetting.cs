using System;
using UnityEditor;


public static class AssetBundleBuildSetting
{
    private const string PREF_OUTPUTPATH = "AssetBundleOutputPath";
    private const string PREF_PUBLISH_STATE = "AssetBundlePublishState";
    private const string PREF_SHELLSCRIPT = "AssetBundleShellScript";
    private const string DEFAULT_OUTPUT_PATH = "AssetBundles";


    public enum PublishStateEnum
    {
        Streaming,
        ShellScript,
    }

    public static string RelativeOutputPath
    {
        get
        {
            return EditorPrefs.GetString(PREF_OUTPUTPATH, DEFAULT_OUTPUT_PATH);
        }
        set
        {
            EditorPrefs.SetString(PREF_OUTPUTPATH, value);
        }
    }
    public static string AbsoluteOutputPath
    {
        get
        {
            return EnvUtil.MakeAbsolute(System.Environment.CurrentDirectory, AssetBundleBuildSetting.RelativeOutputPath);
        }
    }

    public static PublishStateEnum PublishState
    {
        get
        {
            try
            {
                return (PublishStateEnum)System.Enum.Parse(typeof(PublishStateEnum), EditorPrefs.GetString(PREF_PUBLISH_STATE, PublishStateEnum.Streaming.ToString()));
            }
            catch
            {
                return PublishStateEnum.Streaming;
            }
        }
        set
        {
            EditorPrefs.SetString(PREF_PUBLISH_STATE, value.ToString());
        }
    }

    public static string ShellScript
    {
        get
        {
            return EditorPrefs.GetString(PREF_SHELLSCRIPT);
        }
        set
        {
            EditorPrefs.SetString(PREF_SHELLSCRIPT, value);
        }
    }


}

