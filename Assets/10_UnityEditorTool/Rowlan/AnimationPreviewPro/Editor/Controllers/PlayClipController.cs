using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rowlan.AnimationPreviewPro
{
    public class PlayClipController
    {
        private AnimationPreviewProModule editor;
        private AnimationPreviewProData editorTarget;

        public PlayClipController(AnimationPreviewProModule editor, AnimationPreviewProData target)
        {
            this.editor = editor;
            this.editorTarget = target;
        }

        public void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Play Clip Controls", GUIStyles.BoxTitleStyle);

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Previous"))
                    {
                        editor.GetClipManager().PreviousClip();
                    }
                    if (GUILayout.Button("Next"))
                    {
                        editor.GetClipManager().NextClip();
                    }

                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    {   // Play button has special background color handling
                        GUI.backgroundColor = editor.GetClipManager().IsPlaying() ? GUIStyles.PlayBackgroundColor : GUIStyles.DefaultBackgroundColor;
                        if (GUILayout.Button("Play"))
                        {
                            editor.GetClipManager().PlayClip();
                        }
                        GUI.backgroundColor = GUIStyles.DefaultBackgroundColor;
                    }

                    if (GUILayout.Button("Reset"))
                    {
                        // just reset, don't stop
                        editor.GetClipManager().ResetClip();
                    }
                    if (GUILayout.Button("Stop"))
                    {
                        // stop clip and move to first frame
                        editor.GetClipManager().StopClip( true);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
    }
}