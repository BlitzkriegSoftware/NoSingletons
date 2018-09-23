﻿using System;

namespace BlitzLib.AssembyInfo
{
    /// <summary>
    /// Helper: Assembly Info
    /// </summary>
    public static class AssembyInfoHelper
    {
        /// <summary>
        /// Try Parse a <c>System.Reflection.CustomAttributeData</c> into a string
        /// </summary>
        /// <param name="attribute">(this)</param>
        /// <param name="s">Strng to parse into</param>
        /// <returns>True if success</returns>
        public static bool TryParse(this System.Reflection.CustomAttributeData attribute, out string s)
        {
            var flag = false;
            s = attribute.ToString();
            var i = s.IndexOf('"');
            if (i >= 0) { s = s.Substring(i + 1); flag = true; }
            i = s.IndexOf('"');
            if (i >= 0) { s = s.Substring(0, i); flag = true; }
            return flag;
        }
    }
}
