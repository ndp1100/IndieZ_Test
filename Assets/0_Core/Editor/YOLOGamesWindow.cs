using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace YOLOGames.EditorTools
{
    public class YOLOGamesWindow : EditorWindow
    {
        private static YOLOGamesWindow window;
        Vector2 scrollPos;
        private static List<Object> m_ListFolder = new List<Object>();

        [MenuItem("YOLO/Open Project Window")]
        private static void Init()
        {
            window = (YOLOGamesWindow)EditorWindow.GetWindow(typeof(YOLOGamesWindow), false, "YOLOGames Window");
            window.Show();

            m_ListFolder.Clear();
            load();
        }

        static string saveKey
        {
            get { return "H2W " + Application.dataPath; }
        }

        static void save()
        {
            StringBuilder sb = new StringBuilder();
            if (m_ListFolder.Count > 0)
            {
                foreach (Object obj in m_ListFolder)
                {
                    string path = "Assets";
                    path = AssetDatabase.GetAssetPath(obj);
                    if (!string.IsNullOrEmpty(path) && (File.Exists(path) || Directory.Exists(path)))
                    {
                        sb.Append(path);
                        sb.Append("|");
                    }
                }
            }

            //        Debug.Log("Save Data : " + sb.ToString());
            H2Setting.SetString(saveKey, sb.ToString());
        }

        static void load()
        {
            string data = H2Setting.GetString(saveKey, "");
            //        Debug.Log("In Scene : " + mCurrentScene.name + " Key = " + saveKey);
            //        Debug.Log("Load Data : " + data);
            string[] items = data.Split('|');

            bool needResave = false;
            foreach (string item in items)
            {
                Object o = AssetDatabase.LoadMainAssetAtPath(item);
                if (o)
                    m_ListFolder.Add(o);
                else
                    needResave = true;
            }

            if (needResave)
                save();
        }

        void OnDisable()
        {
//        save();
        }

        void OnEnable()
        {
//        Init();
//        EGDebug.Log("Do nothing");
        }

        List<Object> m_RemoveList = new List<Object>();

        public void OnGUI()
        {
            if (window == null)
                Init();

            if (GUILayout.Button("Add", GUILayout.Width(position.width)))
            {
                string path = "Assets";
                foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object),
                             SelectionMode.Assets))
                {
                    path = AssetDatabase.GetAssetPath(obj);
                    if (!string.IsNullOrEmpty(path) && (File.Exists(path) || Directory.Exists(path)))
                    {
                        path = Path.GetDirectoryName(path);
                        //                    Debug.Log("Add Path = " + path);
                        m_ListFolder.Add(obj);
                        save();
                        break;
                    }
                }
            }

            var oldColor = GUI.backgroundColor;
            m_RemoveList.Clear();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width),
                GUILayout.Height(position.height));
            foreach (Object o in m_ListFolder)
            {
                if (o == null)
                    m_RemoveList.Add(o);
                else
                {
                    EditorGUILayout.BeginHorizontal();

                    GUI.backgroundColor =
                        Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets).Contains(o)
                            ? Color.green
                            : oldColor;
                    if (GUILayout.Button(o.name, GUILayout.Width(position.width - 80))) //80
                    {
                        Collapse(o, true);
                    }

                    if (GUILayout.Button("Up", GUILayout.Width(30))) // 30
                    {
                        Up(o);
                    }

                    if (GUILayout.Button("X", GUILayout.Width(30))) //30
                    {
                        m_RemoveList.Add(o);
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();

            foreach (var remove in m_RemoveList)
            {
                m_ListFolder.Remove(remove);
            }

            if (m_RemoveList.Count > 0)
                save();

            //        DragNDropUpdate();
        }

        private static void Up(Object _o)
        {
            if (_o == null) return;
            int _index = m_ListFolder.IndexOf(_o);
            if (_index == 0) return;
            Object _temp = m_ListFolder[_index - 1];
            m_ListFolder[_index - 1] = _o;
            m_ListFolder[_index] = _temp;
        }

        public static void Collapse(Object obj, bool collapse)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path) && (!File.Exists(path) || !Directory.Exists(path)))
            {
                return;
            }

            // get a reference to the hierarchy window
            var hierarchy = GetFocusedWindow("Project");
            // select our go
            SelectObject(obj);

        }

        public static void SelectObject(Object obj)
        {
            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }

        public static EditorWindow GetFocusedWindow(string window)
        {
            if (EditorWindow.focusedWindow.name.Contains("Project"))
            {
                Event enterkey = new Event { keyCode = KeyCode.Return, type = EventType.KeyDown };
                focusedWindow.SendEvent(enterkey);
            }

            return focusedWindow;
        }

        public static void FocusOnWindow(string window)
        {
            EditorApplication.ExecuteMenuItem("Window/" + window);
        }

    }
}