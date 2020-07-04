using System;
using System.Diagnostics;
using System.IO;
using MFrame.Common;
using UnityEngine;
using XLua;
using ILuaState = System.IntPtr;
using Logger = MFrame.Debug.Logger;

public static class ChunkAPI
{
    public static string LuaROOT = string.Format("{0}/Essets/LuaRoot",
        Path.GetDirectoryName(Application.dataPath.Replace("\\", "/")));

    public static string GetFilePath(string fileName)
    {
        return string.IsNullOrEmpty(fileName) ? LuaROOT : string.Format("{0}/{1}", LuaROOT, fileName);
    }

    public static FileSystemWatcher _FSWatcher;

    [Conditional(Logger.UNITY_EDITOR)]
    public static void InitFileWatcher(FileSystemEventHandler onChanged, RenamedEventHandler onRenamed)
    {
        UninitFileWatcher();
        
        try
        {
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            System.Environment.SetEnvironmentVariable("MONO_MANAGED_WATCHER", "enabled");
#endif
            _FSWatcher = new FileSystemWatcher(LuaROOT, "*.lua")
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
            };
            if (onChanged != null)
            {
                _FSWatcher.Changed += onChanged;
                _FSWatcher.Created += onChanged;
                _FSWatcher.Deleted += onChanged;
            }

            if (onRenamed != null)
            {
                _FSWatcher.Renamed += onRenamed;
            }
        }
        catch (Exception e)
        {
            MLog.W("InitFileWatcher Failure: {0}", e);
        }
    }

    [Conditional(Logger.UNITY_EDITOR)]
    public static void UninitFileWatcher()
    {
        if (_FSWatcher != null)
        {
            _FSWatcher.Dispose();
            _FSWatcher = null;
        }
    }

    public static byte[] __Loader(ref string file)
    {
#if UNITY_EDITOR
        
#endif
        byte[] nbytes = null;
        if (false)
        {
            
        }
        else
        {
            if (!file.OrdinalEndsWith(".lua"))
            {
                file += ".lua";
            }

            var luaPath = GetFilePath(file);
            if (!File.Exists(luaPath))
            {
                return null;
            }

            nbytes = File.ReadAllBytes(luaPath);
        }
        MLog.D("lua bytes: {0}", nbytes);
        return nbytes;
    }

    private static byte[] LoadBytes(this ILuaState L, string fileName)
    {
        string lowerName = fileName.ToLower();
        if (lowerName.OrdinalEndsWith(".lua"))
        {
            int index = fileName.LastIndexOf('.');
            fileName = fileName.Substring(0, index);
        }

        fileName = fileName.Replace('.', '/');

        return __Loader(ref fileName);
    }

    private static LuaThreadStatus LoadFile(this ILuaState L, string fileName)
    {
        var bytes = L.LoadBytes(fileName);
        if (bytes != null)
        {
            return L.L_LoadBuffer(bytes, fileName);
        }
        L.PushNil();

        return LuaThreadStatus.LUA_OK;
    }

    public static int DoFile(this ILuaState L, string fileName)
    {
        var bytes = L.LoadBytes(fileName);
        if (bytes != null)
        {
            var oldTop = L.GetTop();
            L.PushErrorFunc();
            var errfunc = oldTop + 1;
            if (L.L_LoadBuffer(bytes, fileName) == LuaThreadStatus.LUA_OK)
            {
                L.PushString(fileName);
                
                if (L.PCall(1, -1, errfunc) == 0)
                {
                    L.Remove(errfunc);
                    return L.GetTop() - oldTop;
                }
            }

            ObjectTranslatorPool.Instance.Find(L).luaEnv.ThrowExceptionFromError(oldTop);
        }

        return 0;
    }

}