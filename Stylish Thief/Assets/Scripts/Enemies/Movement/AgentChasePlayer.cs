using UnityEngine;
using UnityEngine.AI;

public class AgentChasePlayer : EnemyMovement
{
    [Header("Movement")]
    public float maxSpeed;
    public float acceleration;

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
        agent.SetDestination(ctr.ctx.player.transform.position);
        Vector3 lookPos = ctr.ctx.player.transform.position;
        lookPos.y = agent.transform.position.y;
        rotationTf.transform.LookAt(lookPos);
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
