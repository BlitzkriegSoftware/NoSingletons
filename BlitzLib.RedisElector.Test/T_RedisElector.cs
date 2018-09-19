using BlitzLib.Elector.Models;
using BlitzLib.RedisElector.Models;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace BlitzLib.RedisElector.Test
{
    [TestClass]
    public class T_RedisElector
    {
        #region "Properties"
        private RedisConfiguration redisConfig;
        private readonly ILogger _logger;

        private const string ApplicationName = "T_RedisElector";
        #endregion

        #region "CTOR"
        public T_RedisElector()
        {
            _logger = CreateLogger();
            var text = System.IO.File.ReadAllText(@".\ElectorRedisConfig.json");
            redisConfig = JsonConvert.DeserializeObject<RedisConfiguration>(text);
        }
        #endregion

        #region "Log Factory"

        private static ILoggerFactory _Factory = null;

        public void ConfigureLogger(ILoggerFactory factory)
        {
            factory.AddConsole();
        }

        public ILoggerFactory LoggerFactory
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

        public ILogger CreateLogger() => LoggerFactory.CreateLogger<T_RedisElector>();

        #endregion

        [TestMethod]
        public void ConCurr_1()
        {
            bool amIMaster = false;

            RedisElectorProvider prov = new RedisElectorProvider(_logger, redisConfig);

            prov.SetExpirationMilliseconds(RedisElectorProvider.Recommended_ExpirationMilliseconds);

            DateTime stamp = DateTime.UtcNow;

            var agent1 = new ElectorInfo()
            {
                ApplicationName = ApplicationName,
                LastCallUtc = stamp,
                UniqueInstanceId = Guid.NewGuid().ToString()
            };

            amIMaster = prov.AmIMaster(agent1);
            Assert.IsTrue(amIMaster, "Agent 1");

            var agent2 = new ElectorInfo()
            {
                ApplicationName = ApplicationName,
                LastCallUtc = stamp,
                UniqueInstanceId = Guid.NewGuid().ToString()
            };

            amIMaster = prov.AmIMaster(agent2);
            Assert.IsFalse(amIMaster, "Agent 2");

            Thread.Sleep(RedisElectorProvider.Recommended_ExpirationMilliseconds + 1000);

            agent2.LastCallUtc = DateTime.UtcNow;

            amIMaster = prov.AmIMaster(agent2);
            Assert.IsTrue(amIMaster, "Agent 2");

            agent1.UpdateLastCallUtc();

            amIMaster = prov.AmIMaster(agent1);
            Assert.IsFalse(amIMaster, "Agent 1");

            amIMaster = prov.AmIMaster(agent1);
            Assert.IsFalse(amIMaster, "Agent 1");

            amIMaster = prov.AmIMaster(agent2);
            Assert.IsTrue(amIMaster, "Agent 2");
        }
    }
}
