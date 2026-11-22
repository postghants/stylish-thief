using System.Collections.Generic;
using UnityEngine;

public class AnimationControlHolder : StateMachineBehaviour
{
    [SerializeField] public List<string> overrideTriggers;
    [SerializeField] protected string exitState;

    protected PlayerContext ctx;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ctx ??= animator.GetComponentInParent<PlayerStateDriver>().ctx;
        ctx.player.GiveAnimationControlTo(this);
    }

    public virtual void Exit(Animator animator)
    {
        ctx.player.ReleaseAnimationControl(this);
        animator.Play(exitState);
    }
}
