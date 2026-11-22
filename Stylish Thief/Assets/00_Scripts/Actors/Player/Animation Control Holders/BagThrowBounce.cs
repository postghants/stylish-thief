using HSM;
using UnityEngine;

public class BagThrowBounce : AnimationControlHolder
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ctx.player.Machine.Root.ActiveChild == ((PlayerRoot)ctx.player.Machine.Root).fixedSpeed
            || ctx.bhopCount == 0)
        {
            Exit(animator);
        }
        Debug.Log("Running btb");
        ctx.player.SetAnimRotation(Quaternion.LookRotation(ctx.rb.velocity));
    }
}
