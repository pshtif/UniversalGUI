/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using UnityEngine;

namespace UniversalGUI
{
    public class TextureUtils
    {
        private static Dictionary<Color, Texture2D> _cachedColorTextures;
        static private Dictionary<string, Texture> _cachedResourceTextures = new Dictionary<string, Texture>();

        public static Texture GetTexture(string p_name)
        {
            if (_cachedResourceTextures.ContainsKey(p_name))
                return _cachedResourceTextures[p_name];

            Texture texture = Resources.Load<Texture>("Textures/"+p_name);
            if (texture != null)
            {
                _cachedResourceTextures.Add(p_name, texture);
            }
            else
            {
                Debug.Log("Texture not found "+p_name);
            }

            return texture;
        }
        
        public static Texture2D GetColorTexture(Color p_color, int p_width = 4, int p_height = 4)
        {
            if (_cachedColorTextures == null)
            {
                _cachedColorTextures = new Dictionary<Color, Texture2D>();
            }

            if (_cachedColorTextures.ContainsKey(p_color) && _cachedColorTextures[p_color] != null)
            {
                return _cachedColorTextures[p_color];
            }

            var tex = new Texture2D(p_width, p_height);
            var cols = tex.GetPixels();
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i] = p_color;
            }

            tex.SetPixels(cols);
            tex.Apply();

            _cachedColorTextures[p_color] = tex;
            return tex;
        }
    }
}