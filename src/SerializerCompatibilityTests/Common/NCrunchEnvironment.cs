using System;

/// <summary>
/// This class is taken from the NCrunch helper assembly. It is needed to access the NCrunch-specific workspacing
/// locations, so the appdomains correctly load assemblies
/// when running under NCrunch.
/// </summary>
static class NCrunchEnvironment
{
    public static bool NCrunchIsResident()
    {
        return Environment.GetEnvironmentVariable("NCrunch") == "1";
    }

    public static string GetOriginalSolutionPath()
    {
        return Environment.GetEnvironmentVariable("NCrunch.OriginalSolutionPath");
    }

    public static string GetOriginalProjectPath()
    {
        return Environment.GetEnvironmentVariable("NCrunch.OriginalProjectPath");
    }

    public static string[] GetImplicitlyReferencedAssemblyLocations()
    {
        var environmentVariable = Environment.GetEnvironmentVariable("NCrunch.ImplicitlyReferencedAssemblyLocations");
        return environmentVariable?.Split(';');
    }

    public static string[] GetAllAssemblyLocations()
    {
        var environmentVariable = Environment.GetEnvironmentVariable("NCrunch.AllAssemblyLocations");
        return environmentVariable?.Split(';');
    }
}