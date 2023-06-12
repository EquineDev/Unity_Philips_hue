/* Copyright (c) 2023 Scott Tongue
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
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.THE SOFTWARE SHALL NOT 
 * BE USED IN ANY ABLEISM WAY.
 */
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using MiniJSON;
using System;

namespace Hue
{
    public class HueLamp : MonoBehaviour
    {
        [SerializeField]
        public bool UseLight { get; set; } = true;
        public bool TurnedOn { get; private set; } = false;
        public Color CurrentColor { get; private set; } = Color.white;
        public string DevicePath { get; private set; }
        public string Key { get; private set; }
       
        private bool _inUse = false;
        private Dictionary<string, object> _state = new Dictionary<string, object>();

        #region public
        /// <summary>
        /// Setups Hue path and key ID of Light
        /// </summary>
        /// <param name="path">path to hue Hub</param>
        /// <param name="key">key ID</param>
        public void Setup(string path, string key)
        {
            Key = key;
            DevicePath = path;
        }

        /// <summary>
        /// Flash light between current color and a new color
        /// </summary>
        /// <param name="colorFlash">Color to Flash to</param>
        /// <param name="speed">Rate of flash</param>
        /// <param name="count">how many times to flash</param>
        /// <returns>returns true if sucessful</returns>
        public bool FlashToAColor(Color colorFlash, float speed = 1f, int count = 1)
        {
            if (CheckInUse())
                return false;

            _inUse = true;
            FlashColor(colorFlash, speed, count);

            return true;
        }

        /// <summary>
        /// Sets brigthness of light
        /// </summary>
        /// <param name="Brightness">level of brightness</param>
        /// <returns>returns true if sucessful</returns>
        public bool SetBrightness(int Brightness)
        {
            if (CheckInUse())
                return false;

            if (Brightness <= 0)
            {
                return TurnOnOffLight(false);
            }
            else
            {
                _inUse = true;
                HTTPStream(Brightness);
                _inUse = false;
                return true;
            }

        }

        /// <summary>
        /// Set color of Light
        /// </summary>
        /// <param name="color">Color to be changed to</param>
        /// <returns>returns true if sucessful</returns>
        public bool SetColor(Color color)
        {
            if (CheckInUse())
                return false;

            _inUse = true;
            SendData(color);
            _inUse = false;

            return true;
        }

        /// <summary>
        /// Toggle Light on and off
        /// </summary>
        /// <param name="TurnOn">set to true to turn on light</param>
        /// <returns>returns true if sucessful</returns>
        public bool TurnOnOffLight(bool TurnOn)
        {
            if (CheckInUse())
                return false;

            _inUse = true;
            SendData(TurnOn);
            _inUse = false;

            return true;
        }
        #endregion

        #region Private 
        private bool CheckInUse()
        {
            if (_inUse)
            {
                Debug.Log(DevicePath + " is in use");
                return true;
            }
            return false;
        }

        private void SendData(Color color)
        {
            if (CurrentColor != color)
            {
                HTTPStream(color);
                CurrentColor = color;
                
            }
        }

        private void SendData(bool turnOn)
        {
            if (TurnedOn != turnOn)
            {
                HTTPStream(turnOn);
                TurnedOn = turnOn;
            }

        }

        private void HTTPStream(Color color)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(DevicePath);
            Debug.Log(DevicePath);
            request.Method = HueCommonNames.Put;

            // Get color state ready for Hue in HSV in JSON format
            _state.Clear();
            Vector3 hsv = HSVFromRGB.ConvertToHSV(color);
            _state[HueCommonNames.Put] = true;
            _state[HueCommonNames.Hue] = (int)(hsv.x / 360.0f * 65535.0f);
            _state[HueCommonNames.Sat] = (int)(hsv.y * 255.0f);
            _state[HueCommonNames.Bri] = (int)(hsv.z * 255.0f);

            if ((int)(hsv.z * 255.0f) == 0)
            {
                _state[HueCommonNames.On] = false;
                TurnedOn = false;
            }
            else
            {
                TurnedOn = true;
            }

            byte[] data = System.Text.Encoding.ASCII.GetBytes(Json.Serialize(_state));
            request.ContentLength = data.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }
        }

        private void HTTPStream(bool turnOn)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(DevicePath);
            Debug.Log(DevicePath);
            request.Method = HueCommonNames.Put;

            _state.Clear();
            _state[HueCommonNames.Put] = turnOn;

            byte[] data = System.Text.Encoding.ASCII.GetBytes(Json.Serialize(_state));
            request.ContentLength = data.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }
        }

        private void HTTPStream(int brightness)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(DevicePath);
            Debug.Log(DevicePath);
            request.Method = HueCommonNames.Put;

            _state.Clear();
            _state[HueCommonNames.Put] = true;
            _state[HueCommonNames.Bri] = brightness;

            byte[] data = System.Text.Encoding.ASCII.GetBytes(Json.Serialize(_state));
            request.ContentLength = data.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }

        }

        private async void FlashColor(Color flashColor, float time, int count)
        {
            _inUse = true;
            Color startingColor = CurrentColor;

            while (count >= 0)
            {
                SendData(flashColor);
                await Task.Delay(TimeSpan.FromSeconds(time));
                SendData(startingColor);
                await Task.Delay(TimeSpan.FromSeconds(time));
                count--;
            }

            _inUse = false;
        }
        #endregion
    }
}