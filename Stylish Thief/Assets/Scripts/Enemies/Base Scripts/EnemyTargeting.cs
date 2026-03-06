using UnityEngine;

public abstract class EnemyTargeting : MonoBehaviour
{
    [HideInInspector] public EnemyController ctr;

    public abstract void OnStart();
    public abstract void OnUpdate(float deltaTime);

    public abstract void OnExitAttack(EnemyAttack attack);

}