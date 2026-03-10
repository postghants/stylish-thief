using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RandomWalking : EnemyMovement
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float walkAccel;
    [SerializeField] private float minDestinationPauseTime;
    [SerializeField] private float maxDestinationPauseTime;

    [Header("References")]
    private NavMeshAgent agent;
    private ActorPhysics rb;
    [SerializeField] private Transform rotationTf;

    private Vector3 destination;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<ActorPhysics>();
    }

    public override void OnEnter()
    {
        agent.enabled = true;
        agent.speed = walkSpeed;
        agent.acceleration = walkAccel;
        destination = ctr.ctx.activeZone.RandomPointInZone();
        agent.SetDestination(destination);
        Vector3 lookPos = destination;
        lookPos.y = agent.transform.position.y;
        rotationTf.transform.LookAt(lookPos);
        ctr.PlayAnimation("Walk");
    }

    public override void OnUpdate(float deltaTime)
    {
        if (destination == Vector3.zero || (rb.transform.position.x == destination.x && rb.transform.position.z == destination.z))
        {
            StartCoroutine(WaitCoroutine());
        }
    }

    private IEnumerator WaitCoroutine()
    {
        if (maxDestinationPauseTime > 0)
        {
            ctr.PlayAnimation("Idle");
        }
        yield return new WaitForSeconds(Random.Range(minDestinationPauseTime, maxDestinationPauseTime)); 
        destination = ctr.ctx.activeZone.RandomPointInZone();
        agent.SetDestination(destination);
        Vector3 lookPos = destination;
        lookPos.y = agent.transform.position.y;
        rotationTf.transform.LookAt(lookPos);
        ctr.PlayAnimation("Walk");
    }

    public override void OnExit()
    {
        StopAllCoroutines();
    }

    protected override void Reset()
    {
        animationCodeNames.Add("Walk");
        animationCodeNames.Add("Idle");
        base.Reset();
    }
}
