using BlitzLib.AssembyInfo;
using BlitzLib.Elector.Models;
using BlitzLib.LogFactory;
using BlitzLib.RedisElector;
using BlitzLib.RedisElector.Models;

using Microsoft.Extensions.Logging;

using System;
using System.Threading;

namespace Demo_RedisElector_Singleton
{
    class Program
    {
        static ILogger logger;
        static int exitCode = 0;
        static bool shouldRun = true;
        static Random dice = new Random();

        static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            logger = LogFactoryHelper.CreateLogger<Program>();

            string fallbackFilename = @"VCAP_Services.json";
            if (args.Length > 0) fallbackFilename = args[0];

            // Get Assembly Object, casting it to this class type
            var assembly = typeof(Program).Assembly;
            // Get all custom attribute data
            foreach (var attribute in assembly.GetCustomAttributesData())
            {
                // try to find a value
                if (!attribute.TryParse(out string value)) value = string.Empty;
                // write name and value
                if(!string.IsNullOrWhiteSpace(value)) logger.LogInformation($"{attribute.AttributeType.Name} - {value}");
            }

            var whoIAm = new ElectorInfo() { ApplicationName = "Demo_RedisElector_Singleton", LastCallUtc = DateTime.UtcNow, UniqueInstanceId = Guid.NewGuid().ToString() };
            logger.LogInformation("I Am: {0}", whoIAm.ToString());

            var redisConfig = GetConfig();
            var prov = new RedisElectorProvider(redisConfig);
            prov.SetExpirationMilliseconds(RedisElectorProvider.Recommended_ExpirationMilliseconds);

            while (shouldRun)
            {
                if (prov.AmIPrimary(whoIAm))
                {
                    if (dice.Next(1, 100) > 70)
                    {
                        int waiter = RedisElectorProvider.Recommended_ExpirationMilliseconds * 2;
                        logger.LogWarning("{0} for {1} ms, Primary Faulting", whoIAm.UniqueInstanceId, waiter);
                        prov.ForceElection(whoIAm.ApplicationName);
                        Thread.Sleep(waiter);
                    }
                    else
                    {
                        // Fake: Unit of Work
                        int waiter = dice.Next(RedisElectorProvider.Minimim_ExpirationMilliseconds, RedisElectorProvider.Recommended_ExpirationMilliseconds * 2);
                        logger.LogInformation("{0} for {1} ms, Primary Working", whoIAm.UniqueInstanceId, waiter);
                        Thread.Sleep(waiter);
                    }
                }
                else
                {
                    int waiter = RedisElectorProvider.Minimim_ExpirationMilliseconds;
                    logger.LogInformation("{0} for {1} ms, Not Primary", whoIAm.UniqueInstanceId, waiter);
                    Thread.Sleep(waiter);
                }
            }

            Environment.ExitCode = exitCode;
            return exitCode;
        }

        #region "Global Error Handler"

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = new InvalidOperationException("Bad things");
            if ((e != null) && (e.ExceptionObject != null))
            {
                var ex2 = e.ExceptionObject as Exception;
                if (ex2 != null) ex = ex2;
            }

            shouldRun = false;
            logger.LogError(ex, "CurrentDomain_UnhandledException");
        }

        #endregion

        #region "REDIS Configuration"

        private static RedisConfiguration GetConfig()
        {
            var config = new RedisConfiguration();

            return config;
        }

        #endregion

    }
}
