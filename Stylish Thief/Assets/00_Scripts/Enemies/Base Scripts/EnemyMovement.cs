using System.Collections.Generic;
using UnityEngine;

// Decides how to move to its destination.
public abstract class EnemyMovement : MonoBehaviour
{
    [HideInInspector] public EnemyController ctr;
    [HideInInspector] public List<string> animationCodeNames = new();
    [HideInInspector] public bool disableTransition = false;

    public abstract void OnUpdate(float deltaTime);
    public abstract void OnEnter();
    public abstract void OnExit();

    protected virtual void Reset()
    {
        if (TryGetComponent(out EnemyController controller))
        {
            ctr = controller;
            ctr.AddAnimationNames(animationCodeNames);
        }
    }

    public abstract void OnBehaviourDeactivate();
    public abstract void OnBehaviourReactivate();
}
