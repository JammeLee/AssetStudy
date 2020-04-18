using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AssetBundleMenu
{
    private static string path = "Assets/BundleResource";
    private static string OBOPATH = "Assets/BundleResource/OBO";
    private static string CATEGORYPATH = "Assets/BundleResource/CATEGORY";
    
    [MenuItem("Assets/Asset/SetBundleName")]
    public static void AutoSetBundleName()
    {
        var oboDirs = Directory.GetDirectories(OBOPATH);
        foreach (var dir in oboDirs)
        {
//            var dirs = Directory.GetDirectories(dir);
//            foreach (var d in dirs)
//            {
                var bPath = Path.GetFileNameWithoutExtension(dir);
                MarkSingleBundleName(dir, bPath, "*");
//            }
        }

        var categoryDirs = Directory.GetDirectories(CATEGORYPATH);
        foreach (var cdir in categoryDirs)
        {
            var bPath = Path.GetFileNameWithoutExtension(cdir);
            var dirs = Directory.GetDirectories(cdir);
            foreach (var dir in dirs)
            {
                var bName = Path.GetFileNameWithoutExtension(dir);
                var bundleName = string.Format("{0}/{1}", bPath, bName).ToLower();
                MarkPackageBundleName(dir, bundleName, "*");
            }
        }
    }

    private static void MarkSingleBundleName(string rootPath, string bundlePath, string patter, SearchOption option = SearchOption.TopDirectoryOnly)
    {
        var files = Directory.GetFiles(rootPath, patter, option);
        foreach (var file in files)
        {
            var ai = AssetImporter.GetAtPath(file);
            if (ai)
            {
                var fileName = Path.GetFileNameWithoutExtension(file).Replace(".", "-");
                fileName = fileName.Replace("~", ".");
                var nbn = string.Format("{0}/{1}", bundlePath, fileName).ToLower();
                ai.assetBundleName = nbn;
            }
        }
    }

    private static void MarkPackageBundleName(string rootPath, string bundleName, string pattern, SearchOption option = SearchOption.TopDirectoryOnly)
    {
        var files = Directory.GetFiles(rootPath, pattern, option);
        bundleName = bundleName.Replace(".", "-");
        foreach (var file in files)
        {
            var ai = AssetImporter.GetAtPath(file);
            if (ai)
            {
                ai.assetBundleName = bundleName;
            }
        }
    }
}
