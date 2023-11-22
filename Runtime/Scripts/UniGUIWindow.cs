/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace UniversalGUI
{
    public abstract class UniGUIWindow : IWindow
    {
        protected Rect _rect;

        public abstract void DrawGUI(int p_id);

        public Vector2 position => _rect.position;
        
        public virtual Rect GetRect()
        {
            return _rect;
        }
    }
}