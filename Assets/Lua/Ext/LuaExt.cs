using System.Runtime.InteropServices;
using MFrame.Common;
using XLua;
using LuaDLL = XLua.LuaDLL.Lua;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
using ILuaState = System.IntPtr;

public static class LuaExt
{
    public static void GC(this ILuaState self, LuaGCOptions option, int data)
    {
        LuaDLL.lua_gc(self, option, data);
    }

    public static void NewTable(this ILuaState self)
    {
        LuaDLL.lua_newtable(self);
    }

    public static void CreateTable(this ILuaState self, int narr, int nrec)
    {
        LuaDLL.lua_createtable(self, narr, nrec);
    }

    public static void RawGet(this ILuaState self, int index)
    {
        LuaDLL.lua_rawget(self, index);
    }

    public static void RawSet(this ILuaState self, int index)
    {
        LuaDLL.lua_rawset(self, index);
    }

    public static void RawGetI(this ILuaState self, int index, int n)
    {
        LuaDLL.xlua_rawgeti(self, index, n);
    }

    public static void RawSetI(this ILuaState self, int index, int n)
    {
        LuaDLL.xlua_rawseti(self, index, n);
    }

    public static bool GetMetaTable(this ILuaState self, int index)
    {
        return LuaDLL.lua_getmetatable(self, index) != 0;
    }

    public static void SetMetaTable(this ILuaState self, int index)
    {
        LuaDLL.lua_setmetatable(self, index);
    }

    public static void GetField(this ILuaState self, int index, string name)
    {
        LuaDLL.lua_getfield(self, index, name);
    }

    public static void GetField(this ILuaState self, int index, int n)
    {
        LuaDLL.lua_geti(self, index, n);
    }

    public static void GetFieldByPath(this ILuaState self, int index, string path)
    {
        LuaDLL.xlua_pgettable_bypath(self, index, path);
    }

    public static void SetField(this ILuaState self, int index, string name)
    {
        LuaDLL.lua_setfield(self, index, name);
    }

    public static void SetField(this ILuaState self, int index, int n)
    {
        throw new System.NotImplementedException();
    }

    public static void SetFieldByPath(this ILuaState self, int index, string path)
    {
        LuaDLL.xlua_psettable_bypath(self, index, path);
    }

    public static void GetTable(this ILuaState self, int index)
    {
        LuaDLL.xlua_pgettable(self, index);
    }

    public static void SetTable(this ILuaState self, int index)
    {
        LuaDLL.xlua_psettable(self, index);
    }

    public static LuaTypes Type(this ILuaState self, int index)
    {
        return LuaDLL.lua_type(self, index);
    }

    public static string Class(this ILuaState self, int index)
    {
        // if (self.IsTable(index) && self.GetMetaTable(index))
        // {
        //     self.PushString(tableap)
        // }
        throw new System.NotImplementedException();
        return null;
    }

    public static string TypeName(this ILuaState self, LuaTypes type)
    {
        return type.ToString();
    }

    public static void Pop(this ILuaState self, int n)
    {
        LuaDLL.lua_pop(self, n);
    }

    public static void Insert(this ILuaState self, int index)
    {
        LuaDLL.lua_insert(self, index);
    }

    public static void Remove(this ILuaState self, int index)
    {
        LuaDLL.lua_remove(self, index);
    }

    public static int GetTop(this ILuaState self)
    {
        return LuaDLL.lua_gettop(self);
    }

    public static void SetTop(this ILuaState self, int newTop)
    {
        LuaDLL.lua_settop(self, newTop);
    }

    public static bool Next(this ILuaState self, int index)
    {
        return LuaDLL.lua_next(self, index) != 0;
    }

    public static void GetGlobal(this ILuaState self, string name)
    {
        LuaDLL.xlua_getglobal(self, name);
    }

    public static void SetGlobal(this ILuaState self, string name)
    {
        LuaDLL.xlua_setglobal(self, name);
    }

