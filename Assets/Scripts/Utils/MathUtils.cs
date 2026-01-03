using UnityEngine;

namespace Utils
{
    public static class MathUtils
    {
        public static Vector3 ToVector3SetY(Vector2 vec, float y)
        {
            return new Vector3(vec.x, y, vec.y);
        }

        public static Vector2 ToVector2DelY(Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }
        
        /// <summary>
        /// 颜色按 HSV 空间插值
        /// </summary>
        /// <param name="a">颜色A</param>
        /// <param name="b">颜色B</param>
        /// <param name="t">插值比例</param>
        /// <returns></returns>
        public static Color ColorHSVInterpClamped(Color a, Color b, float t)
        {
            float aH, aS, aV, bH, bS, bV;
            Color.RGBToHSV(a, out aH, out aS, out aV);
            Color.RGBToHSV(b, out bH, out bS, out bV);
            
            t = Mathf.Clamp01(t);
            
            float deltaH = bH - aH;
            if (deltaH > 0.5f)
            {
                deltaH -= 1.0f;
            }
            else if (deltaH < -0.5f)
            {
                deltaH += 1.0f;
            }

            float h = aH + deltaH * t;
            if (h < 0.0f)
            {
                h += 1.0f;
            }
            else if (h > 1.0f)
            {
                h -= 1.0f;
            }
            
            float s = aS + (bS - aS) * t;
            float v = aV + (bV - aV) * t;
            
            return Color.HSVToRGB(h, s, v);
        }        
    }
}