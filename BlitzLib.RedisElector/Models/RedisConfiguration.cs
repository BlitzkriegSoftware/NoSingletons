using System;
using System.Collections.Generic;
using System.Text;

namespace BlitzLib.RedisElector.Models
{
    /// <summary>
    /// Configuration: REDIS
    /// </summary>
    public class RedisConfiguration
    {
        /// <summary>
        /// Host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// REDIS Default Port  e.g. <c>6379</c>
        /// </summary>
        public const int Port_Default = 6379;

        private int _port = Port_Default;

        /// <summary>
        /// Port
        /// <para>Defaults to <c>Port_Default</c></para>
        /// </summary>
        public int Port
        {
            get
            {
                if (_port <= 0) _port = Port_Default;
                return _port;
            }
            set
            {
                _port = value;
                if (_port <= 0) _port = Port_Default;
            }
        }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// DB Name as a string (should be -1,0,...)
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// Redis ready version of <c>DbName</c>
        /// </summary>
        public int DbIndex
        {
            get
            {
                int index = -1;
                if (!int.TryParse(this.DbName, out index)) index = -1;
                return index;
            }
        }

        /// <summary>
        /// Is Redis Config Valid
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Host);
            }
        }

        /// <summary>
        /// Is REDIS connect secured
        /// </summary>
        public bool IsSecured
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Password);
            }
        }

        /// <summary>
        /// ToString()
        /// </summary>
        /// <returns>Debug String</returns>
        public override string ToString()
        {
            return string.Format("Redis {0}:{1}, Secure: {2}, Valid: {3}, DB: {4}", this.Host, this.Port, this.IsSecured, this.IsValid, this.DbName);
        }

        /// <summary>
        /// Returns a REDIS connection string ready for Multiplexer
        /// </summary>
        /// <returns>Redis Connection String</returns>
        public string RedisConnectionString()
        {
            var redisConn = string.Format("{0}:{1},password={2}", this.Host, this.Port, this.Password);
            if (string.IsNullOrEmpty(this.Password)) redisConn = string.Format("{0}:{1}", this.Host, this.Port);
            return redisConn;
        }

    }

}
