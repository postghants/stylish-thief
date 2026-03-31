using System.Collections.Generic;
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
    public float jumpArc;

    [Header("References")]
    private NavMeshAgent agent;
    private ActorPhysics rb;
    [SerializeField] private Transform rotationTf;

    [Header("Internal")]
    [SerializeField] private float jumpTimer;
    private bool isJumping = false;
    private Vector3 jumpStartPos;
    private Vector3 jumpEndPos;
    private float jumpDistance;
    private List<PatrolZone> patrolZonePath = null;



    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<ActorPhysics>();
        if (ctr == null)
        {
            if (TryGetComponent(out EnemyController controller))
            {
                ctr = controller;
                ctr.AddAnimationNames(animationCodeNames);
            }
        }
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

        if (isJumping)
        {
            Jumping(deltaTime);
            return;
        }

        // If player is in the same patrol zone
        if (ctr.ctx.activeZone == playerZone)
        {
            agent.SetDestination(ctr.ctx.player.transform.position);
            Vector3 lookPos = ctr.ctx.player.transform.position;
            lookPos.y = agent.transform.position.y;
            rotationTf.transform.LookAt(lookPos);
        }
        else
        {
            if (playerZone != null)
            {
                if (patrolZonePath == null || patrolZonePath.Count == 0 || playerZone != patrolZonePath[^1])
                {
                    patrolZonePath = PatrolZoneManager.instance.FindShortestPath(ctr.ctx.activeZone, playerZone, maxJumpDist);
                }
                ChaseOutsideZone();
            }
        }
    }

    public override void OnExit()
    {

    }

    private void ChaseOutsideZone()
    {
        if ((patrolZonePath != null && patrolZonePath.Count == 0)) { return; }

        Vector3 closestPoint = patrolZonePath[0].ClosestPoint(transform.position);
        agent.SetDestination(ctr.ctx.activeZone.ClosestPoint(closestPoint));
        Vector3 lookPos = closestPoint;
        lookPos.y = agent.transform.position.y;
        rotationTf.transform.LookAt(lookPos);

        if (Vector3.Distance(transform.position, closestPoint) < maxJumpDist)
        {
            JumpTo(transform.position, closestPoint);
        }

    }

    private void JumpTo(Vector3 start, Vector3 end)
    {
        if (Vector3.Distance(start, end) == 0) { return; }
        isJumping = true;
        disableTransition = true;
        jumpTimer = 0;
        jumpStartPos = start;
        jumpEndPos = end + (end - start).normalized * 0.5f;
        start.y = 0;
        end.y = 0;
        jumpDistance = Vector3.Distance(start, end);

        Vector3 lookPos = end;
        lookPos.y = agent.transform.position.y;
        rotationTf.transform.LookAt(lookPos);
        ctr.PlayAnimation("Jump");

    }

    private void Jumping(float deltaTime)
    {
        jumpTimer += deltaTime;
        float jumpProgress = jumpTimer / (jumpDistance / jumpSpeed);
        if (jumpProgress >= 1)
        {
            jumpProgress = 1;
        }
        Vector3 xz = Vector3.Lerp(jumpStartPos, jumpEndPos, jumpProgress);
        float y = GetJumpY(jumpProgress);
        transform.position = new(xz.x, y, xz.z);
        if (jumpProgress == 1)
        {
            isJumping = false;
            disableTransition = false;
            ctr.ctx.activeZone = patrolZonePath[0];
            patrolZonePath.RemoveAt(0);
            patrolZonePath.TrimExcess();
            agent.Warp(transform.position);
        }
    }

    //time is between 0 and 1
    private float GetJumpY(float time)
    {
        return jumpStartPos.y + (jumpEndPos.y - jumpStartPos.y) * (time) + jumpArc * time * (time - 1);
    }
    protected override void Reset()
    {
        animationCodeNames.Add("Run");
        animationCodeNames.Add("Jump");
        base.Reset();
    }
}
