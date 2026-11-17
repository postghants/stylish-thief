using Beans.Unity.Mathematics;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

//Physics behaviour specifically made for actors.
public class ActorPhysics : MonoBehaviour
{
    [Header("Properties")]
    public float mass = 1.0f;

    [Header("Physics settings")]
    public LayerMask collisionLayerMask;
    public LayerMask groundMask;
    public int maxBounces = 5;
    public float skinWidth = 0.015f;
    public float groundCheckDist = 0.1f;
    public float groundCheckSpeedMult = 0.1f;
    public float maxSlopeAngle = 55;
    public float maxStairHeight = 0.15f;
    public float minStairWidth = 0.1f;
    public Vector3 gravity;

    [Header("Moving Platform settings")]
    public float movingWallDetectDist = 0.3f;
    public float pushOut = 0.5f;

    [Header("References")]
    public Collider environmentCollider;

    [Header("Internal NO TOUCH")]
    public Vector3 velocity;
    public bool isGrounded;
    public float currentGroundAngle;
    public float landingSpeed;

    public MovingPlatformRef movingPlatformRef;
    public Vector3 platformVelocity;

    public delegate void OnCollision(RaycastHit hit, Vector3 impactVelocity);
    public OnCollision onCollision;

    private Queue<RaycastHit> hits = new();
    private Queue<Vector3> impactVelocities = new();

    public void Tick(float deltaTime, bool doGravityPass)
    {
        if (velocity.sqrMagnitude > 0)
        {
            transform.rotation = Quaternion.identity;
        }
        if (movingPlatformRef != null && isGrounded)
        {
            MoveOnPlatform(velocity * deltaTime + movingPlatformRef.GetVelocity() * deltaTime, doGravityPass, deltaTime);
        }
        else
        {
            Move(deltaTime * velocity, doGravityPass);
        }
    }

    public void Move(Vector3 moveAmount, bool doGravityPass)
    {
        moveAmount = CollideAndSlide(moveAmount, transform.position, 0, false, moveAmount);

        // do a gravity pass if 
        if (IsGrounded(environmentCollider.transform.position + moveAmount) && doGravityPass)
        {
            Vector3 gravityMoveAmount = gravity * Time.fixedDeltaTime;
            if (isGrounded && velocity.y == 0 && currentGroundAngle > 0.1f)
            {
                gravityMoveAmount.y -= velocity.magnitude * groundCheckSpeedMult;
            }
            moveAmount += CollideAndSlide(gravityMoveAmount, transform.position + moveAmount, 0, true, gravity * Time.fixedDeltaTime);
        }

        transform.Translate(moveAmount);

        for (int i = hits.Count - 1; i >= 0; i--)
        {
            if (onCollision == null) { break; }
            onCollision?.Invoke(hits.Dequeue(), impactVelocities.Dequeue());
        }
        hits.Clear();

    }

    public void MoveOnPlatform(Vector3 moveAmount, bool doGravityPass, float deltaTime)
    {
        moveAmount = CollideAndSlide(moveAmount, transform.position, 0, false, moveAmount);

        // do a gravity pass if 
        if (IsGrounded(environmentCollider.transform.position + moveAmount) && doGravityPass)
        {
            Vector3 gravityMoveAmount = gravity * Time.fixedDeltaTime;
            if (isGrounded && velocity.y == 0 && currentGroundAngle > 0.1f)
            {
                gravityMoveAmount.y -= velocity.magnitude * groundCheckSpeedMult;
            }
            moveAmount += CollideAndSlide(gravityMoveAmount, transform.position + moveAmount, 0, true, gravity * Time.fixedDeltaTime);
        }
        transform.Translate(0, moveAmount.y, 0);
        moveAmount -= movingPlatformRef.GetVelocity() * deltaTime;
        movingPlatformRef.UpdatePosition(moveAmount);


        Bounds bounds = environmentCollider.bounds;
        bounds.Expand(-2 * skinWidth);
        bounds.Expand(new Vector3(movingWallDetectDist, -skinWidth * 4, movingWallDetectDist));
        Collider[] colliders = Physics.OverlapBox(transform.position, bounds.extents, Quaternion.identity, collisionLayerMask, QueryTriggerInteraction.Ignore);
        foreach (Collider collider in colliders)
        {
            Physics.Raycast(transform.position, collider.transform.position - transform.position, out RaycastHit hit, 10, collisionLayerMask);
            Vector3 move = hit.normal;
            move.y = 0;
            move = move.normalized * pushOut;
            movingPlatformRef.UpdatePosition(move);
        }

        for (int i = hits.Count - 1; i >= 0; i--)
        {
            if (onCollision == null) { break; }
            onCollision?.Invoke(hits.Dequeue(), impactVelocities.Dequeue());
        }
        hits.Clear();
    }

