﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BlitzLib.Elector.Models
{
    /// <summary>
    /// Elector Info
    /// </summary>
    public class ElectorInfo
    {
        /// <summary>
        /// Application name, same across all instances, should be app name in PCF or project name
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// Unique Id of this Instance <c>Guid.NewGuid()</c>
        /// </summary>
        public string UniqueInstanceId { get; set; }
        /// <summary>
        /// DateTime of last call in UTC <c>DateTime.UtcNow</c>
        /// </summary>
        public DateTime LastCallUtc { get; set; }
        /// <summary>
        /// Method to update last call to now
        /// </summary>
        public void UpdateLastCallUtc() { this.LastCallUtc = DateTime.UtcNow;  }

        /// <summary>
        /// Debug string
        /// </summary>
        /// <returns>String version of object</returns>
        public override string ToString()
        {
            return string.Format("{0}-{1}", this.ApplicationName, this.UniqueInstanceId);
        }

    }
}
