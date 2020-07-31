/* Copyright (c) 2020 Scott Tongue
 *
 * 
 * Heavily modifed based on Marc Teyssier Hue Light intergration
 * https://github.com/marcteys/unity-hue
 * 
 * Can very easily removed out of monobehaviour and used in pure C#
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
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Net;
using MiniJSON;
using System;

namespace Hue
{
    public class HueBridge : Singleton<HueBridge>
    {
        public Action FinishConfiguringHue = delegate { };

        [Tooltip("Find Lights at start")]
        [SerializeField]
        private bool _findLightsAtStart = false;
  
        public string HostName { get;  set; }
        public string Username { get;  set; }

        private Dictionary<string, HueLamp> _hueLights = new Dictionary<string, HueLamp>();
        private string _stringURI =null;
        private const string  _type = "type", _name = "name", _extend = "Extended color light", _state = "/state", _configFile = "HueConfig.JSON";

        #region UnityAPI
        protected override void Init()
        {
            base.Init();
          
            DontDestroyOnLoad(this.gameObject);
            if (_findLightsAtStart)
                LoadConfig();

        }
        #endregion

        #region public
        /// <summary>
        /// Get a light source that is connected 
        /// </summary>
        /// <param name="Light">Name of light to find</param>
        /// <returns>Returns Hue Lamp Light</returns>
        public HueLamp GetLight(string Light)
        {
            if (!_hueLights.ContainsKey(Light))
            {
                Debug.LogWarning(Light + " couldn't be deleted because hue light doesn't exist");
            }
            return _hueLights[Light];
        }

       /// <summary>
       /// Delete a light source that is connected
       /// </summary>
       /// <param name="Light">name of light to be deleted</param>
       /// <returns>returns true if deleted</returns>
        public bool DeleteLight(string Light)
        {
            if (!_hueLights.ContainsKey(Light))
            {
                Debug.LogWarning(Light + " couldn't be deleted because hue light doesn't exist");
                return false;
            }

            Destroy(_hueLights[Light].gameObject);
            _hueLights.Remove(Light);
            return true;
        }

        /// <summary>
        /// Setup a new user for hue
        /// </summary>
        public void SetupNewUser()
        {
           

            _stringURI = "http://" + HostName + "/api/";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_stringURI);
            request.Method = "POST";
           

            Dictionary<string, object> user = new Dictionary<string, object>();
            user.Add("devicetype", "MyHueUnityApp");

            byte[] data = System.Text.Encoding.ASCII.GetBytes(Json.Serialize(user));
            request.ContentLength = data.Length;
            Stream s = request.GetRequestStream();
            s.Write(data, 0, data.Length);
          
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            s = response.GetResponseStream();

            StreamReader sr = new StreamReader(s, System.Text.Encoding.UTF8);
            string name = sr.ReadToEnd();
            sr.Close();
            s.Close();
            if (name.Contains("username"))
            {
                
                name = name.Substring(name.IndexOf(":"+'\u0022'));
                name = name.Remove(0, 2);
                name = name.Remove(name.Length - 4);
                Username = name;
                Debug.Log("User created");
                return;
            }
            Debug.Log(" User Couldnt be Created please press link on Hue Hub!!");
        }

        /// <summary>
        /// Configure Hue Bridge Hub 
        /// </summary>
        /// <param name="user">Username for hue bridge hub</param>
        /// <param name="host">IP address of Hue bridge hub</param>
        public void Config(string user, string host)
        {
            HostName = host;
            Username = user;
        }

        /// <summary>
        /// Find all our lights connected to hue bridge hub
        /// </summary>
        public void DiscoverLights()
        {
            _stringURI = "http://" + HostName + "/api/" + Username + "/lights";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_stringURI);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Debug.Log(_stringURI);

            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream, System.Text.Encoding.UTF8);

            string data = sr.ReadToEnd();
            if (data.Contains("error"))
            {
                stream.Close();
                sr.Close();
                response.Close();
                Debug.Log("Error user does not exist!! ");
                return;
            }

            var lights = (Dictionary<string, object>)Json.Deserialize(data);

            foreach (string key in lights.Keys)
            {

                var light = (Dictionary<string, object>)lights[key];
                foreach (HueLamp hueLamp in GetComponentsInChildren<HueLamp>())
                    if (hueLamp.Key.Equals(key)) goto Found;
                      

                if (light[_type].Equals(_extend))
                {
                    GameObject obj = new GameObject((string)light[_name], typeof(HueLamp));
                    obj.transform.parent = transform;
                    obj.GetComponent<HueLamp>().Setup(_stringURI + "/" + key + _state, key);

                    if (_hueLights.ContainsKey(obj.name))
                    {
                        Debug.LogWarning(obj.name + " Duplicated light detect please name each light unquie ");
                        Destroy(obj);
                    }
                    else
                        _hueLights.Add(obj.name, obj.GetComponent<HueLamp>());
                }

            Found:
                ;
            }
            stream.Close();
            sr.Close();
            response.Close();
            GC.Collect();
            
        }

        /// <summary>
        /// Load config file and configure Hue Lights
        /// </summary>
        public void LoadConfig()
        {
            string path = Path.Combine(Application.streamingAssetsPath, _configFile);
            if (!File.Exists(path))
            {
                Debug.Log("Hue Config file does not exist");
                return;
            }
            string dataAsJson = File.ReadAllText(path);
            HueConfig Config = JsonUtility.FromJson<HueConfig>(dataAsJson);
            Debug.Log("Hue Config File Loaded");
            Username = Config.Username;
            HostName = Config.HostName;
            DiscoverLights();
            foreach (LightUse light in Config.Lights)
            {
                if (_hueLights.ContainsKey(light.LightName))
                    _hueLights[light.LightName].UseLight = light.UseLight;
            }

            Debug.Log("Hue Configured");
            FinishConfiguringHue?.Invoke();
            return;
        }

        /// <summary>
        /// Save Hue light configuration to a file 
        /// </summary>
        public void SaveConfig()
        {
            string path = Path.Combine(Application.streamingAssetsPath, _configFile);
            HueConfig Config;
            Config.Username = Username;
            Config.HostName = HostName;
            Config.Lights = new List<LightUse>();
            var HueLamp = FindObjectsOfType<HueLamp>();
            foreach (HueLamp lamp in HueLamp)
            {
                LightUse light;
                light.LightName = lamp.gameObject.name;
                light.UseLight = lamp.UseLight;
                Config.Lights.Add(light);
            }

            string dataAsJson = JsonUtility.ToJson(Config, true);
            File.WriteAllText(path, dataAsJson);
            Debug.Log("Hue Config File Saved!!");
            return;
        }
        #endregion

    }

    [Serializable]
    public struct HueConfig
    {
        public string HostName;
        public string Username;
        public List<LightUse> Lights;
    }

    [Serializable]
    public struct LightUse
    {
        public string LightName;
        public bool UseLight;
        
    }
}