    public static bool IsNil(this ILuaState self, int index)
    {
        return LuaDLL.lua_isnil(self, index);
    }

    public static bool IsNone(this ILuaState self, int index)
    {
        return self.Type(index) == LuaTypes.LUA_TNONE;
    }

    public static bool IsNoneOrNil(this ILuaState self, int index)
    {
        var lType = self.Type(index);
        return lType == LuaTypes.LUA_TNIL || lType == LuaTypes.LUA_TNONE;
    }

    public static bool IsBoolean(this ILuaState self, int index)
    {
        return LuaDLL.lua_isboolean(self, index);
    }

    public static bool IsFunction(this ILuaState self, int index)
    {
        return LuaDLL.lua_isfunction(self, index);
    }

    public static bool IsLightUserData(this ILuaState self, int index)
    {
        return LuaDLL.lua_islightuserdata(self, index);
    }

    public static bool IsString(this ILuaState self, int index)
    {
        return LuaDLL.lua_isstring(self, index);
    }
    
    public static bool IsNumber(this ILuaState self, int index)
    {
        return LuaDLL.lua_isnumber(self, index);
    }
    
    public static bool IsLong(this ILuaState self, int index)
    {
        return LuaDLL.lua_isint64(self, index);
    }
    
    public static bool IsTable(this ILuaState self, int index)
    {
        return LuaDLL.lua_istable(self, index);
    }
    
    public static bool IsUserData(this ILuaState self, int index)
    {
        return LuaDLL.lua_type(self, index) == LuaTypes.LUA_TUSERDATA;
    }
    
    public static bool IsClass(this ILuaState self, int index, string name)
    {
        return name == self.Class(index);
    }

    public static void TypeError(this ILuaState self, int index, string tname)
    {
        LuaDLL.luaL_error(self, string.Format("{0} expected, got {1}", tname, LuaDLL.lua_type(self, index)));
    }

    public static void PushNil(this ILuaState self)
    {
        LuaDLL.lua_pushnil(self);
    }

    public static void PushValue(this ILuaState self, int index)
    {
        LuaDLL.lua_pushvalue(self, index);
    }
    
    public static void PushBooleean(this ILuaState self, bool value)
    {
        LuaDLL.lua_pushboolean(self, value);
    }
    
    public static void PushBytes(this ILuaState self, byte[] bytes, int len = -1)
    {
        if (bytes == null)
        {
            LuaDLL.lua_pushnil(self);
        }
        else
        {
            if (len < 0)
            {
                len = bytes.Length;
            }
            
            LuaDLL.xlua_pushlstring(self, bytes, len);
        }
    }
    
    public static void PushString(this ILuaState self, string value)
    {
        LuaDLL.lua_pushstring(self, value);
    }

    public static void PushNumber(this ILuaState self, double value)
    {
        if (System.Math.Abs(value % 1) < double.Epsilon)
        {
            if (System.Math.Abs(value) > int.MaxValue)
            {
                LuaDLL.lua_pushint64(self, (long) value);
            }
            else
            {
                LuaDLL.xlua_pushinteger(self, (int) value);
            }
        }
        else
        {
            LuaDLL.lua_pushnumber(self, value);
        }
    }
    
    public static void PushInteger(this ILuaState self, int value)
    {
        LuaDLL.xlua_pushinteger(self, value);
    }
    
    public static void PushLong(this ILuaState self, long value)
    {
        LuaDLL.lua_pushint64(self, value);
    }
    
    public static void PushULong(this ILuaState self, ulong value)
    {
        LuaDLL.lua_pushuint64(self, value);
    }

    public static void PushLightUserData(this ILuaState self, object value)
    {
        if (value != null)
        {
            if (value.GetType().IsValueType)
            {
                MLog.W("压栈[{0}]为值类型", value);
            }

            ObjectTranslatorPool.Instance.Find(self).Push(self, value);
        }
        else
        {
            self.PushNil();
        }
    }
    