    protected Vector3 CollideAndSlide(Vector3 vel, Vector3 pos, int depth, bool gravityPass, Vector3 velInit)
    {
        if (depth >= maxBounces)
        {
            return Vector3.zero;
        }

        Bounds bounds = environmentCollider.bounds;
        bounds.Expand(-2 * skinWidth);

        float dist = vel.magnitude + skinWidth;
        if (Physics.BoxCast(pos, bounds.extents, vel.normalized, out RaycastHit hit, Quaternion.identity, dist, collisionLayerMask, QueryTriggerInteraction.Ignore))
        {
            if(hit.collider.GetComponent<MovingPlatform>()) { return vel; }
            Vector3 snapToSurface = vel.normalized * (hit.distance - skinWidth);
            Vector3 leftover = vel - snapToSurface;
            float verticalAngle = Vector3.Angle(Vector3.up, hit.normal);

            if (snapToSurface.magnitude <= skinWidth)
            {
                snapToSurface = Vector3.zero;
            }

            // normal ground / slope
            if (verticalAngle <= maxSlopeAngle)
            {
                currentGroundAngle = verticalAngle;
                if (gravityPass) { return snapToSurface; }

                leftover = ProjectAndScale(leftover, hit.normal);
            }
            // wall or steep slope
            else
            {
                float scale = 1 - Vector3.Dot(
                    new Vector3(hit.normal.x, 0, hit.normal.z).normalized,
                    -new Vector3(velInit.x, 0, velInit.z).normalized);

                //bool stairFound = false;
                if (isGrounded && velocity.y == 0 && !gravityPass)
                {
                    // STAIR CHECK STAIR CHECK OH YEAH!!!
                    if (hit.point.y < pos.y - bounds.extents.y + maxStairHeight)
                    {
                        Vector3 stairBoxPos = pos + vel;
                        stairBoxPos.y -= bounds.extents.y - maxStairHeight / 2 - skinWidth * 2;

                        Vector3 antiStairBoxPos = pos + vel;
                        antiStairBoxPos.y += maxStairHeight / 2 + skinWidth * 2;
                        var possibleStairs = Physics.OverlapBox(stairBoxPos, new Vector3(bounds.extents.x, maxStairHeight, bounds.extents.z), Quaternion.identity, groundMask, QueryTriggerInteraction.Ignore);
                        var notStairs = Physics.OverlapBox(antiStairBoxPos, new Vector3(bounds.extents.x, bounds.extents.y - maxStairHeight, bounds.extents.z), Quaternion.identity, groundMask, QueryTriggerInteraction.Ignore);
                        if (notStairs.Length == 0)
                        {
                            foreach (var possibleStair in possibleStairs)
                            {
                                Vector3 raycastStart = hit.point;
                                raycastStart.y = pos.y - bounds.extents.y + maxStairHeight + skinWidth;

                                bool obstructed = Physics.Raycast(raycastStart, -hit.normal, minStairWidth, collisionLayerMask);
                                if (!obstructed)
                                {
                                    Vector3 newPos = pos;
                                    newPos.y += maxStairHeight;
                                    if (Physics.BoxCast(newPos, bounds.extents, vel.normalized, out RaycastHit newHit, Quaternion.identity, dist, collisionLayerMask, QueryTriggerInteraction.Ignore))
                                    {
                                        snapToSurface = vel.normalized * (hit.distance - skinWidth);
                                        hit = newHit;
                                    }
                                    else
                                    {
                                        snapToSurface += vel * 0.5f;

                                    }
                                    snapToSurface.y += maxStairHeight;
                                    //stairFound = true;
                                }

                                break;
                            }

                        }


                    }
                    leftover = ProjectAndScale(new Vector3(leftover.x, 0, leftover.z), new Vector3(hit.normal.x, 0, hit.normal.z)) * scale;


                }
                else
                {
                    Vector3 horizontalLeftover = ProjectAndScale(leftover, hit.normal) * scale;
                    leftover.x = horizontalLeftover.x; leftover.z = horizontalLeftover.z;

                }
                //if (!stairFound)
                //{
                //    velocity.x = leftover.x / Time.deltaTime; velocity.z = leftover.z / Time.deltaTime;
                //}
            }
            if (hit.point != Vector3.zero)
            {
                hits.Enqueue(hit);
                impactVelocities.Enqueue(velInit / Time.deltaTime);
            }
            return snapToSurface + CollideAndSlide(leftover, pos + snapToSurface, depth + 1, gravityPass, velInit);
        }
        if (vel == velInit)
        {
            currentGroundAngle = 0;
        }
        return vel;
    }

    protected Vector3 ProjectAndScale(Vector3 leftover, Vector3 normal)
    {
        return Vector3.ProjectOnPlane(leftover, normal).normalized * leftover.magnitude;
    }

