using System.Collections;
using System.Collections.Generic;
using MFrame.Common;
using UnityEngine;
using XLua;

using ILuaState = System.IntPtr;

public class LuaScriptMgr : MonoSingleton<LuaScriptMgr>
{
    public const string SCRIPT_START_FILE = "framework/main";

    protected LuaTable m_Tb;
    protected LuaEnv m_Env;

    public LuaEnv Env
    {
        get
        {
            if (m_Env == null)
            {
                m_Env = new LuaEnv();
                InitLuaEnv();
                
                // L.Get
            }

            return m_Env;
        }
    }

    public ILuaState L
    {
        get
        {
            return Env.L;
        }
    }

    protected virtual void InitLuaEnv()
    {
        
    }

    protected override void Awaking()
    {
        base.Awaking();
    }
}
