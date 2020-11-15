using Microsoft.Extensions.Configuration;

namespace VideoAnalytics.Web.Tests
{
    public static class ConfigurationHelper
    {
        public static IConfiguration InitConfig()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .Build();
            return config;
        }
    }
}