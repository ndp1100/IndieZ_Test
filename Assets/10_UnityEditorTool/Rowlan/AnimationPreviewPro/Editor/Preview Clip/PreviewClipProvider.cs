using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

// TODO: problem: state machine plays one clip after the other
// animator.Play(previewClip.GetFullPath(), layer, 0f); // TODO: sometimes requires full path, sometimes not; might depend on layer index!
// TODO: recursive statemachine
// TODO: layer id and use that
namespace Rowlan.AnimationPreviewPro
{
    public class PreviewClipProvider
    {
        /// <summary>
        /// Get animation clips from animator
        /// </summary>
        /// <param name="animatorController"></param>
        /// <returns></returns>
        public static PreviewClip[] GetAnimatorClips(AnimatorController animatorController)
        {
            List<PreviewClip> previewClips = new List<PreviewClip>();

            AnimationClip[] clips = animatorController.animationClips;

            foreach (AnimationClip clip in clips)
            {
                PreviewClip pc = new PreviewClip()
                {
                    clip = clip,
                    layerName = null,
                    stateName = clip.name
                };

                previewClips.Add(pc);

            }

            return previewClips.ToArray();
        }

        /// <summary>
        /// Get animation clips from state machine
        /// </summary>
        /// <param name="animatorController"></param>
        /// <returns></returns>
        public static PreviewClip[] GetStateMachineClips(AnimatorController animatorController)
        {
            List<PreviewClip> previewClips = new List<PreviewClip>();

            foreach (AnimatorControllerLayer layer in animatorController.layers)
            {
                // layer states
                foreach (ChildAnimatorState state in layer.stateMachine.states)
                {
                    if (state.state.motion is AnimationClip)
                    {
                        PreviewClip pc = new PreviewClip()
                        {
                            clip = state.state.motion as AnimationClip,
                            layerName = layer.name,
                            stateName = state.state.name
                        };

                        previewClips.Add(pc);
                    }
                }

                // child layer states
                foreach (ChildAnimatorStateMachine stateMachine in layer.stateMachine.stateMachines)
                {
                    foreach (ChildAnimatorState state in stateMachine.stateMachine.states)
                    {
                        if (state.state.motion is AnimationClip)
                        {
                            PreviewClip pc = new PreviewClip()
                            {
                                clip = state.state.motion as AnimationClip,
                                layerName = layer.name,
                                stateName = state.state.name
                            };

                            previewClips.Add(pc);
                        }
                    }
                }

            }

            return previewClips.ToArray();
        }
    }
}