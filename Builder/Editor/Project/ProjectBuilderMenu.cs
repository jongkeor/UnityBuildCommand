using System;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;


public static class ProjectBuilderMenu
{
    private const string ENV_OUTPUT_DIR = "UNI_OUTPUT_DIR";
    private const string ENV_OUTPUT_NAME = "UNI_OUTPUT_NAME";
    private const string ENV_DEFINE_SYMBOLS = "UNI_DEFINE_SYMBOLS";
    private const string ENV_BUNDLE_IDENTIFIER = "UNI_BUNDLE_IDENTIFIER";
    private const string ENV_BUILD_NUMBER = "UNI_BUILD_NUMBER";
    private const string ENV_BUILD_OPT_RELEASE = "UNI_BUILD_OPT_RELEASE";
    private const string ENV_KEYSTORE_PATH = "ANDROID_KEYSTORE_PATH";
    private const string ENV_KEYSTORE_PASS = "ANDROID_KEYSTORE_PASS";
    private const string ENV_KEY_ALIAS_NAME = "ANDROID_KEY_ALIAS_NAME";
    private const string ENV_KEY_ALIAS_PASS = "ANDROID_KEY_ALIAS_PASS";
    private const string Default_OutputPath = "build";

    [MenuItem("Build/CI/Build IOS")]
    public static void PerformiOSBuild()
    {

        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        string originalVersion = PlayerSettings.iOS.buildNumber;
        PlayerSettings.iOS.buildNumber = EnvUtil.GetEnvValue(ENV_BUILD_NUMBER, originalVersion);
        Build(BuildTargetGroup.iOS, BuildTarget.iOS);
        PlayerSettings.iOS.buildNumber = originalVersion;

    }

    [MenuItem("Build/CI/Build Android")]
    public static void PerformAndroidBuild()
    {

        PlayerSettings.Android.keystoreName = EnvUtil.GetEnvValue(ENV_KEYSTORE_PATH);
        PlayerSettings.Android.keystorePass = EnvUtil.GetEnvValue(ENV_KEYSTORE_PASS);
        PlayerSettings.Android.keyaliasName = EnvUtil.GetEnvValue(ENV_KEY_ALIAS_NAME);
        PlayerSettings.Android.keyaliasPass = EnvUtil.GetEnvValue(ENV_KEY_ALIAS_PASS);


        int originalVersion = PlayerSettings.Android.bundleVersionCode;
        PlayerSettings.Android.bundleVersionCode = int.Parse(EnvUtil.GetEnvValue(ENV_BUILD_NUMBER, originalVersion.ToString()));

        Build(BuildTargetGroup.Android, BuildTarget.Android);

        PlayerSettings.Android.bundleVersionCode = originalVersion;
    }

    private static void Build(BuildTargetGroup group, BuildTarget target)
    {
        string buildPath = EnvUtil.GetEnvValue(ENV_OUTPUT_DIR, Default_OutputPath);
        Directory.CreateDirectory(buildPath);

        string originalId = PlayerSettings.applicationIdentifier;
        PlayerSettings.applicationIdentifier = EnvUtil.GetEnvValue(ENV_BUNDLE_IDENTIFIER, originalId);

        string originalD = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, originalD + ";" + EnvUtil.GetEnvValue(ENV_DEFINE_SYMBOLS));

        string release = EnvUtil.GetEnvValue(ENV_BUILD_OPT_RELEASE, "1");
        BuildOptions opt;
        if (release.Equals("1"))
        {
            opt = BuildOptions.None;
        }
        else
        {
            opt = BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging | BuildOptions.SymlinkLibraries;
        }

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.targetGroup = group;
        options.target = target;
        options.scenes = ProjectBuilder.FindEnabledEditorScenes();
        if (group == BuildTargetGroup.Android)
        {
            string outputName = EnvUtil.GetEnvValue(ENV_OUTPUT_NAME, PlayerSettings.productName + ".apk");
            options.locationPathName = Path.Combine(buildPath, outputName);
        }
        else
        {
            options.locationPathName = buildPath;
        }
        options.options = opt;

        if (ProjectBuilder.GenericBuild(options))
        {

        }
        else
        {

        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, originalD);
        PlayerSettings.applicationIdentifier = originalId;
    }

    private static string UnitySymbolsFromArg()
    {
        string[] result = EnvUtil.FindArgsStartWith(ENV_DEFINE_SYMBOLS);
        StringBuilder sb = new StringBuilder();
        foreach (var arg in result)
        {
            sb.Append(arg.Substring(2));
            sb.Append(";");
        }

        return sb.ToString();
    }


}



