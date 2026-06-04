using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ActorPhysics))]
[RequireComponent(typeof(NavMeshAgent))]
public class BigBlasterChase : EnemyMovement
{
    public GameObject testTarget;

    [Header("Movement")]
    public float maxSpeed;
    public float acceleration;
    public float postShootDelay;
    private float postShootTimer;

    [Header("Jump")]
    public float maxJumpDist;
    public float jumpSpeed;
    public float jumpArc;

    [Header("Random Area Around Player Selection")]
    public float circleRadius;
    public float failSafeTime;
    private float timer;

    [Header("References")]
    private NavMeshAgent agent;
    private ActorPhysics rb;
    [SerializeField] private Transform rotationTf;
    [SerializeField] private GameObject projectile;


    [Header("Internal")]
    [SerializeField] private float jumpTimer;
    private bool isJumping = false;
    private Vector3 jumpStartPos;
    private Vector3 jumpEndPos;
    private float jumpDistance;
    private List<PatrolZone> patrolZonePath = null;

    [SerializeField] private bool targetSet;
    [SerializeField] private Vector3 randomTarget;
    [SerializeField] private Vector3 patrolZoneTarget;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<ActorPhysics>();
        if (ctr == null)
        {
            if (TryGetComponent(out EnemyController controller))
            {
                ctr = controller;
                //ctr.AddAnimationNames(animationCodeNames);
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

            if (targetSet)
            {
                /*Vector3 lookPos = patrolZoneTarget;
                lookPos.y = agent.transform.position.y;
                rotationTf.transform.LookAt(lookPos);*/
                Vector3 lookPos = agent.steeringTarget;
                lookPos.y = rotationTf.position.y;
                rotationTf.LookAt(lookPos);
                Vector3 compareTarget = patrolZoneTarget;
                compareTarget.y = 0;
                Vector3 comparePos = agent.transform.position;
                comparePos.y = 0;
                //If arrived at target location
                if (compareTarget == comparePos)
                {
                    Debug.Log("FIRE");
                    targetSet = false;
                    StartCoroutine(ShootAnimationCoroutine());

                    lookPos = ctr.ctx.player.transform.position;
                    lookPos.y = rotationTf.transform.position.y;
                    rotationTf.transform.LookAt(lookPos);
                    if (projectile != null)
                    {
                        if (!projectile.activeSelf)
                        {
                            projectile.SetActive(true);
                        }
                    }
                }
                if (agent.pathPending || agent.pathStatus.ToString() == "Invalid")
                {
                    timer += deltaTime;
                    Debug.Log("Counting: " + timer);
                    if (timer >= failSafeTime)
                    {
                        timer = 0;
                        agent.SetDestination(ctr.ctx.player.transform.position);
                    }
                }
                else
                {
                    timer = 0;
                }
                postShootTimer = 0;
            }
            else
            {
                if (projectile != null && !projectile.activeSelf)
                {
                    if (postShootTimer < postShootDelay)
                    {
                        postShootTimer += deltaTime;
                    }
                    else
                    {
    //Pick a random spot around the player
                        Vector3 randomTargetRaw = Random.insideUnitCircle;
                        randomTargetRaw.z = randomTargetRaw.y;
                        randomTargetRaw.y = 0;
                        randomTarget = ctr.ctx.player.transform.position + (randomTargetRaw.normalized * circleRadius);

                        //Nearest point to target in patrol zone
                        patrolZoneTarget = ctr.ctx.activeZone.ClosestPoint(randomTarget);
                        agent.SetDestination(patrolZoneTarget);

                        targetSet = true;
                        Debug.Log("Moving toward " + randomTarget);
                    }
                    
                }
                else
                {
                }
            }
            //Move toward that spot

            //If on the spot, open fire
            //Script should loop automatically without issue but otherwise reset it now
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
        testTarget.transform.position = patrolZoneTarget;
    }
    public override void OnExit()
    {
        targetSet = false;
        projectile.SetActive(false);
    }

    private IEnumerator ShootAnimationCoroutine()
    {
        var blast = projectile.GetComponent<BigBlast>();
        ctr.PlayAnimation("Aim");
        yield return new WaitForSeconds(blast.telegraphTime);
        ctr.SetAnimationTrigger("Shoot");
        yield return new WaitForSeconds(blast.lingerTime);
        ctr.SetAnimationTrigger("Getup");
        yield return new WaitForSeconds(postShootDelay);

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
        targetSet = false;
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
    private float GetJumpY(float time)
    {
        return jumpStartPos.y + (jumpEndPos.y - jumpStartPos.y) * (time) + jumpArc * time * (time - 1);
    }
    protected override void Reset()
    {
        animationCodeNames.Add("Run");
        animationCodeNames.Add("Jump");
        animationCodeNames.Add("Aim");
        animationCodeNames.Add("Shoot");
        animationCodeNames.Add("Getup");
        base.Reset();
    }
}
