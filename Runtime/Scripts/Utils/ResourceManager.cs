/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversalGUI
{
    public class ResourceManager
    {
        public static List<T> LoadAll<T>(string p_path) where T : UnityEngine.Object
        {
            return new List<T>(UnityEngine.Resources.LoadAll<T>(p_path));
        }
        
        public static List<UnityEngine.Object> LoadAll(string p_path, Type p_type)
        {
            return UnityEngine.Resources.LoadAll(p_path, p_type).ToList();
        }
    }
}