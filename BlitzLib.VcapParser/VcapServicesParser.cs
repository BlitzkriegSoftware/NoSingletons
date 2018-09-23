using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;

namespace BlitzLib.VcapParser
{
    /// <summary>
    /// VCAP_SERVICES Parser
    /// </summary>
    public static class VcapServicesParser
    {

        /// <summary>
        /// Parse <c>VCAP_SERVICES</c> for a list of <c>Credential</c> properties
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="serviceName">Service Name from JSON <c>p-redis</c></param>
        /// <param name="localFilename">(optional) local filename in json format, same as envirnonment variable</param>
        /// <returns>Dictionary of names (lowercase) and values (as cased)</returns>
        public static Dictionary<string, string> GetSettings(ILogger logger, string serviceName, string localFilename = "")
        {
            var d = new Dictionary<string, string>();

            var vcap_services = Environment.GetEnvironmentVariable("VCAP_SERVICES");
            if (string.IsNullOrWhiteSpace(vcap_services) && !string.IsNullOrWhiteSpace(localFilename))
            {
                vcap_services = System.IO.File.ReadAllText(localFilename);
                if (logger != null) logger.LogWarning("VCAP_SERVICES: Falling back to JSON file");
            }

            if(!string.IsNullOrWhiteSpace(vcap_services))
            {
                if (logger != null) logger.LogTrace("VCAP_SERVICES: {0} -> {1}", serviceName, vcap_services);

                JObject jo = JObject.Parse(vcap_services);
                var svr = jo.SelectToken("$.." + serviceName);

                if (svr != null)
                {
                    var credsTokens = svr.SelectToken("$..credentials");
                    var list = credsTokens.Children();
                    foreach (var item in list)
                    {
                        var key = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToLowerInvariant();
                        var value = ((Newtonsoft.Json.Linq.JProperty)item).Value.ToString();

                        d.Add(key, value);
                    }
                }

            }
            return d;
        }

    }

}
