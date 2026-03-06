using System.Collections.Generic;
using UnityEngine;

// Decides how to move to its destination.
public abstract class EnemyMovement : MonoBehaviour
{
    [HideInInspector] public EnemyController ctr;
    public List<string> animationCodeNames;

    public abstract void OnUpdate(float deltaTime);
    public abstract void OnEnter();
    public abstract void OnExit();

    private void Reset()
    {
        if (TryGetComponent(out EnemyController controller))
        {
            ctr = controller;
            ctr.AddAnimationNames(animationCodeNames);
        }
    }
}
