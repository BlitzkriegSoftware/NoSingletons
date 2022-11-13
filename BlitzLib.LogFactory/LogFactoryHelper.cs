using System;
using Microsoft.Extensions.Logging;

namespace BlitzLib.LogFactory
{
    /// <summary>
    /// Log Factory Helper
    /// </summary>
    public static class LogFactoryHelper
    {

        private static ILoggerFactory _factory = null;
        
        /// <summary>
        /// Creates logger factory
        /// </summary>
        private static ILoggerFactory ConsoleLoggerFactory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = LoggerFactory.Create(builder =>
                    {
                        builder.AddFilter("Microsoft", LogLevel.Warning)
                               .AddFilter("System", LogLevel.Warning)
                               .SetMinimumLevel(LogLevel.Trace)
                               .AddConsole()
                               ;
                        
                    });
                }
                return _factory;
            }
            set { _factory = value; }
        }

        /// <summary>
        /// Create Logger
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Logger</returns>
        public static ILogger CreateLogger<T>() => ConsoleLoggerFactory.CreateLogger<T>();

    }
}
