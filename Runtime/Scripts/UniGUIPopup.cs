/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using UnityEngine;

namespace UniversalGUI
{
    public static class UniGUIPopup
    {
        private static List<IPopup> _popups = new List<IPopup>();
        //private static bool _usedMouseEvent = false;

        public static void Show(IPopup p_popup)
        {
            _popups.Add(p_popup);
        }

        public static void Close(IPopup p_popup)
        {
            _popups.Remove(p_popup);
        }

        public static void HandleMouseBlocking(Rect p_rect)
        {
            if (Event.current.type != EventType.MouseDown)
                return;
            
            for (int i = 0; i < _popups.Count; i++)
            {
                var popup = _popups[i];
        
                if (!popup.GetRect().Contains(Event.current.mousePosition))
                {
                    _popups.Remove(popup);
                    i--;
                }
            }
        }
        
        public static void OnGUI(Rect p_rect)
        {
            for (int i = 0; i<_popups.Count; i++)
            {
                var popup = _popups[i];
                
                if (!popup.GetRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                {
                    _popups.Remove(popup);
                    i--;
                }
                else
                {
                    UnityEngine.GUI.Window(i, popup.GetRect(), popup.DrawGUI, GUIContent.none, UniGUI.Skin.window);
                    //popup.DrawGUI();
                }
            }
            
            // _usedMouseEvent = false;
        }
    }
}