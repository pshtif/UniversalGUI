/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;

namespace UniversalGUI
{
    public interface IPopup
    {
        void DrawGUI(int p_id);
        Rect GetRect();
    }
}