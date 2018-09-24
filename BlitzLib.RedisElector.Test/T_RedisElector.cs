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

        private const string ApplicationName = "T_RedisElector";
        #endregion

        #region "CTOR"
        public T_RedisElector()
        {
            var text = System.IO.File.ReadAllText(@".\ElectorRedisConfig.json");
            redisConfig = JsonConvert.DeserializeObject<RedisConfiguration>(text);
        }
        #endregion


        [TestMethod]
        public void ConCurr_1()
        {
            bool amIPrimary = false;

            RedisElectorProvider prov = new RedisElectorProvider( redisConfig);

            prov.SetExpirationMilliseconds(RedisElectorProvider.Recommended_ExpirationMilliseconds);

            DateTime stamp = DateTime.UtcNow;

            var agent1 = new ElectorInfo()
            {
                ApplicationName = ApplicationName,
                LastCallUtc = stamp,
                UniqueInstanceId = Guid.NewGuid().ToString()
            };

            amIPrimary = prov.AmIPrimary(agent1);
            Assert.IsTrue(amIPrimary, "Agent 1");

            var agent2 = new ElectorInfo()
            {
                ApplicationName = ApplicationName,
                LastCallUtc = stamp,
                UniqueInstanceId = Guid.NewGuid().ToString()
            };

            amIPrimary = prov.AmIPrimary(agent2);
            Assert.IsFalse(amIPrimary, "Agent 2");

            Thread.Sleep(RedisElectorProvider.Recommended_ExpirationMilliseconds + 1000);

            agent2.LastCallUtc = DateTime.UtcNow;

            amIPrimary = prov.AmIPrimary(agent2);
            Assert.IsTrue(amIPrimary, "Agent 2");

            agent1.UpdateLastCallUtc();

            amIPrimary = prov.AmIPrimary(agent1);
            Assert.IsFalse(amIPrimary, "Agent 1");

            amIPrimary = prov.AmIPrimary(agent1);
            Assert.IsFalse(amIPrimary, "Agent 1");

            amIPrimary = prov.AmIPrimary(agent2);
            Assert.IsTrue(amIPrimary, "Agent 2");
        }
    }
}
