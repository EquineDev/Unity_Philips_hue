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
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using UnityEditor;
using UnityEngine;
namespace Hue
{
   [CustomEditor(typeof(HueBridge))]
    public class HueBridgeEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            HueBridge hueBridge = (HueBridge)target;

            hueBridge.HostName = EditorGUILayout.TextField("Host name:", hueBridge.HostName);
            //	hueBridge.portNumber = EditorGUILayout.IntField ("Port number:", hueBridge.portNumber);
            hueBridge.Username = EditorGUILayout.TextField("Username:", hueBridge.Username);

            if (GUILayout.Button("Discover Lights"))
            {
                hueBridge.DiscoverLights();
            }
            if (GUILayout.Button("Create New User"))
            {
                hueBridge.SetupNewUser();
            }
            if (GUILayout.Button("Save Config"))
            {
                hueBridge.SaveConfig();
            }
            if (GUILayout.Button("Load Config"))
            {
                hueBridge.LoadConfig();
            }
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

    }
}