using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ActorPhysics))]
[RequireComponent(typeof(NavMeshAgent))]
public class AgentChasePlayer : EnemyMovement
{
    [Header("Movement")]
    public float maxSpeed;
    public float acceleration;
    [SerializeField] private bool onlyInZone;

    [Header("References")]
    private NavMeshAgent agent;
    private ActorPhysics rb;
    [SerializeField] private Transform rotationTf;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<ActorPhysics>();
    }

    public override void OnEnter()
    {
        agent.enabled = true;
        agent.speed = maxSpeed;
        agent.acceleration = acceleration;
        ctr.PlayAnimation("Run");
    }

    public override void OnUpdate(float deltaTime)
    {
        var playerZone = ctr.ctx.player.ctx.rb.CurrentZone();

        if (onlyInZone)
        {
            if (ctr.ctx.activeZone == playerZone)
            {
                agent.SetDestination(ctr.ctx.player.transform.position);
                /*Vector3 lookPos = ctr.ctx.player.transform.position;
                lookPos.y = agent.transform.position.y;
                rotationTf.transform.LookAt(lookPos);*/
                Vector3 lookPos = agent.steeringTarget;
                lookPos.y = agent.transform.position.y;
                rotationTf.transform.LookAt(lookPos);
            }
        }
        else
        {
            agent.SetDestination(ctr.ctx.player.transform.position);
            /*Vector3 lookPos = ctr.ctx.player.transform.position;
            lookPos.y = agent.transform.position.y;
            rotationTf.transform.LookAt(lookPos);*/
            Vector3 lookPos = agent.steeringTarget;
            lookPos.y = agent.transform.position.y;
            rotationTf.transform.LookAt(lookPos);
        }
    }

    public override void OnExit()
    {

    }

    protected override void Reset()
    {
        animationCodeNames.Add("Run");
        base.Reset();
    }
}
