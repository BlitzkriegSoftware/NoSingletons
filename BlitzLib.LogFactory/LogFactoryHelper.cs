using System;
using Microsoft.Extensions.Logging;

namespace BlitzLib.LogFactory
{
    /// <summary>
    /// Log Factory Helper
    /// </summary>
    public static class LogFactoryHelper
    {

        private static ILoggerFactory _Factory = null;

        /// <summary>
        /// Configure Logger
        /// </summary>
        /// <param name="factory">ILoggerFactory</param>
        private static void ConfigureLogger(ILoggerFactory factory)
        {
            factory.AddConsole();
        }

        /// <summary>
        /// Creates logger factory
        /// </summary>
        private static ILoggerFactory LoggerFactory
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

        /// <summary>
        /// Create Logger
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Logger</returns>
        public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();

    }
}
