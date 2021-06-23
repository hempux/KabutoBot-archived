using Microsoft.Extensions.Configuration;
using System.IO;

namespace net.hempux.Utilities
{
    public class AppSettings
    {
        private static AppSettings _appSettings;

        public IConfigurationSection Ninja { get; private set; }
        public IConfigurationSection Teams { get; private set; }
        public IConfigurationSection Mqtt_BrokerHost { get; private set; }
        public IConfigurationSection Mqtt_Client { get; private set; }

        public string ListenAddress { get; private set; }
        public string SqliteDatabase{ get; private set; }

        public AppSettings(IConfiguration config)
        {
            Ninja = config.GetSection("Ninja");

            Teams = config.GetSection("Teams");

            SqliteDatabase = string.Concat(Directory.GetCurrentDirectory(),@"\",config.GetSection("SqliteDatabase").Value);

            ListenAddress = config.GetSection("applicationUrl").Value;
            // Now set Current
            _appSettings = this;
        }

        /// <summary>
        /// Enables easy access to appsettings.json sections. More can be added in the AppSettings Class
        /// <para>string authkey = AppSettings.Current.Credentials['Authkey'];</para>
        /// </summary>
        public static AppSettings Current
        {
            get
            {
                if (_appSettings == null)
                {
                    _appSettings = GetCurrentSettings();
                }

                return _appSettings;
            }
        }

        

        public static AppSettings GetCurrentSettings()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            var settings = new AppSettings(configuration);

            return settings;
        }
    }
}
