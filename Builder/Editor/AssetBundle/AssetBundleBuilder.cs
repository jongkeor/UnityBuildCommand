using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;




public class AssetBundleBuilder
{
    	public static string GetPlatformForAssetBundles (BuildTarget target)
		{
			switch (target) {
			case BuildTarget.Android:
				return "Android";
			case BuildTarget.iOS:
				return "iOS";
			case BuildTarget.WebGL:
				return "WebGL";
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return "Windows";
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.StandaloneOSXUniversal:
				return "OSX";
			default:
				return null;
			}
		}
    public static string GetAssetBundlePath(BuildTarget target, string outputPath)
    {
        string output = Path.Combine(outputPath, AssetBundleBuilder.GetPlatformForAssetBundles(target));
        return output;
    }

    public static List<string> BuildAssetBundle(BuildTarget buildTarget, string outputPath)
    {

        AssetDatabase.RemoveUnusedAssetBundleNames();

        //			AssetDatabase.RemoveAssetBundleName("banner", true);
        //			string[] t =   AssetDatabase.GetAssetPathsFromAssetBundle("banner");
        //			foreach(var e in t ){
        //				Debug.Log(e);
        //			}


        string output = GetAssetBundlePath(buildTarget, outputPath);
        List<string> changedList = new List<string>();
        if (!Directory.Exists(output))
        {
            Directory.CreateDirectory(output);
        }
        Dictionary<string, uint> crcList = new Dictionary<string, uint>();
        string[] bundleNames = AssetDatabase.GetAllAssetBundleNames();
        foreach (string bundleName in bundleNames)
        {
            uint crc;
            if (BuildPipeline.GetCRCForAssetBundle(Path.Combine(output, bundleName), out crc))
            {
                crcList.Add(bundleName, crc);
            }
        }
        BuildPipeline.BuildAssetBundles(output, BuildAssetBundleOptions.DeterministicAssetBundle, buildTarget);
        foreach (string bundleName in bundleNames)
        {
            uint crc;
            if (BuildPipeline.GetCRCForAssetBundle(Path.Combine(output, bundleName), out crc))
            {
                if (!crcList.ContainsKey(bundleName) || !crcList[bundleName].Equals(crc))
                {
                    changedList.Add(bundleName);
                }
            }
        }
        return changedList;
    }

    public static void CopyToStreamingAssetBundle(BuildTarget buildTarget, string inputPath)
    {
        FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
        Directory.CreateDirectory(Application.streamingAssetsPath);
        string source = GetAssetBundlePath(buildTarget, inputPath);

        string destination = GetAssetBundlePath(buildTarget, Application.streamingAssetsPath);
        if (!System.IO.Directory.Exists(source))
            Debug.Log("No assetBundle output folder, try to build the assetBundles first.");

        FileUtil.CopyFileOrDirectory(source, destination);
        AssetDatabase.Refresh();
    }

    public static void DeleteStreamingAssetBundle()
    {
        FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
    }

    public static void ExecuteShellScript(string shellscript, params string[] environments)
    {
#if UNITY_EDITOR_OSX
        try
        {
            var writer = System.IO.File.CreateText("test.sh");
            writer.Write(shellscript);
            writer.Close();
            System.Diagnostics.ProcessStartInfo proc = new System.Diagnostics.ProcessStartInfo();
            proc.FileName = "/bin/sh";
            proc.Arguments = "test.sh";

            proc.UseShellExecute = false;
            proc.RedirectStandardOutput = true;
            proc.RedirectStandardError = true;
            for (int i = 0; i < environments.Length / 2; i++)
            {
                proc.EnvironmentVariables.Add(environments[i * 2], environments[i * 2 + 1]);
            }
            var t = System.Diagnostics.Process.Start(proc);
            string output = t.StandardOutput.ReadToEnd();
            string error = t.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(output))
            {
                Debug.Log(output);
            }
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError(error);
            }
            t.WaitForExit();
        }
        catch (Exception e)
        {
            Debug.LogError(e);

        }
        finally
        {
            File.Delete("test.sh");
        }
#elif UNITY_EDITOR_WIN
			try {
			var writer =  System.IO.File.CreateText ("test.bat");
			writer.Write(shellscript);
			writer.Close ();
			System.Diagnostics.ProcessStartInfo proc = new System.Diagnostics.ProcessStartInfo ();
			proc.FileName = "test.bat";

			proc.UseShellExecute = false;
			proc.RedirectStandardOutput = true;
			proc.RedirectStandardError = true;
			for (int i = 0; i < environments.Length / 2; i++) {
			proc.EnvironmentVariables.Add (environments [i * 2], environments [i * 2 + 1]);
			}
			var t = System.Diagnostics.Process.Start (proc);
			string output = t.StandardOutput.ReadToEnd ();
			string error = t.StandardError.ReadToEnd ();
			if (!string.IsNullOrEmpty (output)) {
			Debug.Log (output);
			}
			if (!string.IsNullOrEmpty (error)) {
			Debug.LogError (error);
			}
			t.WaitForExit ();
			} catch (Exception e){
			Debug.LogError(e);

			} finally {
			File.Delete ("test.bat");
			}

#endif

    }

}


