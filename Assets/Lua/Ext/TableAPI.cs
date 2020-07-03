using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
using ILuaState = System.IntPtr;


public static class TableAPI
{
    public const string META_CLASS = "__name";

    public static void SetDict(this ILuaState self, string key, float value)
    {
        self.PushString(key);
        self.PushNumber(value);
        self.RawSet(-3);
    }
    
    public static void SetDict(this ILuaState self, string key, string value)
    {
        self.PushString(key);
        self.PushString(value);
        self.RawSet(-3);
    }
    
    public static void SetDict(this ILuaState self, string key, bool value)
    {
        self.PushString(key);
        self.PushBooleean(value);
        self.RawSet(-3);
    }
    
    public static void SetDict(this ILuaState self, string key, LuaCSFunction value)
    {
        self.PushString(key);
        self.PushCSharpFunction(value);
        self.RawSet(-3);
    }

//    public static float GetNumber(this ILuaState self, int index, string key, float def = 0)
//    {
//        self.GetField(index, key);
//        var ret = self.opt
//    }
}