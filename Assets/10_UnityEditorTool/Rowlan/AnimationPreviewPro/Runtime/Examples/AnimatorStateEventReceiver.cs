using UnityEngine;

namespace Rowlan.AnimationPreviewPro
{
	/// <summary>
	/// Example about using a state machine event on an animator.
	/// 
	/// Add this behaviour to the animation clip of your preference in your animator.
	/// </summary>
    public class AnimatorStateEventReceiver : StateMachineBehaviour
    {
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			PrintEvent("OnStateEnter", animator, stateInfo, layerIndex);
		}
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			PrintEvent("OnStateExit", animator, stateInfo, layerIndex);
		}
		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			PrintEvent("OnStateUpdate", animator, stateInfo, layerIndex);
		}
		override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			PrintEvent("OnStateMove", animator, stateInfo, layerIndex);
		}
		override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			PrintEvent("OnStateIK", animator, stateInfo, layerIndex);
		}

		public void PrintEvent(string eventName, Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
			AnimatorClipInfo[] animationClip = animator.GetCurrentAnimatorClipInfo(0);

			if (animationClip.Length == 0)
				return;

			int frameCount = (int)(animationClip[0].clip.length * animationClip[0].clip.frameRate);
			int currentFrameInClip = (int)(stateInfo.normalizedTime * (animationClip[0].clip.length * animationClip[0].clip.frameRate));

			currentFrameInClip = Mathf.Clamp(currentFrameInClip, 0, frameCount);

			Debug.Log("Animator State Event: " + eventName + ", frame " + currentFrameInClip + " / " + frameCount);

		}
	}
}