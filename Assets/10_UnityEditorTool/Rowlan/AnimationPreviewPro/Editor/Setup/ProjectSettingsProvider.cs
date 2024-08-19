using UnityEditor;
using UnityEngine;

namespace Rowlan.AnimationPreviewPro
{
    public class ProjectSettingsProvider : SettingsProvider
    {
        const string k_menu = "Rowlan/Animation Preview Pro";
        const SettingsScope k_scope = SettingsScope.User;

        // registry keys
        const string k_workflowEnabled = "Rowlan.AnimationPreviewPro.Workflow.Enabled";
        const string k_stateMachineWarningVisible = "Rowlan.AnimationPreviewPro.Workflow.StateMachine.Warning.Visible";
        const string k_initialWorkflow = "Rowlan.AnimationPreviewPro.InitialWorkflow";

        /* unused
        const string k_windowPositionX = "Rowlan.AnimationPreviewPro.Window.Position.X";
        const string k_windowPositionY = "Rowlan.AnimationPreviewPro.Window.Positin.Y";
        const string k_windowWidth = "Rowlan.AnimationPreviewPro.Window.Width";
        const string k_windowHeight = "Rowlan.AnimationPreviewPro.Window.Height";
        */

        public static bool WorkflowEnabled
        {
            get { return EditorPrefs.GetBool(k_workflowEnabled, false); }
            set { EditorPrefs.SetBool(k_workflowEnabled, value); }
        }

        public static int InitialWorkflow
        {
            get { return EditorPrefs.GetInt(k_initialWorkflow, (int)AnimationPreviewProData.Workflow.AnimationClip); }
            set { EditorPrefs.SetInt(k_initialWorkflow, value); }
        }

        public static bool StateMachineWarningVisible
        {
            get { return EditorPrefs.GetBool(k_stateMachineWarningVisible, true); }
            set { EditorPrefs.SetBool(k_stateMachineWarningVisible, value); }
        }

        /* unused

        public static int WindowPositionX
        {
            get { return EditorPrefs.GetInt(k_windowPositionX, 0); }
            set { EditorPrefs.SetInt(k_windowPositionX, value); }
        }

        public static int WindowPositionY
        {
            get { return EditorPrefs.GetInt(k_windowPositionY, 0); }
            set { EditorPrefs.SetInt(k_windowPositionY, value); }
        }

        public static int WindowWidth
        {
            get { return EditorPrefs.GetInt(k_windowWidth, 400); }
            set { EditorPrefs.SetInt(k_windowWidth, value); }
        }

        public static int WindowHeight
        {
            get { return EditorPrefs.GetInt(k_windowHeight, 600); }
            set { EditorPrefs.SetInt(k_windowHeight, value); }
        }
        */

        public ProjectSettingsProvider(string menuPath, SettingsScope scope) : base(menuPath, scope)
        {
        }

        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);

            // reset button
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Reset"))
                {
                    WorkflowEnabled = false;
                    InitialWorkflow = (int)AnimationPreviewProData.Workflow.AnimationClip;
                    StateMachineWarningVisible = true;

                    /* unused
                    WindowPositionX = 0;
                    WindowPositionY = 0;
                    WindowWidth = 400;
                    WindowHeight = 600;
                    */
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox("Please close and re-open the project window for your settings changes to have effect", MessageType.Info);

            // content browser
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField("Animation Preview Pro", EditorStyles.boldLabel);

                EditorGUI.indentLevel++;

                /* unused

                EditorGUILayout.LabelField("Layout", EditorStyles.boldLabel);

                RectInt windowRect = new RectInt(WindowPositionX, WindowPositionY, WindowWidth, WindowHeight);
                windowRect = EditorGUILayout.RectIntField("Window Dimensions", windowRect);

                EditorGUILayout.Space();
                */

                EditorGUILayout.LabelField("Experimental", EditorStyles.boldLabel);

                bool workflowEnabledValue = EditorGUILayout.Toggle("Workflow Enabled", WorkflowEnabled);
                bool stateMachineWarningVisibleValue = true;
                AnimationPreviewProData.Workflow initialWorkflowValue = AnimationPreviewProData.Workflow.AnimationClip;
                if (workflowEnabledValue)
                {
                    EditorGUILayout.HelpBox("The experimental workflow enables the optional state machine workflow.\nPlease note that experimental features might be removed in a future update", MessageType.Warning);

                    EditorGUI.indentLevel++;
                        
                    initialWorkflowValue = (AnimationPreviewProData.Workflow)EditorGUILayout.EnumPopup("Initial Workflow", (AnimationPreviewProData.Workflow)InitialWorkflow);

                    if (initialWorkflowValue == AnimationPreviewProData.Workflow.StateMachine)
                    {
                        stateMachineWarningVisibleValue = EditorGUILayout.Toggle(new GUIContent("State M. Warning", "Toggle state machine warning visibility in the Animation Preview Pro dialog"), StateMachineWarningVisible);
                    }

                    EditorGUI.indentLevel--;

                }
                
                EditorGUI.indentLevel--;

                EditorGUILayout.EndVertical();

                if (check.changed)
                {
                    WorkflowEnabled = workflowEnabledValue;
                    InitialWorkflow = (int)initialWorkflowValue;
                    StateMachineWarningVisible = stateMachineWarningVisibleValue;

                    /* unused
                    WindowPositionX = windowRect.x;
                    WindowPositionY = windowRect.y;
                    WindowWidth = windowRect.width;
                    WindowHeight = windowRect.height;
                    */
                }
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new ProjectSettingsProvider(k_menu, k_scope);
        }
    }
}