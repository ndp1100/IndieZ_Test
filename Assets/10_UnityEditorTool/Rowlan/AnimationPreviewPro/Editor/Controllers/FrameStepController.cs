using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rowlan.AnimationPreviewPro
{
    public class FrameStepController
    {
        private AnimationPreviewProModule editor;
        private AnimationPreviewProData editorTarget;

        private int currentFrame = 0;

        public FrameStepController(AnimationPreviewProModule editor, AnimationPreviewProData target)
        {
            this.editor = editor;
            this.editorTarget = target;
        }

        public void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Frame Step Controls", GUIStyles.BoxTitleStyle);

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Previous"))
                    {
                        editor.GetClipManager().PreviousClip();
                        editor.GetClipManager().FirstFrame();
                    }
                    if (GUILayout.Button("Next"))
                    {
                        editor.GetClipManager().NextClip();
                        editor.GetClipManager().FirstFrame();
                    }

                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button( GUIStyles.FirstIcon))
                    {
                        FirstFrame();
                    }
                    if (GUILayout.Button(GUIStyles.PreviousIcon))
                    {
                        PreviousFrame();
                    }
                    if (GUILayout.Button(GUIStyles.NextIcon))
                    {
                        NextFrame();
                    }
                    if (GUILayout.Button(GUIStyles.LastIcon))
                    {
                        LastFrame();
                    }

                }
                EditorGUILayout.EndHorizontal();

                int frameCount = editor.GetClipManager().GetFrameCount();

                EditorGUILayout.BeginHorizontal();
                {
                    // EditorGUILayout.PrefixLabel("Frame");
                    EditorGUI.BeginChangeCheck();

                    currentFrame = EditorGUILayout.IntSlider( currentFrame, 0, frameCount);

                    if (EditorGUI.EndChangeCheck())
                    {
                        UpdateFrame();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Frame: " + currentFrame + " / " + frameCount);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        public void FirstFrame()
        {
            currentFrame = 0;
            UpdateFrame();
        }

        private void PreviousFrame()
        {
            currentFrame--;
            UpdateFrame();
        }

        private void NextFrame()
        {
            currentFrame++;
            UpdateFrame();
        }

        private void LastFrame()
        {
            currentFrame = editor.GetClipManager().GetFrameCount();
            UpdateFrame();
        }

        private void UpdateFrame()
        {
            editor.GetClipManager().SetFrame(currentFrame);
        }

    }
}