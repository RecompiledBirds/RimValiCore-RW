using System.Runtime.CompilerServices;

namespace RVCRestructured;

public enum RVCLogType
{
    Message,
    Error,
    Warning,
    ErrorOnce
}
public static class RVCLog
{
    public static void MSG(object o, bool condition = true, bool debugOnly = false)
    {
        Log(o, RVCLogType.Message, condition, debugOnly);
    }
    public static void Warn(object o, bool condition=true, bool debugOnly=false)
    {
        Log(o,RVCLogType.Warning,condition,debugOnly);
    }
    public static void Error(object o, bool condition = true, bool debugOnly = false)
    {
        Log(o, RVCLogType.Error, condition, debugOnly);
    }
    public static void Log(object o, RVCLogType type = RVCLogType.Message, bool condition = true, bool debugOnly = false, [CallerMemberName] string name="")
    {
        if (debugOnly && !VineSettings.debugMode)
            return;
        //Used for some conditional logging.
        if (!condition)
            return;
        switch (type)
        {
            case RVCLogType.Message:
                Verse.Log.Message($"[Vine{(VineSettings.debugMode? $"/{name}]":"]")}: {o}");
                break;
            case RVCLogType.Error:
                Verse.Log.Error($"[Vine{(VineSettings.debugMode ? $"/{name}]" : "]")}: {o}");
                break;
            case RVCLogType.Warning:
                Verse.Log.Warning($"[Vine{(VineSettings.debugMode ? $"/{name}]" : "]")}: {o}");
                break;
            case RVCLogType.ErrorOnce:
                Verse.Log.ErrorOnce($"[Vine{(VineSettings.debugMode ? $"/{name}]" : "]")}: {o}", o.GetHashCode());
                break;
        }
    }
}