    public static void PushCSharpFunction(this ILuaState self, LuaCSFunction value)
    {
        LuaDLL.lua_pushstdcallcfunction(self, value);
    }

    public static double ToNumber(this ILuaState self, int index)
    {
        return LuaDLL.lua_tonumber(self, index);
    }
    
    public static int ToInteger(this ILuaState self, int index)
    {
        return LuaDLL.xlua_tointeger(self, index);
    }
    
    public static long ToLong(this ILuaState self, int index)
    {
        return LuaDLL.lua_toint64(self, index);
    }
    
    public static ulong ToULong(this ILuaState self, int index)
    {
        return LuaDLL.lua_touint64(self, index);
    }
    
    public static bool ToBoolean(this ILuaState self, int index)
    {
        return LuaDLL.lua_toboolean(self, index);
    }

    public static string ToString(this ILuaState self, int index)
    {
        return LuaDLL.lua_tostring(self, index);
    }

    public static byte[] ToBytes(this ILuaState self, int index)
    {
        return LuaDLL.lua_tobytes(self, index);
    }

    public static System.IntPtr ToBufferPtr(this ILuaState self, int index, out int len)
    {
        len = 0;
        System.IntPtr strLen;
        var ptr = LuaDLL.lua_tolstring(self, index, out strLen);
        if (ptr != System.IntPtr.Zero)
        {
            len = strLen.ToInt32();
        }

        return ptr;
    }

    public static int ToBuffer(this ILuaState self, int index, byte[] buffer, int startIdx = 0)
    {
        System.IntPtr strLen;
        var str = LuaDLL.lua_tolstring(self, index, out strLen);
        if (str != System.IntPtr.Zero)
        {
            int len = System.Math.Min(strLen.ToInt32(), buffer.Length - startIdx);
            Marshal.Copy(str, buffer, startIdx, len);
            return len;
        }

        return -1;
    }

    public static System.IntPtr ToLString(this ILuaState self, int index, out int len)
    {
        return LuaDLL.lua_tolstring(self, index, out len);
    }

    public static object ToUserData(this ILuaState self, int index)
    {
        if (self.IsUserData(index))
        {
            return ObjectTranslatorPool.Instance.Find(self).FastGetCSObj(self, index);
        }else if (!self.IsNoneOrNil(index))
        {
            MLog.W("try to convert {0} to {1}", self.Type(index), LuaTypes.LUA_TUSERDATA);
        }

        return null;
    }

    public static void Replace(this ILuaState self, int index)
    {
        LuaDLL.lua_replace(self, index);
    }

    public static int OjbLen(this ILuaState self, int index)
    {
        return LuaDLL.lua_objlen(self, index);
    }

    public static LuaThreadStatus PCall(this ILuaState self, int nArgs, int nResults, int errfunc)
    {
        return (LuaThreadStatus) LuaDLL.lua_pcall(self, nArgs, nResults, errfunc);
    }

    public static void PushErrorFunc(this ILuaState L)
    {
        LuaDLL.load_error_func(L, LuaDLL.get_error_func_ref(L));
    }

    public static string DebugCurrentLine(this ILuaState L, int level)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        string source;
        int line;
        
        L.GetGlobal("debug");
        L.GetField(-1, "getinfo");
        L.Replace(-2);
        L.PushNumber(level);
        L.PushString("Sl");
        L.PCall(2, 1, 0);
        
        L.GetField(-1, "source");
        source = L.ToString(-1);
        L.Pop(1);
        
        L.GetField(-1, "currentline");
        line = L.ToInteger(-1);
        L.Pop(2);

        return string.Format("[{0}:{1}]", source, line);
        
#else
        return "LUA";
#endif
    }
    
    public static LuaThreadStatus L_LoadBuffer(this ILuaState self, byte[] buff, string name)
    {
        return (LuaThreadStatus)LuaDLL.luaL_loadbuffer(self, buff, buff.Length, name);
    }






















}
