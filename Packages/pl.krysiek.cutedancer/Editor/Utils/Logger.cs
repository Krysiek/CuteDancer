using UnityEngine;

public class Logger
{
    public enum LogLevel
    {
        DEBUG = 0, INFO = 1, WARN = 2, ERROR = 3
    }

    private static LogLevel currentLevel = LogLevel.INFO;
    public static LogLevel CurrentLevel { get => currentLevel; set => currentLevel = value; }

    private string logFormat;

    public Logger(string className)
    {
        logFormat = "<color={0}>[{1}]</color> [CuteDancer] <color=white>" + className + "</color>: {2}";
    }

    private void Log(LogLevel level, string message)
    {
        if (level >= currentLevel)
        {
            switch (level)
            {
                case LogLevel.WARN:
                    Debug.LogWarningFormat(logFormat, "yellow", level, message);
                    break;
                case LogLevel.ERROR:
                    Debug.LogErrorFormat(logFormat, "red", level, message);
                    break;
                default:
                    Debug.LogFormat(logFormat, level == LogLevel.DEBUG ? "blue" : "green", level, message);
                    break;
            }
        }
    }

    // names has to start with Log to maintain original double-click on log behaviour
    // [HideInCallstack] is used optionally to hide the methods in stack when Strip logging callstack is enabled
    [HideInCallstack] public void LogDebug(string message) { Log(LogLevel.DEBUG, message); }
    [HideInCallstack] public void LogInfo(string message) { Log(LogLevel.INFO, message); }
    [HideInCallstack] public void LogWarn(string message) { Log(LogLevel.WARN, message); }
    [HideInCallstack] public void LogError(string message) { Log(LogLevel.ERROR, message); }
}
