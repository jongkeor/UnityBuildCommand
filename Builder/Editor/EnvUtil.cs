using System;
using System.Collections.Generic;
using System.IO;

public class EnvUtil
{
    public static string MakeAbsolute(string basePath, string relativePath)
    {
        if (!basePath.EndsWith("/"))
            basePath += "/";

        var ret = new System.Uri(new System.Uri(basePath), relativePath);
        return ret.AbsolutePath;
    }

    public static string MakeRelative(string filePath, string referencePath)
    {
        if (!referencePath.EndsWith("/"))
            referencePath += "/";
        var fileUri = new System.Uri(filePath);
        
        var referenceUri = new System.Uri(referencePath );
        
        return referenceUri.MakeRelativeUri(fileUri).ToString();
    }
    public static string GetEnvValue(string name, string defaultValue = "")
    {
        string ret = System.Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrEmpty(ret))
        {
            return defaultValue;
        }
        return ret;
    }
    public static bool IsExistEnv(string name)
    {
        string ret = System.Environment.GetEnvironmentVariable(name);
        return ret != null;
    }

    public static bool IsExistCommandArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name)
            {
                return true;
            }
        }
        return false;
    }

    public static string[] FindArgsStartWith(string name)
    {
        List<string> argList = new List<string>();
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith(name))
            {
                argList.Add(args[i]);
            }
        }
        return argList.ToArray();
    }

    public static string GetCommandArg(string name, string defaultValue = "")
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return defaultValue;
    }
}