    public bool IsGrounded(Vector3 pos, out Collider ground)
    {
        Bounds bounds = environmentCollider.bounds;
        bounds.Expand(-2 * skinWidth);

        bool snapDown = false;
        float dist = groundCheckDist;
        if (isGrounded && velocity.y == 0 /*&& currentGroundAngle > 0.1f*/)
        {
            dist += velocity.magnitude * groundCheckSpeedMult;
            snapDown = true;
        }
        var hits = Physics.BoxCastAll(pos, bounds.extents, Vector3.down, Quaternion.identity, dist, groundMask, QueryTriggerInteraction.Ignore);

        List<RaycastHit> validHits = new();
        foreach (var hit in hits)
        {
            if (Vector3.Angle(Vector3.up, hit.normal) < maxSlopeAngle)
            {
                if (snapDown)
                {
                    validHits.Add(hit);
                }
                else
                {
                    ground = hit.collider;
                    return true;
                }
            }
        }
        if (snapDown)
        {
            if (validHits.Count == 0)
            {
                ground = null;
                return false;
            }
            RaycastHit best = validHits[0];
            foreach (var hit in validHits)
            {
                if (best.distance > hit.distance)
                {
                    best = hit;
                }
            }
            transform.Translate(0, -(best.distance - skinWidth), 0);
            ground = best.collider;
            return true;
        }
        ground = null;
        return false;
    }

    public bool IsGrounded(Vector3 pos)
    {
        return IsGrounded(pos, out var ground);
    }

    public bool IsGrounded(out Collider ground)
    {
        Vector3 pos = environmentCollider.transform.position;
        bool grounded = IsGrounded(pos, out var g);
        ground = g;
        return grounded;
    }

    public bool IsGrounded()
    {
        Vector3 pos = environmentCollider.transform.position;
        return IsGrounded(pos, out var g);
    }

    public PatrolZone CurrentZone()
    {
        var colliders = Physics.OverlapBox(environmentCollider.transform.position, environmentCollider.bounds.extents);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out PatrolZone zone))
            {
                return zone;
            }
        }
        return null;
    }

    public void SetMovingPlatformRef(Collider ground)
    {
        //check for moving walls
        //Collider[] colliders = new Collider[16];
        //Bounds bounds = environmentCollider.bounds;
        //bounds.Expand(-2 * skinWidth);
        //bounds.Expand(new Vector3(movingWallDetectDist, -skinWidth * 4, movingWallDetectDist));
        //int wallCount = Physics.OverlapBoxNonAlloc(transform.position, bounds.extents, colliders, Quaternion.identity, collisionLayerMask, QueryTriggerInteraction.Ignore);
        //if (wallCount > 0)
        //{
        //    for (int i = 0; i < wallCount; i++)
        //    {
        //        if (colliders[i].gameObject.TryGetComponent(out MovingPlatform wall))
        //        {
        //            Debug.Log("Yeah");
        //            if (movingPlatformRef == null)
        //            {
        //                //make new ref
        //                CreateMovingPlatformRef(wall, Vector3.zero);
        //            }
        //            else if (movingPlatformRef.movingPlatform.gameObject != wall.gameObject)
        //            {
        //                //replace ref and destroy old one
        //                Vector3 previousVel = movingPlatformRef.GetVelocity();
        //                DestroyMovingPlatformRef(false);
        //                CreateMovingPlatformRef(wall, previousVel);
        //            }

        //            return;
        //        }
        //    }
        //}

        if (ground != null && ground.TryGetComponent(out MovingPlatform platform))
        {
            if(velocity.y > platform.GetVelocity(transform.position).y) { return; }
            if (movingPlatformRef == null)
            {
                //make new ref
                CreateMovingPlatformRef(platform, Vector3.zero);
            }
            else if (movingPlatformRef.movingPlatform.gameObject != platform.gameObject)
            {
                //replace ref and destroy old one
                Vector3 previousVel = movingPlatformRef.GetVelocity();
                DestroyMovingPlatformRef(false);
                CreateMovingPlatformRef(platform, previousVel);
            }
        }
        else if (movingPlatformRef != null)
        {
            //apply platform velocity and destroy ref
            DestroyMovingPlatformRef(true);
        }
    }

    private void CreateMovingPlatformRef(MovingPlatform platform, Vector3 previousPlatVelocity)
    {
        var mpr = new GameObject("MovingPlatformRef");
        mpr.transform.parent = platform.transform;
        mpr.transform.SetPositionAndRotation(transform.position, platform.transform.rotation);
        movingPlatformRef = mpr.AddComponent<MovingPlatformRef>();
        movingPlatformRef.movingPlatform = platform;
        movingPlatformRef.Init();

        transform.parent = mpr.transform;
        velocity -= movingPlatformRef.GetVelocity() - previousPlatVelocity;
    }

    public void DestroyMovingPlatformRef(bool applyVel)
    {
        transform.parent = null;
        if (applyVel)
        {
            velocity += movingPlatformRef.GetVelocity();
        }
        Destroy(movingPlatformRef.gameObject);
    }

    public Vector3 GetAbsoluteVel()
    {
        if (movingPlatformRef == null)
        {
            return velocity;
        }
        else
        {
            return velocity + movingPlatformRef.GetVelocity();
        }
    }
}
