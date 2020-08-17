/* Copyright (c) 2020 Scott Tongue
 *
 * 
 * Heavily modifed based on Marc Teyssier Hue Light intergration
 * https://github.com/marcteys/unity-hue
 * 
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.THE SOFTWARE SHALL 
 * NOT BE USED IN ANY ABLEISM WAY.
 */

using UnityEngine;

namespace Hue
{
    public static class HSVFromRGB
    {
        #region Public

        public static Vector3 ConvertToHSV(Color rgb)
        {
            float max = Mathf.Max(rgb.r, Mathf.Max(rgb.g, rgb.b));
            float min = Mathf.Min(rgb.r, Mathf.Min(rgb.g, rgb.b));

            float brightness = rgb.a;

            float hue, saturation;
            if (max == min)
            {
                hue = 0f;
                saturation = 0f;
            }
            else
            {
                float c = max - min;
                if (max == rgb.r)
                {
                    hue = (rgb.g - rgb.b) / c;
                }
                else if (max == rgb.g)
                {
                    hue = (rgb.b - rgb.r) / c + 2f;
                }
                else
                {
                    hue = (rgb.r - rgb.g) / c + 4f;
                }

                hue *= 60f;
                if (hue < 0f)
                {
                    hue += 360f;
                }

                saturation = c / max;
            }

            return new Vector3(hue, saturation, brightness);
        }
        #endregion
    }

}
