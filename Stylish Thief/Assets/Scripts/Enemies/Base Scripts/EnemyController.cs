using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

// Holds and updates other enemy components
public abstract class EnemyController : Actor
{
    public EnemyContext ctx;
    public EnemyTargeting targeting;
    public EnemyMovement currentMovement;
    public EnemyAttack currentAttack;
    public List<AnimationNameTitlePair> animations;
    public Animator anim;

    protected virtual void Start()
    {
        targeting.OnStart();
    }

    public virtual void Initialize(PatrolZone zone, PlayerStateDriver player)
    {
        ctx.activeZone = zone;
        ctx.player = player;
    }

    protected virtual void Update()
    {
        targeting.OnUpdate(Time.deltaTime);
        if (currentAttack != null)
        {
            currentAttack.OnUpdate(Time.deltaTime);
        }
        else if (currentMovement != null)
        {
            currentMovement.OnUpdate(Time.deltaTime);
        }
    }

    public virtual void SetMovement(EnemyMovement movement)
    { 
        if(currentMovement.disableTransition) { return; }
        currentMovement.OnExit();
        Debug.Log("Setting movement to " + movement);
        movement.OnEnter();
        currentMovement = movement;
    }

    public virtual void Attack(EnemyAttack attack)
    {
        if (currentMovement.disableTransition) { return; }
        currentAttack = attack;
        attack.OnEnter();
    }

    public virtual void ExitAttack(EnemyAttack attack)
    {
        currentAttack.OnExit();
        currentAttack = null;
        targeting.OnExitAttack(attack);
    }

    public virtual void PlayAnimation(string animationName)
    {
        foreach (var animation in animations)
        {
            if (animation.codeName == animationName)
            {
                anim.Play(animation.animationStateName);
            }
        }
    }

    public virtual void SetAnimationTrigger(string triggerName)
    {
        foreach (var animation in animations)
        {
            if (animation.codeName == triggerName)
            {
                anim.SetTrigger(animation.animationStateName);
            }
        }
    }

    private void Reset()
    {
        List<string> names = new();
        foreach (var move in GetComponents<EnemyMovement>())
        {
            move.ctr = this;
            names.AddRange(move.animationCodeNames);
        }
        foreach (var attk in GetComponents<EnemyAttack>())
        {
            attk.ctr = this;
            names.AddRange(attk.animationCodeNames);
        }
        AddAnimationNames(names);

        targeting = GetComponent<EnemyTargeting>();
        targeting.ctr = this;
    }
    public void AddAnimationNames(List<string> names)
    {
        foreach (var name in names)
        {
            if (!AnimationNameTitlePair.ContainsCodeName(name, animations))
            {
                animations.Add(new AnimationNameTitlePair(null, name));
            }
        }
    }


}

[Serializable]
public class EnemyContext
{
    public PatrolZone activeZone;
    public PlayerStateDriver player;
}

[Serializable]
public class AnimationNameTitlePair
{
    public string animationStateName;
    public string codeName;

    public AnimationNameTitlePair(string animationStateName, string codeName)
    {
        this.animationStateName = animationStateName;
        this.codeName = codeName;
    }

    public static bool ContainsCodeName(string name, List<AnimationNameTitlePair> list)
    {
        foreach (var pair in list)
        {
            if (name == pair.codeName)
            {
                return true;
            }
        }
        return false;
    }
}
