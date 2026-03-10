using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ActorPhysics))]
[RequireComponent(typeof(NavMeshAgent))]
public class AgentJumpChase : EnemyMovement
{
    [Header("Movement")]
    public float maxSpeed;
    public float acceleration;

    [Header("Jump")]
    public float maxJumpDist;
    public float jumpSpeed;

    [Header("References")]
    private NavMeshAgent agent;
    private ActorPhysics rb;
    [SerializeField] private Transform rotationTf;

    [Header("Internal")]
    [SerializeField] private float jumpTimer;
    private Vector3 jumpStartPos;
    private Vector3 jumpEndPos;

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
        // If player is in the same patrol zone
        if(ctr.ctx.activeZone.ClosestPoint(ctr.ctx.player.transform.position) == ctr.ctx.player.transform.position)
        {
            agent.SetDestination(ctr.ctx.player.transform.position);
            Vector3 lookPos = ctr.ctx.player.transform.position;
            lookPos.y = agent.transform.position.y;
            rotationTf.transform.LookAt(lookPos);
        }
        else
        {
            var playerZone = ctr.ctx.player.ctx.rb.CurrentZone();
            if (playerZone != null)
            {

            }
        }
    }

    public override void OnExit()
    {

    }

    private void JumpTo(Vector3 start, Vector3 end, float speed, float arc)
    {

    }
}
