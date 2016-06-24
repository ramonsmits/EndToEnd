namespace ServiceControlCompatibilityTests
{
    using System.Configuration;

    public static class ConfigurationSectionExtensions
    {
        public static void Set(this ConnectionStringSettingsCollection collection, string key, string value)
        {
            collection.Remove(key);
            if (!string.IsNullOrWhiteSpace(value))
            {
                collection.Add(new ConnectionStringSettings(key, value));
            }
        }

        public static void Set(this KeyValueConfigurationCollection collection, SettingInfo keyInfo, string value)
        {
            collection.Remove(keyInfo.Name);
            if (!string.IsNullOrWhiteSpace(value))
            {
                collection.Add(new KeyValueConfigurationElement(keyInfo.Name, value));
            }
        }
    }
}