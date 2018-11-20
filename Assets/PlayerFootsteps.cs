using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerFootsteps : StateMachineBehaviour {

    public List<Breakpoint> breakpoints;
    /*= new List<Breakpoint>()
    {
        new Breakpoint(0.15f, SoundName.RobotFootsteps),
        new Breakpoint(0.65f, SoundName.RobotFootsteps),
    };
    */
    

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        foreach (var bp in breakpoints)
        {
            bp.UnHit();
        }
    }

    [Serializable]
    public class Breakpoint
    {
        public float percent;
        bool hit = false;
        public SoundName sound;
        public float volume = 1.0f;

        public bool IsHit(float percent)
        {
            if (hit) return false;

            if (percent >= this.percent)
            {
                hit = true;
                return true;
            }

            return false;
        }

        public void UnHit()
        {
            hit = false;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        foreach (Breakpoint bp in breakpoints) {
            if (bp.IsHit(stateInfo.normalizedTime))
            {
                SoundManager.instance.Play(bp.sound);
            }
        }
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
