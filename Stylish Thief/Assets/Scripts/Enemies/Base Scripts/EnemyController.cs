using System;
using System.Collections.Generic;
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

    protected virtual void Update()
    {
        if (currentAttack != null)
        {
            currentAttack.OnUpdate(Time.deltaTime);
        }
        else
        {
            currentMovement.OnUpdate(Time.deltaTime);
        }
    }

    public virtual void ExitAttack(EnemyAttack attack)
    {
        targeting.OnExitAttack(attack);
    }

    public virtual void PlayAnimation(string animationName)
    {
        foreach(var animation in animations)
        {
            if(animation.codeName == animationName)
            {
                anim.Play(animation.animationStateName);
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
