using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


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

    [Header("References")]
    public Collider environmentCollider;

    [Header("Internal NO TOUCH")]
    public Vector3 velocity;
    public bool isGrounded;
    public float currentGroundAngle;

    public delegate void OnCollision(RaycastHit hit, Vector3 impactVelocity);
    public OnCollision onCollision;

    private Queue<RaycastHit> hits = new();
    private Queue<Vector3> impactVelocities = new();

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

    protected Vector3 CollideAndSlide(Vector3 vel, Vector3 pos, int depth, bool gravityPass, Vector3 velInit)
    {
        if (depth >= maxBounces)
        {
            return Vector3.zero;
        }

        Bounds bounds = environmentCollider.bounds;
        bounds.Expand(-2 * skinWidth);

        float dist = vel.magnitude + skinWidth;
        if (Physics.BoxCast(pos, bounds.extents, vel.normalized, out RaycastHit hit, Quaternion.identity, dist, collisionLayerMask))
        {
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

                bool stairFound = false;
                if (isGrounded && velocity.y == 0 && !gravityPass)
                {
                    // STAIR CHECK STAIR CHECK OH YEAH!!!
                    if (hit.point.y < pos.y - bounds.extents.y + maxStairHeight)
                    {
                        Vector3 stairBoxPos = pos + vel;
                        stairBoxPos.y -= bounds.extents.y - maxStairHeight / 2 - skinWidth * 2;

                        Vector3 antiStairBoxPos = pos + vel;
                        antiStairBoxPos.y += maxStairHeight / 2 + skinWidth * 2;
                        var possibleStairs = Physics.OverlapBox(stairBoxPos, new Vector3(bounds.extents.x, maxStairHeight, bounds.extents.z), Quaternion.identity, groundMask);
                        var notStairs = Physics.OverlapBox(antiStairBoxPos, new Vector3(bounds.extents.x, bounds.extents.y - maxStairHeight, bounds.extents.z), Quaternion.identity, groundMask);
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
                                    if (Physics.BoxCast(newPos, bounds.extents, vel.normalized, out RaycastHit newHit, Quaternion.identity, dist, collisionLayerMask))
                                    {
                                        snapToSurface = vel.normalized * (hit.distance - skinWidth);
                                        hit = newHit;
                                    }
                                    else
                                    {
                                        snapToSurface += vel * 0.5f;

                                    }
                                    snapToSurface.y += maxStairHeight;
                                    stairFound = true;
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
                if (!stairFound)
                {
                    velocity.x = leftover.x / Time.deltaTime; velocity.z = leftover.z / Time.deltaTime;
                }
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

    public bool IsGrounded(Vector3 pos)
    {
        Bounds bounds = environmentCollider.bounds;
        bounds.Expand(-2 * skinWidth);

        float dist = groundCheckDist;
        if (isGrounded && velocity.y == 0 /*&& currentGroundAngle > 0.1f*/)
        {
            dist += velocity.magnitude * groundCheckSpeedMult;
        }
        var hits = Physics.BoxCastAll(pos, bounds.extents, Vector3.down, Quaternion.identity, dist, groundMask);

        foreach (var hit in hits)
        {
            if (hit.distance == 0) // if collider is already overlapping
            {

            }

            if (Vector3.Angle(Vector3.up, hit.normal) < maxSlopeAngle)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsGrounded()
    {
        Vector3 pos = environmentCollider.transform.position;
        return IsGrounded(pos);
    }
}
