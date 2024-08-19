#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;

class YOLOEditor : EditorWindow
{
    // Scenes
    static Dictionary<string, string> _scene;

    // Screenshot
    int startNumber;
    string path = "/Users/Data/YOLO/";

    [MenuItem("YOLO/Scene Manager")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(YOLOEditor), utility:false, title:"YOEditor");
    }

    #region Function
    static void StartGame()
    {
        if (!UnityEngine.Application.isPlaying)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(_scene.ElementAt(0).Value);
                EditorApplication.ExecuteMenuItem("Edit/Play");
            }
        }
        else EditorApplication.ExecuteMenuItem("Edit/Play");

    }

    static void LoadScene(string scenename)
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                if (_scene.ContainsKey(scenename))
                {
                    EditorSceneManager.OpenScene(_scene[scenename]);
                }
            }
        }
        else
        {
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }
    }

    static string[] ReadNames()
    {
        _scene = new Dictionary<string, string>();
        List<string> temp = new List<string>();

        foreach (EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
        {
            if (S.enabled)
            {
                string name = S.path.Substring(S.path.LastIndexOf('/') + 1);
                name = name.Substring(0, name.Length - 6);
                temp.Add(name);

                _scene.Add(name, S.path);
            }
        }
        return temp.ToArray();
    }

    static void ClearAllData()
    {
        PlayerPrefs.DeleteAll();
        
        // Delete persistent data path
        
    }

    public void CaptureScreenshot(string _path)
    {
        int number = startNumber;
        string name = $"{number}";

        while (System.IO.File.Exists($"{_path}/{name}.png"))
        {
            number++;
            name = $"{number}";
        }
        startNumber = number + 1;
        ScreenCapture.CaptureScreenshot($"{_path}/{name}.png");
    }

    Vector2 scrollPos;
    #endregion

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };

        //
        // Start Game
        if (GUILayout.Button("Start Game"))
        {
            StartGame();
        }

        //
        // Settings
        if (GUILayout.Button("Open Players Setting"))
        {
            EditorApplication.ExecuteMenuItem("File/Build Settings...");
        }

        //
        // Data
        GUILayout.Space(10);
        GUILayout.Label("------ DATA ------", style);
        GUILayout.Space(10);
        if (GUILayout.Button("Clear Data"))
        {
            ClearAllData();
        }
        
        //
        //  Scenes
        GUILayout.Space(10);
        GUILayout.Label("------ SCENES ------", style);
        GUILayout.Space(10);
        EditorGUILayout.BeginVertical();
        var allscene = ReadNames();
        for (int i = 0; i < allscene.Length; i++)
        {
            if (GUILayout.Button(allscene[i]))
            {
                LoadScene(allscene[i]);
            }
        }
        EditorGUILayout.EndVertical();

        //
        // Tools
        GUILayout.Space(10);
        GUILayout.Label("------ TOOLS ------", style);


        //
        // Screenshot
        path = GUILayout.TextField(path);
        if (GUILayout.Button("Capture Screenshot"))
        {
            CaptureScreenshot(path);
        }

        EditorGUILayout.EndScrollView();
    }
}
#endif