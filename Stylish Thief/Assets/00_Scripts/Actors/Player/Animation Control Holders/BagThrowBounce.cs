using HSM;
using UnityEngine;

public class BagThrowBounce : AnimationControlHolder
{
    private PlayerContext ctx;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ctx ??= animator.GetComponentInParent<PlayerStateDriver>().ctx;
        ctx.player.GiveAnimationControlTo(this);
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        ctx.player.SetAnimRotation(Quaternion.LookRotation(ctx.rb.velocity));
        if (ctx.bhopCount == 0)
        {
            ctx.player.ReleaseAnimationControl(this);
            animator.Play(exitState);
        }
    }
}
