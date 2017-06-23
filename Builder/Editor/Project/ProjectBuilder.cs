using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

class ProjectBuilder
{
	public static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled)
                continue;
            EditorScenes.Add(scene.path);
        }

        return EditorScenes.ToArray();
    }

    public static bool GenericBuild(BuildPlayerOptions options)
    {
        string res = BuildPipeline.BuildPlayer(options);
        if (res.Length > 0)
        {
            Debug.LogError("BuildPlayer failure: " + res);
            return false;
        }
        return true;
    }


}