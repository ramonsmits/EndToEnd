using System;
using NServiceBus.Logging;

namespace Common
{
    static class Log
    {
        public static void Env()
        {
            var log = LogManager.GetLogger("Environment");
            log.InfoFormat("IsServerGC:{0} ({1})", System.Runtime.GCSettings.IsServerGC, System.Runtime.GCSettings.LatencyMode);
            log.InfoFormat("ProcessorCount: {0}", Environment.ProcessorCount);
            log.InfoFormat("64bit: {0}", Environment.Is64BitProcess);
        }
    }
}
