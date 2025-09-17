using System.Linq;
using UnityEngine;

// Base class for any player/enemy/npc. Includes basic collision and a Move function to be called every fixed update.
public class Actor : MonoBehaviour
{

    [Header("Physics & Collision")]
    [SerializeField] protected LayerMask collisionLayerMask;
    [SerializeField] protected LayerMask groundMask;
    [SerializeField] protected int maxBounces = 5;
    [SerializeField] protected float skinWidth = 0.015f;
    [SerializeField] protected float groundCheckDist = 0.1f;
    [SerializeField] protected float maxSlopeAngle = 55;
    [SerializeField] protected float maxStairHeight = 0.15f;
    [SerializeField] protected float minStairWidth = 0.1f;
    [SerializeField] protected Vector3 gravity;

    [Header("References")]
    [SerializeField] protected Collider environmentCollider;

    [Header("Internal NO TOUCH")]
    [SerializeField] protected Vector3 velocity;
    [SerializeField] protected bool isGrounded;

    public void Move(Vector3 moveAmount, bool doGravityPass)
    {
        moveAmount = CollideAndSlide(moveAmount, transform.position, 0, false, moveAmount);

        // do a gravity pass if 
        if (IsGrounded(environmentCollider.transform.position + moveAmount) && doGravityPass)
        {
            moveAmount += CollideAndSlide(gravity * Time.fixedDeltaTime, transform.position + moveAmount, 0, true, gravity * Time.fixedDeltaTime);
        }

        velocity.x = moveAmount.x / Time.deltaTime; velocity.z = moveAmount.z / Time.deltaTime;

        transform.Translate(moveAmount);
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
            if (verticalAngle < maxSlopeAngle)
            {
                if (gravityPass) { return snapToSurface; }

                float mag = leftover.magnitude;
                leftover = ProjectAndScale(leftover, hit.normal);
            }
            // wall or steep slope
            else
            {
                float scale = 1 - Vector3.Dot(
                    new Vector3(hit.normal.x, 0, hit.normal.z).normalized,
                    -new Vector3(velInit.x, 0, velInit.z).normalized);



                if (isGrounded && !gravityPass)
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
            }

            return snapToSurface + CollideAndSlide(leftover, pos + snapToSurface, depth + 1, gravityPass, velInit);
        }

        return vel;
    }

    protected Vector3 ProjectAndScale(Vector3 leftover, Vector3 normal)
    {
        return Vector3.ProjectOnPlane(leftover, normal).normalized * leftover.magnitude;
    }

    protected bool IsGrounded(Vector3 pos)
    {
        Bounds bounds = environmentCollider.bounds;
        bounds.Expand(-2 * skinWidth);
        var hits = Physics.BoxCastAll(pos, bounds.extents, Vector3.down, Quaternion.identity, groundCheckDist, groundMask);

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
}
