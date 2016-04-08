using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NServiceBus.Logging;

public static class AssemblyScanner
{
    public static IEnumerable<Assembly> GetAssemblies()
    {
        var l = LogManager.GetLogger(typeof(AssemblyScanner));

        foreach (var path in Directory.EnumerateFiles(Environment.CurrentDirectory, "*.dll"))
        {
            Assembly.LoadFile(path);
        }

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var a in assemblies.OrderBy(a=>a.ToString()))
        {
            l.InfoFormat("Loaded: {0}", a);
        }

        return assemblies;


    }
}
