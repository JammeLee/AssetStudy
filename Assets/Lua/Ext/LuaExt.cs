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

        return null;
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}
