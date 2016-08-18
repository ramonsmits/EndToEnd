using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using NServiceBus;
using NServiceBus.Transport.AzureServiceBus;

class V6Sanitization : ISanitizationStrategy
{
    public string Sanitize(string entityPathOrName, EntityType entityType)
    {
        // remove invalid characters
        var regex = new Regex(@"[^a-zA-Z0-9\-\._]");
        entityPathOrName = regex.Replace(entityPathOrName, string.Empty);

        var entityPathOrNameMaxLength = 0;

        switch (entityType)
        {
            case EntityType.Queue:
            case EntityType.Topic:
                entityPathOrNameMaxLength = 260;
                break;
            case EntityType.Subscription:
            case EntityType.Rule:
                entityPathOrNameMaxLength = 50;
                break;
        }

        // hash if still too long
        if (entityPathOrName.Length > entityPathOrNameMaxLength)
        {
            entityPathOrName = MD5DeterministicNameBuilder.Build(entityPathOrName);
        }

        return entityPathOrName;
    }

    static class MD5DeterministicNameBuilder
    {
        public static string Build(string input)
        {
            var inputBytes = Encoding.Default.GetBytes(input);
            //use MD5 hash to get a 16-byte hash of the string
            using (var provider = new MD5CryptoServiceProvider())
            {
                var hashBytes = provider.ComputeHash(inputBytes);
                return new Guid(hashBytes).ToString();
            }
        }
    }
}