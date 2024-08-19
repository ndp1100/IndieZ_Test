using UnityEditor;
using UnityEngine;

namespace Rowlan.AnimationPreviewPro
{
    /// <summary>
    /// Animation preview module which allows you to select clips of an Animator and play them inside the Unity Editor.
    /// </summary>
    public class AnimationPreviewProModule
    {
        private AnimationPreviewProModule editor;
        private AnimationPreviewProData editorTarget;
        private SerializedObject serializedObject;

        private SerializedProperty autoAssignment;
        private SerializedProperty mode;
        private SerializedProperty workflow;

        private GUIContent[] modeButtons;

        private AnimatorManager animatorManager;
        private ClipManager clipManager;

        private PlayClipController playClipController;
        private FrameStepController frameStepController;

        public void OnEnable(SerializedObject serializedObject, AnimationPreviewProData target)
        {
            editor = this;
            editorTarget = target;
            this.serializedObject = serializedObject;

            autoAssignment = serializedObject.FindProperty("autoAssignment");
            mode = serializedObject.FindProperty("mode");
            workflow = serializedObject.FindProperty("workflow");

            modeButtons = new GUIContent[]
            {
                new GUIContent( "Play Clip", "Play a clip"),
                new GUIContent( "Frame Step", "Step through a clip frame by frame"),
            };

            // managers
            animatorManager = new AnimatorManager(editor, target);
            clipManager = new ClipManager(editor, target);

            // controllers
            playClipController = new PlayClipController(editor, target);
            frameStepController = new FrameStepController(editor, target);

            // try to get the animator from the gameobject if none is specified
            /*
            if ( !animatorManager.HasAnimator())
            {
                editorTarget.animator = editorTarget.GetComponent<Animator>();
            }
            */
            
            if(animatorManager.HasAnimator() && !animatorManager.HasAnimatorController())
            {
                Debug.LogWarning( $"Runtime animator controller not found for animator {editorTarget.animator.name}");
            }

        }

        public void OnDisable()
        {
            clipManager.OnDisable();
        }

        #region Inspector
        public void OnInspectorGUI()
        {
            serializedObject.Update();

            /// 
            /// Info & Help
            /// 
            GUILayout.BeginVertical(GUIStyles.HelpBoxStyle);
            {
                EditorGUILayout.BeginHorizontal();


                if (GUILayout.Button("Asset Store", EditorStyles.miniButton, GUILayout.Width(120)))
                {
                    Application.OpenURL("https://assetstore.unity.com/packages/tools/animation/animation-preview-pro-227335");
                }

                if (GUILayout.Button("Documentation", EditorStyles.miniButton))
                {
                    Application.OpenURL("https://bit.ly/animationpreviewpro-doc");
                }

                if (GUILayout.Button("Forum", EditorStyles.miniButton, GUILayout.Width(120)))
                {
                    Application.OpenURL("https://forum.unity.com/threads/released-animation-preview.1297551");
                }

                if (GUILayout.Button( new GUIContent("?", "Help box visibility"), EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    // toggle help box visibility
                    editorTarget.helpBoxVisible = !editorTarget.helpBoxVisible;
                }

                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUIStyles.AppTitleBoxStyle);
            {
                EditorGUILayout.LabelField("Animation Preview Pro", GUIStyles.AppTitleBoxStyle, GUILayout.Height(30));

            }
            GUILayout.EndVertical();

            bool animatorChanged = false;

            // help
            if (editorTarget.helpBoxVisible)
            {
                EditorGUILayout.HelpBox(
                    "Play animator clips inside the Unity editor. Press Play or the clip button to play the selected animation. Press Stop to stop continuous playing."
                    + "\n\n"
                    + "Setup: Create an animator controller, drag animations into the controller, assign the controller to an animator of a gameobject and drag the gameobject into the Animator slot."
                    , MessageType.Info);
            }

            // data
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("Clip Data", GUIStyles.BoxTitleStyle);

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUI.BeginChangeCheck();
                    {
                        EditorGUILayout.PropertyField(autoAssignment, new GUIContent("Auto Assignment", "Automatically assign a gameobject to the animator on selection change"));
                    }
                    if( EditorGUI.EndChangeCheck())
                    {
                        AssignSelectedGameObject();
                    }

                    if (ProjectSettingsProvider.WorkflowEnabled)
                    {
                        EditorGUILayout.PropertyField(workflow, new GUIContent("Workflow", "Use animation clip or state machine. State machine can lead to unexcpected behaviour, it depends on the animator controller"));

                        if (workflow.intValue == (int)AnimationPreviewProData.Workflow.StateMachine)
                        {
                            if (ProjectSettingsProvider.StateMachineWarningVisible)
                            {
                                EditorGUILayout.HelpBox("The State Machine workflow is experimental. Its purpose is to scan the state machine of the animator for animations. It's not a runtime player in editor, so it doesn't eg handle events or blending. It was added for customers who requested that feature.", MessageType.Warning);
                            }
                        }
                    }

                    GUI.backgroundColor = animatorManager.HasAnimator() && animatorManager.HasAnimatorController() ? GUIStyles.DefaultBackgroundColor : GUIStyles.ErrorBackgroundColor;
                    {
                        editorTarget.animator = EditorGUILayout.ObjectField(new GUIContent("Animator"), editorTarget.animator, typeof(Animator), true) as Animator; // animator isn't serialized

                        // visualize the controller
                        GUI.enabled = false;
                        {
                            EditorGUILayout.ObjectField(new GUIContent("Controller"), editorTarget.controller, typeof(RuntimeAnimatorController), true); // controller isn't serialized
                        }
                        GUI.enabled = true;

                        if (!animatorManager.HasAnimator() || !animatorManager.HasAnimatorController())
                        {
                            EditorGUILayout.HelpBox("The animator must have a controller. Use a gameobject with an attached Animator and Controller.", MessageType.Error);
                        }

                    }
                    GUI.backgroundColor = GUIStyles.DefaultBackgroundColor;


                    if (!animatorManager.HasAnimatorController())
                    {
                        EditorGUILayout.HelpBox("Quick solution to get a preview of all animations:\n1. Create > Animator Controller\n2. Drag all animations into the controller\n3. Add controller to your animator ", MessageType.Info);
                    }

                }

                // stop clip in case the animator changes
                if (EditorGUI.EndChangeCheck())
                {
                    animatorChanged = true;

                    // update the controller
                    editorTarget.controller = animatorManager.GetAnimatorController();
                }

                GUI.enabled = false;
                {
                    EditorGUILayout.IntField(new GUIContent("Clip Index"), editorTarget.clipIndex); // clip index isn't serialized
                    EditorGUILayout.TextField(new GUIContent("Clip Name"), editorTarget.clipName); // clip name isn't serialized
                }
                GUI.enabled = true;

            }
            EditorGUILayout.EndVertical();

