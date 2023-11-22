/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using UnityEngine;

namespace UniversalGUI
{
    public class UniGUIWindowManager
    {
        private static List<IWindow> _windows = new List<IWindow>();

        public static void Show(IWindow p_window)
        {
            _windows.Add(p_window);
        }

        public static void Close(IWindow p_window)
        {
            _windows.Remove(p_window);
        }

        public static void HandleMouseBlocking(Rect p_rect)
        {
            
        }
        
        public static void OnGUI(Rect p_rect)
        {
            for (int i = 0; i<_windows.Count; i++)
            {
                var window = _windows[i];
                
                UnityEngine.GUI.Window(i, window.GetRect(), window.DrawGUI, GUIContent.none, UniGUI.Skin.window);
            }
        }
    }
}