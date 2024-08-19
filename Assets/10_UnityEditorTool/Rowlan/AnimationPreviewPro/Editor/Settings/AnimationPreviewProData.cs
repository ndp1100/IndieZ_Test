using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace Rowlan.AnimationPreviewPro
{
    /// <summary>
    /// Preview animations of an Animator inside the Unity Editor
    /// 
    /// Usage:
    /// 
    ///   + create empty gameobject and attach this script to it
    ///   + drag a gameobject with an animator from the scene into the Animator slot
    ///   + use the control functions to play animations
    /// 
    /// </summary>
    public class AnimationPreviewProData : ScriptableObject
    {
        public enum Mode
        {
            PlayClip,
            FrameStep
        }

        public enum Workflow
        {
            AnimationClip,
            StateMachine
        }

        /// <summary>
        /// Show help box for information
        /// </summary>
        public bool helpBoxVisible = false;

        /// <summary>
        /// Automatically assigns the selected gameobject to the animator
        /// </summary>
        [SerializeField]
        public bool autoAssignment = false;

        [SerializeField]
        public Mode mode = Mode.PlayClip;

        [SerializeField]
        public Workflow workflow = Workflow.AnimationClip;

        [System.NonSerialized]
        public int clipIndex = -1;

        [System.NonSerialized]
        public string clipName = "";

        /// <summary>
        /// The animator to be used.
        /// Not serializable because it can't be stored when it comes from the scene.
        /// </summary>
        [System.NonSerialized]
        public Animator animator;

        /// <summary>
        /// The controller to be used.
        /// Not serializable because it can't be stored when it comes from the scene.
        /// </summary>
        [System.NonSerialized]
        public RuntimeAnimatorController controller;

    }
}