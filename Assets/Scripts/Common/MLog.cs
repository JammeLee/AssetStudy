using UnityEngine;

namespace MFrame.Common
{
    using MFrame.Debug;
    public class MLog : MonoSingleton<MLog>
    {
        public LogLevel level = LogLevel.D;
        public static LogLevel logLevel
        {
            get => Logger.level;
            private set => Logger.level = value;
        }

        protected override void Awaking()
        {
#if UNITY_EDITOR
            logLevel = level;
#endif
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            logLevel = level;
        }
#endif

        public void SetLevel(LogLevel l)
        {
            level = l;
            logLevel = l;
        }

        public static void Log(LogLevel l, string format, params object[] args)
        {
            Logger.Format(l, format, args);
        }

        public static void Log(LogLevel l, Object context, string format, params object[] args)
        {
            Logger.LogFormat(context, l, format, args);
        }

        public static void I(string format, params object[] args)
        {
            Log(LogLevel.I, format, args);
        }

        public static void D(string format, params object[] args)
        {
            Log(LogLevel.D, format, args);
        }

        public static void W(string format, params object[] args)
        {
            Log(LogLevel.W, format, args);
        }

        public static void E(string format, params object[] args)
        {
            Log(LogLevel.E, format, args);
        }

        public static void I(Object context, string format, params object[] args)
        {
            Log(LogLevel.I, context, format, args);
        }
        
        public static void D(Object context, string format, params object[] args)
        {
            Log(LogLevel.D, context, format, args);
        }
        
        public static void W(Object context, string format, params object[] args)
        {
            Log(LogLevel.W, context, format, args);
        }
        
        public static void E(Object context, string format, params object[] args)
        {
            Log(LogLevel.E, context, format, args);
        }
        
        public static void Info(object msg)
        {
            if (logLevel <= LogLevel.I)
            {
                UnityEngine.Debug.Log(msg);
            }
        }

        public static void Log(object msg)
        {
            if (logLevel <= LogLevel.D)
            {
                UnityEngine.Debug.Log(msg);
            }
        }
        
        public static void LogWarning(object msg)
        {
            if (logLevel <= LogLevel.W)
            {
                UnityEngine.Debug.LogWarning(msg);
            }
        }
        
        public static void LogError(object msg)
        {
            if (logLevel <= LogLevel.E)
            {
                UnityEngine.Debug.LogError(msg);
            }
        }
        
    }

}