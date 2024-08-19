using UnityEngine;
using UnityEditor;

namespace Rowlan.AnimationPreviewPro
{
    public class AnimationPreviewProEditorWindow : EditorWindow
    {
        private AnimationPreviewProEditorWindow editorWindow;
        private AnimationPreviewProEditor editor;
        private AnimationPreviewProData data;

        [MenuItem( ProjectSetup.MENU)]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            AnimationPreviewProEditorWindow window = (AnimationPreviewProEditorWindow)EditorWindow.GetWindow(typeof(AnimationPreviewProEditorWindow));

            window.titleContent = new GUIContent( "Animation Preview Pro");

            window.Show();
        }


        void OnEnable()
        {
            editorWindow = this;

            ScriptableObjectManager<AnimationPreviewProData> settingsManager = new ScriptableObjectManager<AnimationPreviewProData>(ProjectSetup.SETTINGS_FOLDER, ProjectSetup.SETTINGS_FILENAME);
            data = settingsManager.GetAsset();

            #region init settings

            // override workflow with global settings
            data.workflow = (AnimationPreviewProData.Workflow) ProjectSettingsProvider.InitialWorkflow;

            // auto-assign animator when the window is opened
            if(data.autoAssignment)
            {
                if( Selection.activeObject != null) 
                { 
                    Object obj = Selection.activeObject;
                    if( obj is GameObject)
                    {
                        GameObject go = (GameObject) obj;
                        Animator animator = go.GetComponent<Animator>();

                        data.animator = animator;
                    }
                }
            }

            #endregion init settings

            editor = Editor.CreateEditor(data) as AnimationPreviewProEditor;

            editor.OnEnable();

        }

        public void OnDisable()
        {
            editor.OnDisable();
        }

        void OnGUI()
        {
            editor.OnInspectorGUI();
        }

        /// <summary>
        /// Analyze selection, add animator
        /// </summary>
        void OnSelectionChange()
        {
            editor.AssignSelectedGameObject();

            this.Repaint();
        }
    }
}