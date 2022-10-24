using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured
{
    public enum RVCLogType
    {
        Message,
        Error,
        Warning,
        ErrorOnce
    }
    public static class RVCLog
    {
        public static void Log(object o, RVCLogType type = RVCLogType.Message,bool log =true)
        {
            //Used for some conditional logging.
            if (!log)
                return;
            switch (type)
            {
                case RVCLogType.Message:
                    Verse.Log.Message($"[RVC]: {o}");
                    break;
                case RVCLogType.Error:
                    Verse.Log.Error($"[RVC]: {o}");
                    break;
                case RVCLogType.Warning:
                    Verse.Log.Warning($"[RVC]: {o}");
                    break;
                case RVCLogType.ErrorOnce:
                    Verse.Log.ErrorOnce($"[RVC]: {o}", o.GetHashCode());
                    break;
            }
        }
    }
}
