using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public static class AssemblyScanner
{
    public static IEnumerable<Assembly> GetAssemblies()
    {
        foreach (var path in Directory.EnumerateFiles(Environment.CurrentDirectory, "*.dll"))
        {
            Assembly.LoadFile(path);
        }

        return AppDomain.CurrentDomain.GetAssemblies();
    }
}
