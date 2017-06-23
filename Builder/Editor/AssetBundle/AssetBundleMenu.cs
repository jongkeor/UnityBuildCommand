using System;
using UnityEditor;


public static class AssetBundleMenu
{
    private const string ENV_ASSETBUNDLE_OUTPUT = "UNI_ASSETBUNDLE_OUTPUT";


    [MenuItem("Build/CI/Build IOS AssetBundle")]
    static void BuildIOSAssetBundle()
    {
        AssetBundleBuildSetting.RelativeOutputPath = EnvUtil.GetEnvValue(ENV_ASSETBUNDLE_OUTPUT, AssetBundleBuildSetting.RelativeOutputPath);
        AssetBundleBuilder.BuildAssetBundle(BuildTarget.iOS, AssetBundleBuildSetting.RelativeOutputPath);
    }

    [MenuItem("Build/CI/Build Android AssetBundle")]
    static void BuildAndroidAssetBundle()
    {
        AssetBundleBuildSetting.RelativeOutputPath = EnvUtil.GetEnvValue(ENV_ASSETBUNDLE_OUTPUT, AssetBundleBuildSetting.RelativeOutputPath);
        AssetBundleBuilder.BuildAssetBundle(BuildTarget.Android, AssetBundleBuildSetting.RelativeOutputPath);
    }
}




