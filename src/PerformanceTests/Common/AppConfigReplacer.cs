namespace Utils
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;

    public static class AppConfigReplacer
    {
        public static void Replace(string path)
        {
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", path);
            ResetConfigMechanism();
        }

        private static void ResetConfigMechanism()
        {
            typeof(ConfigurationManager)
                .GetField("s_initState", BindingFlags.NonPublic |
                                            BindingFlags.Static)
                .SetValue(null, 0);

            typeof(ConfigurationManager)
                .GetField("s_configSystem", BindingFlags.NonPublic |
                                            BindingFlags.Static)
                .SetValue(null, null);

            typeof(ConfigurationManager)
                .Assembly.GetTypes()
                .Where(x => x.FullName ==
                            "System.Configuration.ClientConfigPaths")
                .First()
                .GetField("s_current", BindingFlags.NonPublic |
                                        BindingFlags.Static)
                .SetValue(null, null);
        }
    }
}