using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Rowlan.AnimationPreviewPro
{
    public class ClipManager
    {
        private AnimationPreviewProModule editor;
        private AnimationPreviewProData editorTarget;

        private GameObject previewObject;

        private PreviewClip[] previewClips = new PreviewClip[0];
        private PreviewClip previewClip;

        private bool isPlaying = false;

        private Vector2 scrollPosition;

        /// <summary>
        /// The name filter text in the search field
        /// </summary>
        private string nameFilterText = string.Empty;

        /// <summary>
        /// The culture invariante name filter text, lower case.
        /// </summary>
        private string nameFilterTextPattern = string.Empty;


        public ClipManager(AnimationPreviewProModule editor, AnimationPreviewProData target)
        {
            this.editor = editor;
            this.editorTarget = target;
            this.previewObject = target.animator != null ? target.animator.gameObject : null;


            UpdateClipName();
        }

        public void OnInspectorGUI()
        {
            #region clip filter

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("Filter", GUIStyles.BoxTitleStyle);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        float prevLabelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 60f;
                        {

                            EditorGUI.BeginChangeCheck();
                            {
                                nameFilterText = EditorGUILayout.TextField("Name", nameFilterText, GUIStyles.SearchTextField);
                                nameFilterTextPattern = nameFilterText;
                            }
                            if (EditorGUI.EndChangeCheck())
                            {
                                if (string.IsNullOrEmpty(nameFilterTextPattern) == false)
                                {
                                    nameFilterTextPattern = nameFilterTextPattern.ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim();
                                }
                                else
                                {
                                    nameFilterTextPattern = string.Empty;
                                }
                            }

                            GUIStyle nameFilterButtonStyle = string.IsNullOrEmpty(nameFilterTextPattern) ? GUIStyles.SearchCancelButtonEmpty : GUIStyles.SearchCancelButton;
                            if (GUILayout.Button(GUIContent.none, nameFilterButtonStyle))
                            {
                                GUIUtility.keyboardControl = 0;
                                nameFilterText = string.Empty;
                                nameFilterTextPattern = string.Empty;
                            }
                        }
                        EditorGUIUtility.labelWidth = prevLabelWidth;

                    }
                    EditorGUILayout.EndHorizontal();

                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            #endregion clip filter

            EditorGUILayout.Space();

            #region clip list

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("Clip List", GUIStyles.BoxTitleStyle);

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                {
                    if (editor.GetAnimatorManager().HasAnimator() && editor.GetAnimatorManager().HasAnimatorController())
                    {
                        previewClips = GetPreviewClips();

                        for (int i = 0; i < previewClips.Length; i++)
                        {
                            PreviewClip clip = previewClips[i];

                            // filter clips
                            bool nameFilterActive = !string.IsNullOrEmpty(nameFilterTextPattern);

                            if (nameFilterActive)
                            {
                                bool isInFilter = clip.GetDisplayName().ToLower(System.Globalization.CultureInfo.InvariantCulture).Trim().Contains(nameFilterTextPattern);
                                if (!isInFilter)
                                    continue;
                            }

                            // show clip
                            bool isCurrentClip = i == editorTarget.clipIndex;

                            GUI.backgroundColor = isCurrentClip ? GUIStyles.SelectedClipBackgroundColor : GUIStyles.DefaultBackgroundColor;
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("Clip: " + i, GUILayout.Width(60));

                                    if (GUILayout.Button(clip.GetDisplayName()))
                                    {
                                        SetClip(i);
                                        ClipAction();
                                    }

                                    if (GUILayout.Button(EditorGUIUtility.IconContent("AnimationClip Icon", "Open Clip in Project"), GUIStyles.ToolbarButtonStyle))
                                    {
                                        OpenClip(i);
                                    }

                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            GUI.backgroundColor = GUIStyles.DefaultBackgroundColor;
                        }
                    }
                }
                EditorGUILayout.EndScrollView();

            }
            EditorGUILayout.EndVertical();

            #endregion clip list

            #region clip tools

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Log Clips"))
                    {
                        LogClips();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            #endregion clip tools
        }

        // TODO: check how often this is invoked
        private PreviewClip[] GetPreviewClips()
        {
            switch( editorTarget.workflow)
            {
                case AnimationPreviewProData.Workflow.AnimationClip:
                    return PreviewClipProvider.GetAnimatorClips(editor.GetAnimatorManager().GetAnimatorController() as AnimatorController);

                case AnimationPreviewProData.Workflow.StateMachine:
                    return PreviewClipProvider.GetStateMachineClips(editor.GetAnimatorManager().GetAnimatorController() as AnimatorController);

                default:
                    Debug.LogError("Unsupported workflow: " + editorTarget.workflow);
                    return new PreviewClip[0];

            }
        }

        public bool IsPlaying()
        {
            return isPlaying;
        }

        public void OnDisable()
        {
            EditorApplication.update -= DoPreview;

        }

        #region Clip Navigation
        public void PreviousClip()
        {
            editorTarget.clipIndex--;
            editorTarget.clipIndex = GetValidClipIndex(editorTarget.clipIndex);

            ClipChanged();
        }

        public void NextClip()
        {
            editorTarget.clipIndex++;
            editorTarget.clipIndex = GetValidClipIndex(editorTarget.clipIndex);
            ClipChanged();
        }

        private void SetClip(int clipIndex)
        {
            editorTarget.clipIndex = GetValidClipIndex(clipIndex);
            ClipChanged();

        }

        /// <summary>
        /// Reset the clip index to -1 and update the name to implicitly be blank.
        /// This can be used if the underlaying animator controller data changed.
        /// </summary>
        public void ResetClipIndex()
        {
            editorTarget.clipIndex = -1;

            // reset the preview clip. otherwise it would be used in Reset
            // this would otherwise lead to an invalid state when you change gameobjects
            previewClip = null;

            UpdateClipName();
        }

        /// <summary>
        /// Open the clip file in project view
        /// </summary>
        /// <param name="clipIndex"></param>
        private void OpenClip(int clipIndex)
        {

            PreviewClip previewClip = GetClip(clipIndex);

            if (previewClip == null || previewClip.clip == null)
                return;

            Selection.activeObject = previewClip.clip;

        }

        private void ClipChanged()
        {
            if (isPlaying)
                PlayClip();
            else
                ResetClip();

            UpdateClipName();
        }

        public void UpdateClipName()
        {
            PreviewClip clip = GetClipToPreview();

            editorTarget.clipName = clip == null ? "" : clip.GetDisplayName();

        }

        private int GetValidClipIndex(int clipIndex)
        {
            if (!editor.GetAnimatorManager().HasAnimator())
                return -1;

            int clipCount = GetPreviewClips().Length;

            // check if there are clips at all
            if (clipCount == 0)
            {
                return -1;
            }

            if (clipIndex < 0)
            {
                return clipCount - 1;
            }

            if (clipIndex >= clipCount)
            {
                return 0;
            }

            return clipIndex;

        }

        private PreviewClip GetClipToPreview()
        {
            int clipIndex = editorTarget.clipIndex;
            if (clipIndex == -1)
                return null;

            return GetClip(clipIndex);
        }

        private PreviewClip GetClip(int clipIndex)
        {
            if (!editor.GetAnimatorManager().HasAnimatorController())
                return null;

            PreviewClip[] clips = GetPreviewClips();

            if (clipIndex >= clips.Length)
                return null;

            PreviewClip clip = clips[clipIndex];

            return clip;
        }

        #endregion Clip Navigation


        #region Clip Control
        public void ClipAction()
        {
            switch (editorTarget.mode)
            {
                case AnimationPreviewProData.Mode.PlayClip:
                    PlayClip();
                    break;

                case AnimationPreviewProData.Mode.FrameStep:
                    FirstFrame();
                    break;

                default:
                    throw new System.Exception("Unsupported mode: " + editorTarget.mode);

            }
        }

        public void PlayClip()
        {
            isPlaying = true;

            previewClip = GetClipToPreview();
            ResetClip();

            EditorApplication.update -= DoPreview;
            EditorApplication.update += DoPreview;
        }

        void DoPreview()
        {
            if (previewClip == null)
                return;

            if (!editor.GetAnimatorManager().HasAnimator())
                return;

            if (!isPlaying)
                return;

            if (EndOfClipReached())
                return;

            previewClip.clip.SampleAnimation(previewObject, Time.deltaTime);

            editor.GetAnimatorManager().GetAnimator().Update(Time.deltaTime);
        }

        /// <summary>
        /// Check if the end of the clip would be reached considering the current delta time.
        /// If it's reached then the clip is stopped and remains at the end
        /// </summary>
        /// <returns></returns>
        private bool EndOfClipReached()
        {
            Animator animator = editor.GetAnimatorManager().GetAnimator();

            // calculate the current clip time considering the new delta time
            int layer = previewClip.layerName == null ? 0 : animator.GetLayerIndex(previewClip.layerName);
            float length = editor.GetAnimatorManager().GetAnimator().GetCurrentAnimatorStateInfo(layer).length;
            float normalizedTime = editor.GetAnimatorManager().GetAnimator().GetCurrentAnimatorStateInfo(layer).normalizedTime;
            float time = length * normalizedTime;

            //Debug.Log("time " + time + ", norm " + normalizedTime + ", delta " + Time.deltaTime 
            //    + ", array " + editor.GetAnimatorManager().GetAnimator().GetCurrentAnimatorClipInfo(layer).Length
            //    + ", " + editor.GetAnimatorManager().GetAnimator().GetCurrentAnimatorClipInfo(layer)[0].clip.name);

            // the clip would overlap if the time + delta time is larger than the clip length
            bool exceeded = (time + Time.deltaTime) >= length;
            if (exceeded)
            {
                // Debug.Log("Stop Animation");

                StopClip(false); // needs to be reset, otherwise animations without state connections wouldn't be played

                // remain at the end of the clip
                GoToAnimatorPosition(1f);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Reset the clip and move the frame to the first one. Keep on playing if in play mode
        /// </summary>
        public void ResetClip()
        {
            if (previewClip == null)
                return;

            this.previewObject = editorTarget.animator != null ? editorTarget.animator.gameObject : null;

            if (!this.previewObject)
                return;

            previewClip.clip.SampleAnimation(previewObject, 0);

            GoToAnimatorPosition(0f);
        }

        /// <summary>
        /// Go to the specified normalized position of the clip. Used to go to start or the end.
        /// </summary>
        /// <param name="normalizedPosition"></param>
        private void GoToAnimatorPosition( float normalizedPosition)
        {
            Animator animator = editor.GetAnimatorManager().GetAnimator();

            int layer = previewClip.layerName == null ? 0 : animator.GetLayerIndex(previewClip.layerName);

            animator.Play(previewClip.stateName, layer, normalizedPosition);
            animator.Update(0);

        }

        /// <summary>
        /// Stop the play mode. This unregisters from the editor's update event.
        /// 
        /// Optionally reset the clip and move the frame to the first one.
        /// Reset may not be wanted if you switch from one gameobject to another.
        /// </summary>
        public void StopClip(bool reset)
        {
            isPlaying = false;

            EditorApplication.update -= DoPreview;

            if (reset)
            {
                ResetClip();
            }
        }

        public void SetFrame(int frame)
        {
            previewClip = GetClipToPreview();

            if (previewClip == null)
                return;

            if (!editor.GetAnimatorManager().HasAnimator())
                return;

            if (IsPlaying())
            {
                StopClip(true);
            }

            Animator animator = editor.GetAnimatorManager().GetAnimator();

            int layer = previewClip.layerName == null ? 0 : animator.GetLayerIndex(previewClip.layerName);

            AnimatorClipInfo[] animationClip = animator.GetCurrentAnimatorClipInfo(layer);

            if (animationClip.Length == 0)
                return;

            int frameCount = (int)(animationClip[0].clip.length * animationClip[0].clip.frameRate);
            if (frame >= frameCount)
                frame = frameCount;

            if (frame < 0)
                frame = 0;

            this.previewObject = editorTarget.animator != null ? editorTarget.animator.gameObject : null;

            if (!this.previewObject)
                return;

            // int currentFrameInClip = (int)(editor.GetAnimatorManager().GetAnimator().GetCurrentAnimatorStateInfo(0).normalizedTime * (animationClip[0].clip.length * animationClip[0].clip.frameRate));
            previewClip.clip.SampleAnimation(previewObject, 0);

            float frameTime = frame / (float)frameCount;

            animator.speed = 1f;

            animator.Play(previewClip.stateName, layer, frameTime);
            animator.Update(0);

            // Debug.Log($"frameCount: {frameCount}, currentFrame: {frame}, frameTime: {frameTime}");

        }

        /// <summary>
        /// Number of frames in current clip
        /// </summary>
        /// <returns></returns>
        public int GetFrameCount()
        {
            previewClip = GetClipToPreview();

            if (previewClip == null)
                return 0;

            AnimatorClipInfo[] animationClip = editor.GetAnimatorManager().GetAnimator().GetCurrentAnimatorClipInfo(0);

            if (animationClip.Length == 0)
                return 0;

            int frameCount = (int)(animationClip[0].clip.length * animationClip[0].clip.frameRate);

            return frameCount;
        }

        public void FirstFrame()
        {
            editor.GetFrameStepController().FirstFrame();
        }

        #endregion Clip Control

        #region Logging
        private void LogClips()
        {
            if (!editor.GetAnimatorManager().HasAnimator() || !editor.GetAnimatorManager().HasAnimatorController())
                return;

            PreviewClip[] clips = GetPreviewClips();

            string text = "Clips of " + editor.GetAnimatorManager().GetAnimator().name + ": " + clips.Length + "\n";

            for (int i = 0; i < clips.Length; i++)
            {
                PreviewClip previewClip = clips[i];

                text += string.Format("{0}: {1}\n", i, previewClip.GetDisplayName());
            }

            Debug.Log(text);

        }
        #endregion Logging

    }
}