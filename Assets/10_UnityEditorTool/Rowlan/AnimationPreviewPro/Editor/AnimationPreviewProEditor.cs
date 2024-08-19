using UnityEditor;
using UnityEngine;

namespace Rowlan.AnimationPreviewPro
{
    /// <summary>
    /// Animation preview editor which allows you to select clips of an Animator and play them inside the Unity Editor.
    /// </summary>
    [ExecuteInEditMode]
    [CustomEditor(typeof(AnimationPreviewProData))]
    public class AnimationPreviewProEditor : Editor
    {
        private AnimationPreviewProEditor editor;
        private AnimationPreviewProData editorTarget;

        private AnimationPreviewProModule module;

        public AnimationPreviewProEditor()
        {
            this.module = new AnimationPreviewProModule();
        }
        public void OnEnable()
        {
            editor = this;
            editorTarget = (AnimationPreviewProData)target;

            module.OnEnable(serializedObject, editorTarget);

        }

        public void OnDisable()
        {
            module.OnDisable();
        }
        public override void OnInspectorGUI()
        {
            module.OnInspectorGUI();
        }

        public void AssignSelectedGameObject()
        {
            module.AssignSelectedGameObject();
        }

    }
}

