using UnityEngine;

public class DistanceTargeting : EnemyTargeting
{
    [Header("Behaviours")]
    [SerializeField] private EnemyMovement idle;
    [SerializeField] private EnemyMovement chase;
    [SerializeField] private EnemyAttack attack;

    [Header("Targeting")]
    [SerializeField] private float chaseDistance;
    [SerializeField] private float loseDistance;
    [SerializeField] private float attackDistance;

    public override void OnStart()
    {
        ctr.currentMovement = idle;
    }

    public override void OnUpdate(float deltaTime)
    {
        if(ctr.currentAttack != null) { return; }
        if(ctr.currentMovement == idle) { Idle(); }
        if(ctr.currentMovement == chase) { Chase(); }
    }

    private void Idle()
    {
        if ((transform.position - ctr.ctx.player.transform.position).sqrMagnitude < chaseDistance * chaseDistance)
        {
            ctr.SetMovement(chase);
        }
    }

    private void Chase()
    {
        float sqrDistance = (transform.position - ctr.ctx.player.transform.position).sqrMagnitude;
        if (sqrDistance < attackDistance * attackDistance)
        {
            ctr.Attack(attack);
        }
        if(sqrDistance > loseDistance * loseDistance)
        {
            ctr.SetMovement(idle);
        }
    }


    public override void OnExitAttack(EnemyAttack attack)
    {
        ctr.SetMovement(chase);
    }
}
