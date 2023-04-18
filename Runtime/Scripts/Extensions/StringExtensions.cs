/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Linq;

namespace UniversalGUI
{
    public static class StringExtensions
    {
        public static string RemoveWhitespace(this string p_string)
        {
            if (p_string == null)
                return null;
            
            return new string(p_string.Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }
        
        public static bool IsNullOrWhitespace(this string p_string)
        {
            if (!string.IsNullOrEmpty(p_string))
            {
                for (int i = 0; i < p_string.Length; i++)
                {
                    if (char.IsWhiteSpace(p_string[i]) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}