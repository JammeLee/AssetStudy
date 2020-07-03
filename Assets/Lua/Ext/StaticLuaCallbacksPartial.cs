using ILuaState = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
namespace XLua
{
    public static class StaticLuaCallbacksPartial
    {
        public static int loadfile(ILuaState L)
        {
            string name = L.ToString(1);
            var oldTop = L.GetTop();
            L.PushErrorFunc();
//            if (L.Load)
//            {
//                
//            }
            return 0;
        }
    }
}