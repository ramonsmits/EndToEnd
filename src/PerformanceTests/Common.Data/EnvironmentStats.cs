using System;
using System.Net;
using NServiceBus.Logging;

static class EnvironmentStats
{
    public static void Write()
    {
        var log = LogManager.GetLogger("Environment");
        log.InfoFormat("IsServerGC:{0} ({1})", System.Runtime.GCSettings.IsServerGC, System.Runtime.GCSettings.LatencyMode);
        log.InfoFormat("ProcessorCount: {0}", Environment.ProcessorCount);
        log.InfoFormat("64bit: {0}", Environment.Is64BitProcess);
        log.InfoFormat("ServicePointManager.DefaultConnectionLimit: {0}", ServicePointManager.DefaultConnectionLimit);
        log.InfoFormat("ServicePointManager.DefaultNonPersistentConnectionLimit: {0}", ServicePointManager.DefaultNonPersistentConnectionLimit);
        log.InfoFormat("ServicePointManager.EnableDnsRoundRobin: {0}", ServicePointManager.EnableDnsRoundRobin);
        log.InfoFormat("ServicePointManager.Expect100Continue: {0}", ServicePointManager.Expect100Continue);
        log.InfoFormat("ServicePointManager.UseNagleAlgorithm: {0}", ServicePointManager.UseNagleAlgorithm);
    }
}
