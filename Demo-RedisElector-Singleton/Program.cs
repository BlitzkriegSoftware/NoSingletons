using System;
using BlitzLib.Elector.Models;
using BlitzLib.RedisElector;
using Microsoft.Extensions.Logging;
using BlitzLib.RedisElector.Models;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Demo_RedisElector_Singleton
{
    class Program
    {
        static ILogger logger;
        static int exitCode = 0;
        static Random dice = new Random();
        static bool shouldRun = true;

        static int Main(string[] args)
        {
            logger =  CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var whoIAm = new ElectorInfo() { ApplicationName = "Demo_RedisElector_Singleton", LastCallUtc = DateTime.UtcNow, UniqueInstanceId = Guid.NewGuid().ToString() };
            var redisConfig = GetConfig("p-redis");
            var prov = new RedisElectorProvider(redisConfig);
            prov.SetExpirationMilliseconds(RedisElectorProvider.Recommended_ExpirationMilliseconds);

            while(shouldRun)
            {
                if(prov.AmIMaster(whoIAm))
                {

                    if (dice.Next(1, 100) > 70)
                    {
                        logger.LogWarning("Faulting: {0}", whoIAm.ToString());
                        Thread.Sleep(RedisElectorProvider.Recommended_ExpirationMilliseconds * 10);
                    } else
                    {
                        // Fake: Unit of Work
                        logger.LogInformation("Doing work: {0}", whoIAm.ToString());
                        int waiter = (int)(RedisElectorProvider.Recommended_ExpirationMilliseconds * dice.NextDouble()) + (int)(RedisElectorProvider.Recommended_ExpirationMilliseconds * 0.25);
                        Thread.Sleep(waiter);
                    }
                }
                else
                {
                    logger.LogInformation("No work: {0}", whoIAm.ToString());
                    Thread.Sleep(RedisElectorProvider.Minimim_ExpirationMilliseconds);
                }
            }

            Environment.ExitCode = exitCode;
            return exitCode;
        }

        #region "Global Error Handler"

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = new InvalidOperationException("Bad things");
            if((e != null) && (e.ExceptionObject != null))
            {
                var ex2 = e.ExceptionObject as Exception;
                if (ex2 != null) ex = ex2;
            }

            shouldRun = false;
            logger.LogError(ex, "CurrentDomain_UnhandledException");
        }

        #endregion

        #region "REDIS Configuration"

        private static RedisConfiguration GetConfig(string serviceName)
        {
            RedisConfiguration config = new RedisConfiguration();

            var vcap_services = Environment.GetEnvironmentVariable("VCAP_SERVICES");
            if(string.IsNullOrWhiteSpace(vcap_services))
            {
                vcap_services = System.IO.File.ReadAllText(@".\VCAP_Services.json");
                logger.LogWarning("VCAP_SERVICES: Falling back to JSON file");
            }

            logger.LogInformation("VCAP_SERVICES: {0}", vcap_services);

            JObject jo = JObject.Parse(vcap_services);

            var svr = jo.SelectToken("$.." + serviceName);

            if (svr != null)
            {
                var credsTokens = svr.SelectToken("$..credentials");
                var list = credsTokens.Children();
                foreach (var item in list)
                {
                    var key = ((Newtonsoft.Json.Linq.JProperty)item).Name;
                    var value = ((Newtonsoft.Json.Linq.JProperty)item).Value.ToString();
                    switch (key.ToLowerInvariant())
                    {
                        case "host":
                            config.Host = value;
                            break;
                        case "password":
                            config.Password = value;
                            break;
                        case "port":
                            config.Port = int.Parse(value);
                            break;
                    }
                }
            }

            return config;
        }

        #endregion

        #region "Log Factory"

        private static ILoggerFactory _Factory = null;

        public static void ConfigureLogger(ILoggerFactory factory)
        {
            factory.AddConsole();
        }

        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_Factory == null)
                {
                    _Factory = new LoggerFactory();
                    ConfigureLogger(_Factory);
                }
                return _Factory;
            }
            set { _Factory = value; }
        }

        public static ILogger CreateLogger() => LoggerFactory.CreateLogger<Program>();

        #endregion

    }
}
