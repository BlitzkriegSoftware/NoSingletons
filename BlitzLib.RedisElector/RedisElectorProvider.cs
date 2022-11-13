using BlitzLib.Elector;
using BlitzLib.Elector.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace BlitzLib.RedisElector
{
    public class RedisElectorProvider : IElectorProvider
    {
        private RedisElectorProvider() { }

        private Models.RedisConfiguration _config;

        private const string NO_INSTANCE = "(n/a)";

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="config">RedisConfiguration</param>
        public RedisElectorProvider(Models.RedisConfiguration config)
        {
            if (config == null) throw new ArgumentNullException("config");
            if (!config.IsValid) throw new InvalidOperationException("A Valid Redis configuration is required");
            _config = config;
        }

        /// <summary>
        /// Am I Primary? 
        /// </summary>
        /// <param name="info">ElectorInfo</param>
        /// <returns>True, if so</returns>
        public bool AmIPrimary(ElectorInfo info)
        {
            bool amI = false;

            ConnectionMultiplexer redis;
            IDatabase db;
            ITransaction trans;

            DateTime stamp = DateTime.UtcNow;

            ElectorInfo otherInfo;

            int expireMS = GetExpirationMilliseconds();

            try
            {
                redis = ConnectionMultiplexer.Connect(this._config.RedisConnectionString, (opts) =>
                {
                    opts.IncludeDetailInExceptions = true;
                    opts.ClientName = "RedisElectorProvider";
                    opts.AllowAdmin = true;
                    opts.ConnectTimeout = 20;
                    opts.AsyncTimeout = 10;

                });
                
                db = redis.GetDatabase();

                trans = db.CreateTransaction();

                var text = db.StringGet(info.ApplicationName);
                if (string.IsNullOrWhiteSpace(text)) otherInfo = info;
                else otherInfo = JsonConvert.DeserializeObject<ElectorInfo>(text);

                if (otherInfo.UniqueInstanceId == info.UniqueInstanceId)
                {
                    UpdateKey(db, info, stamp);
                    amI = true;
                }
                else
                {
                    var ts = stamp - otherInfo.LastCallUtc;
                    var age = ts.TotalMilliseconds;
                    if (age > expireMS)
                    {
                        UpdateKey(db, info, stamp);
                        amI = true;
                    }
                }

                trans.Execute();
            }

            finally
            {
                db = null;
                trans = null;
                redis = null;
            }

            return amI;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        public bool ForceElection(string applicationName)
        {
            ConnectionMultiplexer redis;
            IDatabase db;
            ITransaction trans;

            try
            {
                redis = ConnectionMultiplexer.Connect(this._config.RedisConnectionString, (opts) =>
                {
                    opts.IncludeDetailInExceptions = true;
                    opts.ClientName = "RedisElectorProvider";
                    opts.AllowAdmin = true;
                    opts.ConnectTimeout = 20;
                    opts.AsyncTimeout = 10;

                });
                
                db = redis.GetDatabase();

                trans = db.CreateTransaction();

                DateTime stamp = DateTime.UtcNow.AddDays(-1);
                var info = new ElectorInfo() { LastCallUtc = stamp, ApplicationName = applicationName, UniqueInstanceId = NO_INSTANCE };
                UpdateKey(db, info, stamp);

                trans.Execute();
            }
            finally
            {
                db = null;
                trans = null;
                redis = null;
            }

            return true;
        }

        private void UpdateKey(IDatabase db, ElectorInfo info, DateTime stamp)
        {
            info.LastCallUtc = stamp;
            var text = JsonConvert.SerializeObject(info);
            db.KeyDelete(info.ApplicationName);
            db.StringSet(info.ApplicationName, text);
        }

        /// <summary>
        /// Recommended: Expiration Tolerance (30 seconds)
        /// </summary>
        public const int Recommended_ExpirationMilliseconds = 1000 * 30;

        /// <summary>
        /// Minimum expiration tolerance
        /// </summary>
        public const int Minimim_ExpirationMilliseconds = 1000;

        private int _expirationTolerance;

        public void SetExpirationMilliseconds(int milliseconds)
        {
            _expirationTolerance = milliseconds;
            if (_expirationTolerance <= 0) _expirationTolerance = Minimim_ExpirationMilliseconds;
        }

        public int GetExpirationMilliseconds()
        {
            return _expirationTolerance;
        }

    }
}
