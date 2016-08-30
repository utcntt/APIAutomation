using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace TestLibrary
{

    /// <summary>
    /// Add a 'appsettings.json' file for storing all configuration information.
    /// </summary>
    public class TestConfig
    {
        public static IConfiguration Config
        {
            get
            {
                if (config == null)
                {
                    //_config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    //string path = _config.FilePath;

                    var builder = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config = builder.Build();
                }
                return config;
            }
        }
        //private static Configuration config;
        private static IConfiguration config;

        public static string GetValueFromConfig(string key)
        {
            //string value = string.Empty;
            //KeyValueConfigurationElement keyValueConfig = Config.AppSettings.Settings[key];
            //if (keyValueConfig != null)
            //{
            //    value = keyValueConfig.Value;
            //}
            //return value;
            return Config.GetSection(key).Value;
        }

        public static string ServerURL
        {
            get
            {
                return GetValueFromConfig("ServerURL");
            }
        }

    }

}
