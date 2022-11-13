using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlitzLib.RedisElector.Models
{
    public class RedisConfiguration
    {
        public string RedisConnectionString { get; set; }

        public bool IsValid => !string.IsNullOrEmpty(RedisConnectionString);
    }
}
