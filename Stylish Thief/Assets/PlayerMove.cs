using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Grounded Movement")]
    [SerializeField] private float speed;

    [Header("Physics & Collision")]
    [SerializeField] private LayerMask collisionLayerMask;
    [SerializeField] private int maxBounces = 5;
    [SerializeField] private float skinWidth = 0.015f;
    [SerializeField] private float maxSlopeAngle = 55;
    [SerializeField] private Vector3 gravity;

    [Header("References")]
    [SerializeField] private Collider collider;

    private InputAction moveAction;
    private Transform cam;
    

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        cam = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        Vector2 moveInputValue = moveAction.ReadValue<Vector2>();
        float targetAngle = Mathf.Atan2(moveInputValue.x, moveInputValue.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
        Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        Vector3 moveAmount = moveDirection * moveInputValue.magnitude;
        if (moveInputValue != Vector2.zero)
        {
            OnMoveInput(moveInputValue);
        }
        Move(speed * Time.deltaTime * moveAmount);
    }

    public void OnMoveInput(Vector2 input)
    {

    }

    public void Move(Vector3 moveAmount)
    {
        moveAmount = CollideAndSlide(moveAmount, transform.position, 0, false, moveAmount);
        moveAmount += CollideAndSlide(gravity, transform.position + moveAmount, 0, true, gravity);
        transform.Translate(moveAmount);
    }

    private Vector3 CollideAndSlide(Vector3 vel, Vector3 pos, int depth, bool gravityPass, Vector3 velInit)
    {
        if (depth >= maxBounces)
        {
            return Vector3.zero;
        }

        Bounds bounds = collider.bounds;
        bounds.Expand(-2 * skinWidth);

        float dist = vel.magnitude + skinWidth;
        RaycastHit hit;
        if (Physics.BoxCast(pos, bounds.extents, vel.normalized, out hit, Quaternion.identity, dist, collisionLayerMask))
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

                leftover = ProjectAndScale(leftover, hit.normal) * scale;
            }

            return snapToSurface + CollideAndSlide(leftover, pos + snapToSurface, depth + 1, gravityPass, velInit);
        }

        return vel;
    }

    private Vector3 ProjectAndScale(Vector3 leftover, Vector3 normal)
    {
        return Vector3.ProjectOnPlane(leftover, normal).normalized * leftover.magnitude;
    }
}
