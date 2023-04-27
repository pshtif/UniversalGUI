/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using UnityEngine;

namespace UniversalGUI
{
    public static class UniGUI
    {
        public static string VERSION = "0.1.0";
        
        public static GUISkin Skin => (GUISkin)Resources.Load("Skins/UniversalSkin");
        
        private static readonly Stack<bool> _changedStack = new Stack<bool>();
        
        public static bool changed = false;
        
        internal static string currentEditingString = "";
        internal static int currentId = -1;
        internal static int currentShowingPopup = -1;
        internal static int currentShowingPopupIndex = -1;

        public static void FocusControl(string p_name)
        {
            currentId = -1;
            currentEditingString = "";
            GUIUtility.keyboardControl = -1;
            UnityEngine.GUI.FocusControl(p_name);
        }
        
        internal static bool IsActiveControl(int p_id)
        {
            return GUIUtility.keyboardControl == p_id;
        }
        
        public static void BeginChangeCheck()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorGUI.BeginChangeCheck();
            #endif
            _changedStack.Push(changed);
            changed = false;
        }

        public static bool EndChangeCheck()
        {
            if (_changedStack.Count == 0)
            {
                changed = true;
                return true;
            }

            bool currentChanged = changed;
            #if UNITY_EDITOR
            currentChanged |= UnityEditor.EditorGUI.EndChangeCheck();
            #endif
            changed |= _changedStack.Pop();
            return currentChanged;
        }

        public static string NicifyString(string p_string)
        {
            string nicifed = ""; 
            for(int i = 0; i < p_string.Length; i++)
            {
                if (p_string[i] == '_')
                    continue;

                if(char.IsUpper(p_string[i]) && i != 0)
                {
                    nicifed += " ";
                }
                
                nicifed += p_string[i];
            }

            return nicifed;
        }

        public static void Box(Rect p_rect, Color p_color)
        {
            UnityEngine.GUI.Box(p_rect, TextureUtils.GetColorTexture(p_color));
        }

        public static bool Button(Rect p_rect, GUIContent p_content, GUIStyle p_style = null)
        {
            if (p_style == null) p_style = Skin.button;
            return UnityEngine.GUI.Button(p_rect, p_content, p_style);
        }
        
        public static bool Toggle(Rect p_rect, bool p_value)
        {
            int thisId = GUIUtility.GetControlID("Toggle".GetHashCode(), FocusType.Keyboard);
            Event current = Event.current;
            EventType type = current.type;
            bool flag1 = current.type == EventType.MouseDown && current.button != 0;
            if (flag1)
            {
                current.type = EventType.Ignore;
            }

            var boolValue = UnityEngine.GUI.Toggle(p_rect, (bool) p_value, "", Skin.toggle);

            if (flag1)
                current.type = type;
            else if (current.type != type)
                GUIUtility.keyboardControl = thisId;

            if (boolValue != p_value)
            {
                UniGUI.changed = true;
            }

            return boolValue;
        }
    }
}