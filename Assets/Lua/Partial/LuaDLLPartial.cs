﻿
using System;
using System.Runtime.InteropServices;

namespace XLua.LuaDLL
{
    public static partial class Lua
    {
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushinteger(IntPtr L, long value);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern long lua_tointegerx(IntPtr L, int index, out int isnum);

        public static long lua_tointeger(IntPtr L, int index)
        {
            int isnum;
            return lua_tointegerx(L, index, out isnum);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_setfield(IntPtr L, int index, string name);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_getfield(IntPtr L, int index, string name);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_geti(IntPtr L, int index, int n);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_getmetatable(IntPtr L, int objIndex);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_argerror(IntPtr luaState, int narg, string extramsg);

        public static IntPtr lua_tolstring(IntPtr L, int index, out int len)
        {
            IntPtr @int;
            lua_tolstring(L, index, out @int);
            len = @int.ToInt32();
            return @int;
        }

        public static int lua_objlen(IntPtr L, int index)
        {
            return (int) xlua_objlen(L, index);
        }

        public static int luaL_loadbuffer(IntPtr L, byte[] buff, int buffLen, string name)
        {
            return xluaL_loadbuffer(L, buff, buffLen, name);
        }

        public static int luaL_dostring(IntPtr L, string buff)
        {
            var b = load_error_func(L, get_error_func_ref(L));
            luaL_loadbuffer(L, buff, "nil");
            return lua_pcall(L, 0, -1, b);
        }
        
    }
}