            // mode toolbar
            EditorGUI.BeginChangeCheck();
            {
                mode.intValue = GUILayout.Toolbar(mode.intValue, modeButtons, EditorStyles.miniButton);
            }
            if (EditorGUI.EndChangeCheck())
            {
                ModeChanged((AnimationPreviewProData.Mode)mode.intValue);
            }

            // control
            switch( editorTarget.mode)
            {
                case AnimationPreviewProData.Mode.PlayClip:
                    playClipController.OnInspectorGUI();
                    break;

                case AnimationPreviewProData.Mode.FrameStep:
                    frameStepController.OnInspectorGUI();
                    break;

                default:
                    throw new System.Exception("Unsupported mode: " + editorTarget.mode);
            }

            // clip list
            clipManager.OnInspectorGUI();

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();

            if (animatorChanged)
            {
                clipManager.StopClip( true);

                // reset the 
                // set index to either -1 or the first index depending on the number of animations
                editorTarget.clipIndex = !animatorManager.HasAnimator() || !animatorManager.HasAnimatorController() || animatorManager.GetAnimatorController().animationClips.Length == 0 ? -1 : 0;

                clipManager.UpdateClipName();

                EditorUtility.SetDirty(serializedObject.targetObject);

            }
        }
        #endregion Inspector

        /// <summary>
        /// Get the animator
        /// </summary>
        /// <returns></returns>
        public Animator GetAnimator()
        {
            return editorTarget.animator;
        }

        public AnimatorManager GetAnimatorManager()
        {
            return animatorManager;
        }

        public ClipManager GetClipManager()
        {
            return clipManager;
        }

        public FrameStepController GetFrameStepController()
        {
            return frameStepController;
        }

        private void ModeChanged(AnimationPreviewProData.Mode mode)
        {
            clipManager.StopClip( true);
            clipManager.FirstFrame();
        }

        public void AssignSelectedGameObject()
        {
            if (!autoAssignment.boolValue)
                return;

            // stop clip in case the user switches to another object
            // otherwise the warning "Can't call Animator.Update on inactive object" would show up
            // in case the new selected object isn't properly configured
            // besides we don't want to reset because otherwise when switching from one gameobject
            // to another the clip would be reset and the frame would be set to the first one
            GetClipManager().StopClip( false);

            // reset the clip index so that no clip is selected and no name is displayed
            // the underlaying animator controller changed. it may be the same, but not necessarily
            GetClipManager().ResetClipIndex();

            Animator animator = null;

            GameObject selectedObject = Selection.activeGameObject;

            // note: can be null; in that case the preview window will show the missing animator
            if (selectedObject)
            {
                animator = selectedObject.GetComponent<Animator>();
            }

            editorTarget.animator = animator;

            // update controller (name in ui, etc)
            editorTarget.controller = animatorManager.GetAnimatorController();

            EditorUtility.SetDirty(serializedObject.targetObject);
            
        }
    }
}

