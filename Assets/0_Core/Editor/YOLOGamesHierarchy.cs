using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace YOLOGames.EditorTools
{
#if UNITY_EDITOR
    public class YOLOGamesHierarchy : EditorWindow
    {
        private static YOLOGamesHierarchy window;
        private static List<Transform> m_ListTranform = new List<Transform>();
        private bool mMouseIsInside;

        private GameObject draggedObject
        {
            get
            {
                if (DragAndDrop.objectReferences == null) return null;
                if (DragAndDrop.objectReferences.Length == 1) return DragAndDrop.objectReferences[0] as GameObject;
                return null;
            }
            set
            {
                if (value != null)
                {
                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.objectReferences = new Object[1] { value };
                }
                else DragAndDrop.AcceptDrag();
            }
        }

        Vector2 scrollPos;

        private static Scene mCurrentScene;

        [MenuItem("YOLO/Open Hierarchy")]
        private static void Init()
        {
            mCurrentScene = SceneManager.GetActiveScene();
            window = (YOLOGamesHierarchy)GetWindow(typeof(YOLOGamesHierarchy), false, "YOLOGames Hierarchy");
            window.Show();

            m_ListTranform.Clear();
            load();
        }

        static string saveKey { get { return "H2 " + Application.dataPath + " " + mCurrentScene.name; } }
        static void save()
        {
            StringBuilder sb = new StringBuilder();
            if (m_ListTranform.Count > 0)
            {
                foreach (Transform t in m_ListTranform)
                {
                    sb.Append(GetPath(t));
                    sb.Append("|");
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
                GameObject ob = GameObject.Find(item);
                if (ob)
                    m_ListTranform.Add(ob.transform);
                else
                    needResave = true;
            }

            if (needResave)
                save();
        }

        void OnHierarchyChange()
        {
            //change scene, need load new data
            if (mCurrentScene != SceneManager.GetActiveScene())
            {
                //get new data for new scene
                mCurrentScene = SceneManager.GetActiveScene();
                Init();
                return;
            }

            //save data 
            save();
        }

        void OnDisable()
        {
            save();
        }

        void OnEnable()
        {
            //        Init();
        }

        /*void Update()
        {
            //        Selection.objects = ;
            var gos = Selection.gameObjects;
            if (gos != null && gos.Length > 0)
            {
                Selection.objects = gos.Where(g => m_ListTranform.Contains(g.transform) == false).ToArray();
            }
        }*/

        List<Transform> _listRemoveTransforms = new List<Transform>();
        public void OnGUI()
        {
            /*if (window == null)
                Init();*/

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));
            var oldColor = GUI.backgroundColor;
            _listRemoveTransforms.Clear();
            foreach (var transform in m_ListTranform)
            {
                if (transform == null)
                    _listRemoveTransforms.Add(transform);
                else
                {
                    EditorGUILayout.BeginHorizontal();

                    GUI.backgroundColor = Selection.transforms.Contains(transform) ? Color.green : oldColor;
                    if (GUILayout.Button(transform.name, GUILayout.Width(position.width - 80))) //80
                    {
                        SelectObject(transform.gameObject);
                    }

                    if (GUILayout.Button("Up", GUILayout.Width(30))) // 30
                    {
                        Up(transform);
                    }

                    if (GUILayout.Button("X", GUILayout.Width(30))) //30
                    {
                        _listRemoveTransforms.Add(transform);
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();

            foreach (var listRemoveTransform in _listRemoveTransforms)
            {
                m_ListTranform.Remove(listRemoveTransform);
            }

            if (_listRemoveTransforms.Count > 0)
                save();

            DragNDropUpdate();


        }

        private static void Up(Transform _t)
        {
            if (_t == null) return;
            int _index = m_ListTranform.IndexOf(_t);
            if (_index == 0) return;
            Transform _temp = m_ListTranform[_index - 1];
            m_ListTranform[_index - 1] = _t;
            m_ListTranform[_index] = _temp;
        }

        public static void Collapse(GameObject go, bool collapse)
        {
            // bail out immediately if the go doesn't have children
            if (go.transform.childCount == 0) return;
            // get a reference to the hierarchy window
            var hierarchy = GetFocusedWindow("Hierarchy");
            // select our go
            SelectObject(go);
            // create a new key event (RightArrow for collapsing, LeftArrow for folding)
            var key = new Event { keyCode = collapse ? KeyCode.RightArrow : KeyCode.LeftArrow, type = EventType.KeyDown };
            // finally, send the window the event
            hierarchy.SendEvent(key);
        }

        public static void SelectObject(Object obj)
        {
            // GetFocusedWindow("Hiker Hierarchy");
            Selection.activeObject = obj;
        }

        public static void SelectObjects(Object[] objs)
        {
            GetFocusedWindow("YOLOGames Hierarchy");
            Selection.objects = objs;
        }

        public static EditorWindow GetFocusedWindow(string window)
        {
            FocusOnWindow(window);
            return focusedWindow;
        }

        public static void FocusOnWindow(string window)
        {
            // EditorApplication.ExecuteMenuItem("Window/" + window);
            EditorApplication.ExecuteMenuItem(window);
        }

        void AddItem(Transform transform)
        {
            m_ListTranform.Add(transform);
            save();
        }

        private void DragNDropUpdate()
        {
            var currentEvent = Event.current;
            var type = currentEvent.type;
            var dragged = draggedObject;
            //        bool isDragging = (dragged != null);

            if (type == EventType.MouseDown)
            {
                mMouseIsInside = true;
            }
            else if (type == EventType.MouseDrag)
            {
                mMouseIsInside = true;
                currentEvent.Use();
            }
            else if (type == EventType.MouseUp)
            {
                DragAndDrop.PrepareStartDrag();
                mMouseIsInside = false;
                Repaint();
            }
            else if (type == EventType.DragUpdated)
            {
                // Something dragged into the window
                mMouseIsInside = true;
                UpdateVisual();
                currentEvent.Use();
            }
            else if (type == EventType.DragPerform)
            {

                // We've dropped a new object into the window
                if (dragged != null)
                {
                    //                Debug.Log("Dragged Object " + dragged.name);
                    AddItem(dragged.transform);
                    draggedObject = null;
                }
                mMouseIsInside = false;
                currentEvent.Use();
            }
            else if (type == EventType.DragExited || type == EventType.Ignore)
            {
                mMouseIsInside = false;
            }

            // If the mouse is not inside the window, clear the selection and dragged object
            if (!mMouseIsInside)
            {
                //            selection = null;
                dragged = null;
            }
        }

        private void UpdateVisual()
        {
            if (draggedObject == null) DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            //        else if (draggedObjectIsOurs) DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            else DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }

        public static string GetPath(Transform transform)
        {
            if (transform == null) return null;

            string result = null;
            while (transform)
            {
                result = transform.gameObject.name + '/' + result;
                transform = transform.parent;
            }
            // Drop the trailing '/':
            return result.Remove(result.Length - 1);
        }

        public void OnInspectorUpdate()
        {
            this.Repaint();
        }
    }
#endif
}