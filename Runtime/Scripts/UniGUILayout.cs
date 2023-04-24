/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniversalGUI
{
    public static class UniGUILayout
    {
        static public GUISkin Skin => (GUISkin)Resources.Load("RuntimeSkin");
        
        public static int labelWidth = 150;
        public static int fieldWidth = 50;

        internal static float GetStyleHeight(GUIStyle p_style)
        {
            //if (p_style == null) p_style = GUIStyle.none;
            
            return p_style.CalcHeight(new GUIContent("1"), 10);
        }
        
        private static Rect GetSliderRect (Rect p_rect)
        {
            return new Rect (p_rect.x, p_rect.y, p_rect.width - fieldWidth, p_rect.height);
        }

        private static Rect GetSliderFieldRect (Rect sliderRect)
        {
            return new Rect (sliderRect.x + sliderRect.width - fieldWidth, sliderRect.y, fieldWidth, sliderRect.height);
        }
        
        // internal static Rect GetLayoutRect(GUIContent p_label, GUIStyle p_style, params GUILayoutOption[] p_options)
        // {
        //     float minContentWidth = 0;
        //     float maxContentWidth = 0;
        //     if (p_label != GUIContent.none)
        //     {
        //         p_style.CalcMinMaxWidth(p_label, out minContentWidth, out maxContentWidth);
        //     }
        //
        //     var horizontalMargin = p_style.margin.left + p_style.margin.right;
        //     var verticalMargin = p_style.margin.top + p_style.margin.bottom;
        //     var height = GetStyleHeight(p_style);
        //     var wholeRect =  GUILayoutUtility.GetRect(fieldWidth + minContentWidth + horizontalMargin, fieldWidth + maxContentWidth + horizontalMargin, height,
        //         height + verticalMargin, p_options);
        //     
        //     var rect = new Rect(wholeRect.x + p_style.margin.left, wholeRect.y + p_style.margin.top,
        //         wholeRect.width - horizontalMargin, wholeRect.height - verticalMargin);
        //     return rect;
        // }  
        //
        // public static Rect InsertLabel(Rect p_rect, GUIContent p_label, GUIStyle p_style)
        // {
        //     if (p_label == GUIContent.none)
        //         return p_rect;
        //
        //     Rect labelPos = new Rect(p_rect.x, p_rect.y, labelWidth, p_rect.height);
        //     GUI.Label(labelPos, p_label, p_style);
        //
        //     return new Rect(p_rect.x + labelWidth, p_rect.y, p_rect.width - labelWidth - 6, p_rect.height);
        // }
        //
        // public static Rect InsertLabel (Rect p_rect, float p_ratio, GUIContent p_label, GUIStyle p_style)
        // {
        //     if (p_label == GUIContent.none)
        //         return p_rect;
        //
        //     Rect labelPos = new Rect(p_rect.x, p_rect.y, p_rect.width * p_ratio, p_rect.height);
        //     GUI.Label(labelPos, p_label, p_style);
        //
        //     return new Rect(p_rect.x + p_rect.width * p_ratio, p_rect.y, p_rect.width * (1 - p_ratio), p_rect.height);
        // }

        public static void Box(string p_label, params GUILayoutOption[] p_options)
        {
            GUILayout.Box(p_label, Skin.box, p_options);
        }
        
        public static bool Button(string p_label, params GUILayoutOption[] p_options)
        {
            return Button(new GUIContent(p_label), Skin.button, p_options);
        }
        
        public static bool Button(string p_label, GUIStyle p_style = null, params GUILayoutOption[] p_options)
        {
            return Button(new GUIContent(p_label), p_style, p_options);
        }

        public static bool Button(GUIContent p_label, GUIStyle p_style = null, params GUILayoutOption[] p_options)
        {
            var style = p_style == null ? Skin.button : p_style;
            if (GUILayout.Button(p_label, style, p_options))
            {
                GUIUtility.keyboardControl = -1;
                UniGUI.changed = true;
                return true;
            }
            
            return false;
        }

        public static int Popup(GUIContent p_label, int p_selectedIndex, string[] p_items, params GUILayoutOption[] p_options)
        {
#if UNITY_EDITOR && USE_EDITORGUI
            return UnityEditor.EditorGUILayout.Popup(p_label, p_selectedIndex, p_items, p_options);
#else
            if (p_label != GUIContent.none)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(p_label, GUILayout.Width(labelWidth));
            }

            int thisId = GUIUtility.GetControlID("Popup".GetHashCode(), FocusType.Keyboard) + 1;
            if (thisId == 0)
            {
                if (p_label != GUIContent.none)
                {
                    GUILayout.EndHorizontal();
                }

                return p_selectedIndex;
            }

            if (thisId == UniGUI.currentShowingPopup && UniGUI.currentShowingPopupIndex >= 0)
            {
                p_selectedIndex = UniGUI.currentShowingPopupIndex;
                UniGUI.changed = true;
                UniGUI.currentShowingPopup = -1;
                UniGUI.currentShowingPopupIndex = -1;
            }

            var style = Skin.GetStyle("enumpopupfield");
            if (GUILayout.Button(p_items[p_selectedIndex], style))
            {
                GUIUtility.keyboardControl = -1;
                UniGUI.changed = true;
                UniGUI.currentShowingPopup = thisId;
                ShowPopup(p_selectedIndex, p_items);
            }
            
            var rect = GUILayoutUtility.GetLastRect();
            var size = rect.height - 8;
            UnityEngine.GUI.DrawTexture(new Rect(rect.x + rect.width - size - 4, rect.y + 4, size, size), TextureUtils.GetTexture("enumpopuparrow"), ScaleMode.StretchToFill);

            if (p_label != GUIContent.none)
            {
                GUILayout.EndHorizontal();
            }

            return p_selectedIndex;
#endif
        }
        
        private static void ShowPopup(int p_selectedIndex, string[] p_items)
        {
            UniGUIGenericMenu menu = new UniGUIGenericMenu();
            
            int i = 0;
            foreach (var item in p_items)
            {
                int local = i;
                menu.AddItem(new GUIContent(item), i == p_selectedIndex, () => { UniGUI.currentShowingPopupIndex = local; });
                i++;
            }

#if UNITY_EDITOR
            UniGUIGenericMenuPopup.Show(menu, "", Event.current.mousePosition, 240, 300, false, false, true);
#else
            UniGUIGenericMenuPopup.Show(menu, "", GUIUtility.GUIToScreenPoint(Event.current.mousePosition), 240, 300, false, false, true);
#endif
        }
        
        public static Enum EnumPopup(Enum p_value)
        {
            return EnumPopup(GUIContent.none, p_value);
        }

        public static Enum EnumPopup(string p_label, Enum p_value)
        {
            return EnumPopup(new GUIContent(p_label), p_value);
        }
        
        public static Enum EnumPopup(GUIContent p_label, Enum p_value)
        {
#if UNITY_EDITOR && USE_EDITORGUI
            return UnityEditor.EditorGUILayout.EnumPopup(p_label, p_value);
#else
            if (p_label != GUIContent.none)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(p_label, GUILayout.Width(labelWidth));
            }

            int thisId = GUIUtility.GetControlID("EnumPopup".GetHashCode(), FocusType.Keyboard) + 1;
            if (thisId == 0)
            {
                if (p_label != GUIContent.none)
                {
                    GUILayout.EndHorizontal();
                }

                return p_value;
            }

            if (thisId == UniGUI.currentShowingPopup && UniGUI.currentShowingPopupIndex >= 0)
            {
                p_value = (Enum)Enum.ToObject(p_value.GetType(), UniGUI.currentShowingPopupIndex);
                UniGUI.changed = true;
                UniGUI.currentShowingPopup = -1;
                UniGUI.currentShowingPopupIndex = -1;
            }

            var style = Skin.GetStyle("enumpopupfield");
            if (GUILayout.Button(Enum.GetName(p_value.GetType(), p_value), style))
            {
                GUIUtility.keyboardControl = -1;
                UniGUI.changed = true;
                UniGUI.currentShowingPopup = thisId;
                ShowEnumPopup(p_value);
            }

            var rect = GUILayoutUtility.GetLastRect();
            var size = rect.height - 8;
            UnityEngine.GUI.DrawTexture(new Rect(rect.x + rect.width - size - 4, rect.y + 4, size, size), TextureUtils.GetTexture("enumpopuparrow"), ScaleMode.StretchToFill);

            if (p_label != GUIContent.none)
            {
                GUILayout.EndHorizontal();
            }
            
            return p_value;
#endif
        }

        private static void ShowEnumPopup(Enum p_value)
        {
            UniGUIGenericMenu menu = new UniGUIGenericMenu();

            int index = Array.IndexOf(Enum.GetValues(p_value.GetType()), p_value);

            foreach (int i in Enum.GetValues(p_value.GetType()))
            {
                menu.AddItem(new GUIContent(Enum.GetName(p_value.GetType(), i)), i == index,
                    () =>
                    {
                        UniGUI.currentShowingPopupIndex = i;
                    });
            }
            
#if UNITY_EDITOR
            UniGUIGenericMenuPopup.Show(menu, "", Event.current.mousePosition, 240, 300, false, false, true);
#else
            UniGUIGenericMenuPopup.Show(menu, "", GUIUtility.GUIToScreenPoint(Event.current.mousePosition), 240, 300, false, false, true);
#endif
        }

        public static bool Toggle(bool p_value, params GUILayoutOption[] p_options)
        {
            return Toggle(GUIContent.none, p_value, p_options);
        }

        public static bool Toggle(string p_label, bool p_value, params GUILayoutOption[] p_options)
        {
            return Toggle(p_label == "" ? GUIContent.none : new GUIContent(p_label), p_value, p_options);
        }

        public static bool Toggle(GUIContent p_label, bool p_value, params GUILayoutOption[] p_options)
        {
#if UNITY_EDITOR && USE_EDITORGUI
            return UnityEditor.EditorGUILayout.Toggle(p_label, p_value, p_options);
#else
            int thisId = GUIUtility.GetControlID("Toggle".GetHashCode(), FocusType.Keyboard);
            Event current = Event.current;
            EventType type = current.type;
            bool flag1 = current.type == EventType.MouseDown && current.button != 0;
            if (flag1)
            {
                current.type = EventType.Ignore;
            }

            GUILayout.BeginHorizontal();
            if (p_label != GUIContent.none)
            {
                GUILayout.Label(p_label, GUILayout.Width(labelWidth));
            }

            var boolValue = GUILayout.Toggle((bool) p_value, "", Skin.toggle);
            
            GUILayout.EndHorizontal();
            
            if (flag1)
                current.type = type;
            else if (current.type != type)
                GUIUtility.keyboardControl = thisId;

            if (boolValue != p_value)
            {
                UniGUI.changed = true;
            }

            return boolValue;
#endif
        }

        public static void Label(string p_label, params GUILayoutOption[] p_options)
        {
            Label(new GUIContent(p_label), p_options);
        }

        public static void Label(string p_label, GUIStyle p_style = null, params GUILayoutOption[] p_options)
        {
            Label(new GUIContent(p_label), p_style, p_options);
        }
        
        public static void Label(GUIContent p_label, params GUILayoutOption[] p_options)
        {
            Label(p_label, null, p_options);
        }
        
        public static void Label(GUIContent p_label, GUIStyle p_style = null, params GUILayoutOption[] p_options)
        {
            var labelStyle = p_style == null ? Skin.label : p_style;

            GUILayout.Label(p_label, labelStyle, p_options);
        }

        public static void BeginHorizontal(params GUILayoutOption[] p_options)
        {
            GUILayout.BeginHorizontal(p_options);
        }

        public static void EndHorizontal()
        {
            GUILayout.EndHorizontal();
        }
        
        public static int IntField (string p_label, int p_value, params GUILayoutOption[] p_options)
        {
            return (int)FloatField (new GUIContent (p_label), p_value, p_options);
        }
        
        public static int IntField (GUIContent p_label, int value, params GUILayoutOption[] options)
        {
            return (int)FloatField(p_label, value, options);
        }
        
        public static int IntField (int p_value, params GUILayoutOption[] p_options)
        {
            return (int)FloatField(p_value, p_options);
        }

        public static float FloatField(string p_label, float p_value, params GUILayoutOption[] p_options)
        {
            return FloatField(new GUIContent(p_label), p_value, p_options);
        }

        public static float FloatField(GUIContent p_label, float p_value, params GUILayoutOption[] p_options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(p_label, GUILayout.Width(labelWidth));

            var value = FloatField(p_value, p_options);
            GUILayout.EndHorizontal();
            return value;
        }
        
        public static float FloatField (float p_value, params GUILayoutOption[] p_options)
        {
            int thisId = GUIUtility.GetControlID("FloatField".GetHashCode(), FocusType.Keyboard) + 1;
            if (thisId == 0)
                return p_value;
            
            bool current = UniGUI.currentId == thisId;
            
            string floatString = current ? UniGUI.currentEditingString : p_value.ToString();
            
            floatString = GUILayout.TextField(floatString, Skin.textField, p_options);
            if (current) UniGUI.currentEditingString = floatString;

            if (floatString != "" && floatString != p_value.ToString())
            {
                float newValue;
                if (float.TryParse(floatString, NumberStyles.Float, CultureInfo.InvariantCulture, out newValue) && p_value != newValue)
                {
                    UniGUI.changed = true;
                    p_value = newValue;
                }
            }

            if (UniGUI.IsActiveControl(thisId) && !current)
            {
                UniGUI.currentId = thisId;
                UniGUI.currentEditingString = floatString;
            }

            if (!UniGUI.IsActiveControl(thisId) && current)
            {
                UniGUI.currentId = -1;
            }

            return p_value;
        }

        public static string TextField(string p_text, params GUILayoutOption[] p_options)
        {
            return TextField(p_text, Skin.textField, p_options);
        }
        
        public static string TextField(string p_text, GUIStyle p_style, params GUILayoutOption[] p_options)
        {
            var newText = GUILayout.TextField(p_text, p_style, p_options);
             
            if (newText != p_text)
                UniGUI.changed = true;

            return newText;
        }
        
        public static string TextField(string p_label, string p_text, params GUILayoutOption[] p_options)
        {
            return TextField(p_label, p_text, Skin.textField, p_options);
        }
        
        public static string TextField(string p_label, string p_text, GUIStyle p_style, params GUILayoutOption[] p_options)
        {
            GUILayout.BeginHorizontal();
            
            GUILayout.Label(p_label, GUILayout.Width(labelWidth));

            var text = TextField(p_text, p_style, p_options);
            
            GUILayout.EndHorizontal();

            return text;
        }
        
        public static string PasswordField(string p_text, params GUILayoutOption[] p_options)
        {
            return PasswordField(p_text, Skin.textField, p_options);
        }
        
        public static string PasswordField(string p_text, GUIStyle p_style, params GUILayoutOption[] p_options)
        {
            if (p_style == null)
                p_style = Skin.textField;
            
            var newText = GUILayout.PasswordField(p_text, '*', p_style, p_options);
             
            if (newText != p_text)
                UniGUI.changed = true;

            return newText;
        }
        
        public static string PasswordField(string p_label, string p_text, params GUILayoutOption[] p_options)
        {
            return PasswordField(p_label, p_text, Skin.textField, p_options);
        }
        
        public static string PasswordField(string p_label, string p_text, GUIStyle p_style, params GUILayoutOption[] p_options)
        {
            GUILayout.BeginHorizontal();
            
            GUILayout.Label(p_label, GUILayout.Width(labelWidth));

            var text = PasswordField(p_text, p_style, p_options);
            
            GUILayout.EndHorizontal();

            return text;
        }
        
        public static Vector3 Vector2Field(string p_label, Vector2 p_value, params GUILayoutOption[] p_options)
        {
            return Vector2Field(new GUIContent(p_label), p_value, p_options);
        }
        
        public static Vector2 Vector2Field(GUIContent p_label, Vector2 p_value, params GUILayoutOption[] p_options)
        {
#if UNITY_EDITOR && USE_EDITORGUI
            return UnityEditor.EditorGUILayout.Vector2Field(p_label, p_value, p_options);
#else
            GUILayout.BeginHorizontal(p_options);
            if (!p_label.text.IsNullOrWhitespace())
            {
                GUILayout.Label(p_label, GUILayout.Width(labelWidth));
            }
            
            var oldLabelWidth = labelWidth;
            labelWidth = 11;
            var valueX = FloatField("X",p_value.x);
            var valueY = FloatField("Y",p_value.y);
            p_value = new Vector2(valueX, valueY);
            labelWidth = oldLabelWidth;
            
            GUILayout.EndHorizontal();
            return p_value;
#endif
        }

        public static Vector3 Vector3Field(string p_label, Vector3 p_value, params GUILayoutOption[] p_options)
        {
            return Vector3Field(new GUIContent(p_label), p_value, p_options);
        }
        
        public static Vector3 Vector3Field(GUIContent p_label, Vector3 p_value, params GUILayoutOption[] p_options)
        {
#if UNITY_EDITOR && USE_EDITORGUI
            return UnityEditor.EditorGUILayout.Vector3Field(p_label, p_value, p_options);
#else
            GUILayout.BeginHorizontal(p_options);
            if (!p_label.text.IsNullOrWhitespace())
            {
                GUILayout.Label(p_label, GUILayout.Width(labelWidth));
            }
            
            var oldLabelWidth = labelWidth;
            labelWidth = 11;
            var valueX = FloatField("X", p_value.x, GUILayout.ExpandWidth(true));
            var valueY = FloatField("Y", p_value.y, GUILayout.ExpandWidth(true));
            var valueZ = FloatField("Z", p_value.z, GUILayout.ExpandWidth(true));
            p_value = new Vector3(valueX, valueY, valueZ);
            labelWidth = oldLabelWidth;
            GUILayout.EndHorizontal();
            return p_value;
#endif
        }
        
        // This is a hack to send max width directly instead of evaluating GUILayout options using reflection due to its type being internal to Unity
        public static Vector3 Vector3Field(GUIContent p_label, Vector3 p_value, float p_maxWidth, params GUILayoutOption[] p_options)
        {
#if UNITY_EDITOR && USE_EDITORGUI
            return UnityEditor.EditorGUILayout.Vector3Field(p_label, p_value, p_options);
#else
            GUILayout.BeginHorizontal(p_options);
            if (!p_label.text.IsNullOrWhitespace())
            {
                GUILayout.Label(p_label, GUILayout.Width(labelWidth));
            }

            var oldLabelWidth = labelWidth;
            labelWidth = 11;
            float valueX, valueY, valueZ;
            
            if (p_maxWidth > 0)
            {
                valueX = FloatField("X", p_value.x, GUILayout.MaxWidth((p_maxWidth - 45) / 3),
                    GUILayout.ExpandWidth(true));
                valueY = FloatField("Y", p_value.y, GUILayout.MaxWidth((p_maxWidth - 45) / 3),
                    GUILayout.ExpandWidth(true));
                valueZ = FloatField("Z", p_value.z, GUILayout.MaxWidth((p_maxWidth - 45) / 3),
                    GUILayout.ExpandWidth(true));
            }
            else
            {
                valueX = FloatField("X", p_value.x, GUILayout.ExpandWidth(true));
                valueY = FloatField("Y", p_value.y, GUILayout.ExpandWidth(true));
                valueZ = FloatField("Z", p_value.z, GUILayout.ExpandWidth(true));
            }

            p_value = new Vector3(valueX, valueY, valueZ);
            labelWidth = oldLabelWidth;
            GUILayout.EndHorizontal();
            return p_value;
#endif
        }

        // private static int GetMaxWidth(GUILayoutOption[] p_options)
        // {
        //     for (int i = 0; i < p_options.Length; i++)
        //     {
        //         var option = p_options[i];
        //         if (option.type == GUILayoutOption.Type.maxWidth)
        //         {
        //             return (int) option.value;
        //         }
        //     }
        //
        //     return 0;
        // }

        public static Vector4 Vector4Field(string p_label, Vector4 p_value, params GUILayoutOption[] p_options)
        {
            return Vector4Field(new GUIContent(p_label), p_value, p_options);
        }
        
        public static Vector4 Vector4Field(GUIContent p_label, Vector4 p_value, params GUILayoutOption[] p_options)
        {
#if UNITY_EDITOR && USE_EDITORGUI
            return UnityEditor.EditorGUILayout.Vector4Field(p_label, p_value, p_options);
#else
            GUILayout.BeginHorizontal(p_options);
            if (!p_label.text.IsNullOrWhitespace())
            {
                GUILayout.Label(p_label, GUILayout.Width(labelWidth));
            }
            
            var oldLabelWidth = labelWidth;
            labelWidth = 11;
            var valueX = FloatField("X",p_value.x);
            var valueY = FloatField("Y",p_value.y);
            var valueZ = FloatField("Z",p_value.z);
            var valueW = FloatField("W",p_value.w);
            labelWidth = oldLabelWidth;
            p_value = new Vector4(valueX, valueY, valueZ, valueW);
            
            GUILayout.EndHorizontal();
            return p_value;
#endif
        }
        
        public static Color ColorField(Color p_value, params GUILayoutOption[] p_options)
        {
#if UNITY_EDITOR && USE_EDITORGUI
            return UnityEditor.EditorGUILayout.ColorField(p_value);
#else
            int thisId = GUIUtility.GetControlID("ColorField".GetHashCode(), FocusType.Keyboard) + 1;
            if (thisId == 0)
                return p_value;
            
            bool current = UniGUI.currentId == thisId;
            
            string colorString = current ? UniGUI.currentEditingString : "#"+ColorUtility.ToHtmlStringRGBA(p_value);

            colorString = GUILayout.TextField(colorString, Skin.textField, p_options);
            if (current) UniGUI.currentEditingString = colorString;

            if (colorString != "" && colorString != ColorUtility.ToHtmlStringRGBA(p_value))
            {
                Color newValue;
                if (ColorUtility.TryParseHtmlString(colorString, out newValue) && p_value != newValue)
                {
                    UniGUI.changed = true;
                    p_value = newValue;
                }
            }

            if (UniGUI.IsActiveControl(thisId) && !current)
            {
                UniGUI.currentId = thisId;
                UniGUI.currentEditingString = colorString;
            }

            if (!UniGUI.IsActiveControl(thisId) && current)
            {
                UniGUI.currentId = -1;
            }

            return p_value;
#endif
        }
        
        public static Object ObjectField(Object p_value, Type p_type, bool p_sceneObject, params GUILayoutOption[] p_options)
        {
#if UNITY_EDITOR && USE_EDITORGUI
            return UnityEditor.EditorGUILayout.ObjectField(p_value, p_type, p_sceneObject, p_options);
#else
            List<UnityEngine.Object> objectList = ResourceManager.LoadAll("", p_type);
            objectList.Insert(0, null);
            var index = objectList.IndexOf(p_value);
            var nameList = objectList.Select(o => o == null ? "none" : o.name).ToArray();
            var newIndex = Popup(GUIContent.none, index, nameList, p_options);
            if (newIndex != index)
            {
                p_value = objectList[newIndex];
                UniGUI.changed = true;
            }
            return p_value;
#endif
        }

        public static void Space(int p_width, bool p_expand = false)
        {
#if UNITY_EDITOR && USE_EDITORGUI
            UnityEditor.EditorGUILayout.Space(p_width, p_expand);
#else
            // Not working as we would need to internally add it to layer cache which is not accessible in Unity :/
            GUILayoutUtility.GetRect(p_width, p_width, GUILayout.ExpandWidth(p_expand));
#endif
        }

        // public static float Slider(GUIContent p_label, float p_value, float p_minValue, float p_maxValue, params GUILayoutOption[] p_options) 
        // {
        //     Rect rect = GetLayoutRect (p_label, Skin.GetStyle("WhiteLabel"), p_options);
        //     Rect fieldRect = InsertLabel(rect, 0.5f, p_label, Skin.GetStyle("WhiteLabel"));
        //
        //     p_value = GUI.HorizontalSlider(GetSliderRect(fieldRect), p_value, p_minValue, p_maxValue, Skin.GetStyle("HorizontalSlider"), Skin.GetStyle("HorizontalSliderThumb"));
        //     //p_value = Mathf.Min (p_maxValue, Mathf.Max (p_minValue, FloatField(GetSliderFieldRect(fieldRect), p_value)));
        //     return p_value;
        // }
    }
}