namespace MFrame.Debug
{
    using UnityEngine;
    public enum LogLevel
    {
        E = LogType.Error,
        W = LogType.Warning,
        D = LogType.Log,
        I
    }
    public static class Logger
    {
        public const string UNITY_EDITOR = "UNITY_EDITOR";
        public const string UNITY_STANDALONE = "UNITY_STANDALONE";
        public const string DEVELOPMENT_BUILD = "DEVELOPMENT_BUILD";
        
        private static ILogger m_ULogger = Debug.unityLogger;
        private static LogLevel m_Level = LogLevel.D;

        public static LogLevel level
        {
            get { return m_Level; }
            set
            {
                m_Level = value;
                if (m_ULogger != null)
                    m_ULogger.filterLogType = (LogType) value;
            }
        }

        public static void LogFormat(this Object context, LogLevel l, string format, params object[] args)
        {
            if (m_ULogger == null) return;
            if (l < LogLevel.I)
            {
                m_ULogger.LogFormat((LogType)l, context, format, args);
            }
            else if (m_Level == LogLevel.I)
            {
                m_ULogger.LogFormat(LogType.Log, context, format, args);
            }
        }

        public static void Format(LogLevel l, string format, params object[] args)
        {
            if (m_ULogger == null) return;

            if (l < LogLevel.I)
            {
                m_ULogger.LogFormat((LogType)l, format, args);
            }
            else if (m_Level == LogLevel.I)
            {
                m_ULogger.LogFormat(LogType.Log, format, args);
            }
        }

        public static void Exception(System.Exception exception, Object context)
        {
            if (m_ULogger != null)
            {
                m_ULogger.LogException(exception, context);
            }
        }
    }
}