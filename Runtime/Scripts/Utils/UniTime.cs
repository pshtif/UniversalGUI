/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace UniversalGUI
{
    public class UniTime
    {
        public static double timeSinceStartup
        {
            get {
#if UNITY_EDITOR
                return UnityEditor.EditorApplication.timeSinceStartup;
#else
                return Time.realtimeSinceStartupAsDouble;
#endif
            }
        }
    }
}