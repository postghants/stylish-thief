using System.Collections.Generic;
using UnityEngine;

// Decides behaviour during an attack. Won't exit until ExitAttack is called.
public abstract class EnemyAttack : MonoBehaviour
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
