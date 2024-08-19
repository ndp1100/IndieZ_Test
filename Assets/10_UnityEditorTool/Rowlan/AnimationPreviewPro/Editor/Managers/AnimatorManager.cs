﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.AnimationPreviewPro
{
    public class AnimatorManager
    {
        private AnimationPreviewProModule editor;
        private AnimationPreviewProData editorTarget;

        public AnimatorManager(AnimationPreviewProModule editor, AnimationPreviewProData target)
        {
            this.editor = editor;
            this.editorTarget = target;
        }


        /// <summary>
        /// Check if an animator is available
        /// </summary>
        /// <returns></returns>
        public bool HasAnimator()
        {
            return GetAnimator() != null;
        }

        /// <summary>
        /// Check if an animator controller is available
        /// </summary>
        /// <returns></returns>
        public bool HasAnimatorController()
        {
            return GetAnimatorController() != null;
        }


        /// <summary>
        /// Wrapper method to get the controller from an animator without throwing an exception if the animator is null
        /// </summary>
        /// <returns></returns>
        public RuntimeAnimatorController GetAnimatorController()
        {
            Animator animatorReference = editor.GetAnimator();

            if (animatorReference == null)
                return null;

            return animatorReference.runtimeAnimatorController;
        }

        /// <summary>
        /// Wrapper method for the editor's GetAnimator() method. 
        /// The original should reside in the editor class (for now) because of the serializedproperty access.
        /// </summary>
        /// <returns></returns>
        public Animator GetAnimator()
        {
            return editor.GetAnimator();
        }
    }
}