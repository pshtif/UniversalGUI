using UnityEngine;

namespace UniversalGUI
{
    public abstract class UniGUIRenderer : MonoBehaviour
    {
        public bool drawBackground = false;
        public Color backgroundColor = Color.black;

        public bool useCustomRect = false;
        public Rect customRect;

        protected Rect GetRect()
        {
            if (useCustomRect)
            {
                return customRect;
            }
            
            return new Rect(0, 0, Screen.width, Screen.height);
        }
        
        protected virtual void OnGUI()
        {
            UnityEngine.GUI.skin = UniversalGUI.UniGUI.Skin;

            var rect = GetRect();
            UniGUIPopup.HandleMouseBlocking(rect);
            
            if (drawBackground)
            {
                UnityEngine.GUI.DrawTexture(rect, TextureUtils.GetColorTexture(backgroundColor));
            }

            GUILayout.BeginArea(rect);

            OnGUIInternal(rect);            
            
            GUILayout.EndArea();
            UniGUIPopup.OnGUI(rect);
        }

        protected abstract void OnGUIInternal(Rect p_rect);
    }
}