using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class RickBarSwing : MonoBehaviour
{
    public float testDifference;
    [SerializeField] bool flipDirection;
    PlayerStateDriver player;
    public bool countDown;
    public float timeUntilControl;
    private float timer;
    public float forwardVelocity;
    public float upVelocity;
    private bool hanging;
    private Vector3 entrySpeed;

    private Vector3 facingDirection;
    private PlayerAnimEventHandler eventHandler;
    Vector3 entryRotation;
    private Vector3 localModelPosition;

    [Header("Input")]
    private InputAction jump;
    private InputAction trick;
    private InputAction grab;
    public JumpData jumpData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        jump = InputSystem.actions.FindAction("Jump");
        trick = InputSystem.actions.FindAction("Trick");
        grab = InputSystem.actions.FindAction("Grab");
    }

    // Update is called once per frame
    void Update()
    {
        //Jump.PerformJump(player.ctx);
        if (player != null)
        {
            if (hanging)
            {
                if (trick.WasPerformedThisFrame() || grab.WasPerformedThisFrame())
                {
                    flipDirection = !flipDirection;
                    Debug.Log("flipping");
                }
                if (flipDirection)
                {
                    eventHandler.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);
                }
                else
                {
                    eventHandler.transform.rotation = transform.rotation;
                }
                if (jump.WasPerformedThisFrame())
                {
                    player.gameObject.GetComponentInChildren<PlayerAnimEventHandler>().gameObject.transform.localPosition = localModelPosition;
                    //if (entrySpeed.magnitude < forwardVelocity)
                    //{
                    Vector3 launchSpeeds = (transform.forward * forwardVelocity);
                    launchSpeeds.y = upVelocity;

                    //float difference = entryRotation.y - eventHandler.transform.rotation.eulerAngles.y;
                    if (flipDirection)
                    {
                        launchSpeeds.x = -launchSpeeds.x;
                        launchSpeeds.z = -launchSpeeds.z;
                    }
                    player.SetVelocity(launchSpeeds);
                    player.SetTrigger("BarSwingToJump");
                    //}
                    /*else
                    {
                        Vector3 launchSpeeds = (transform.forward * entrySpeed.magnitude);
                        launchSpeeds.y = upVelocity;
                        player.SetVelocity(launchSpeeds);
                    }*/

                    player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne);
                    hanging = false;
                    player.ctx.hasGrabbed = false;
                    countDown = true;
                    player.DisableControls();
                    //player.ctx.airMoveData.deceleration = 0;
                    player.ctx.cmd.deceleration = 0;
                }
            }
            if (countDown)
            {
                timer += Time.deltaTime;
                if (timer >= timeUntilControl)
                {
                    timer = 0;
                    countDown = false;
                    player.EnableControls();
                    //player.ctx.airMoveData.deceleration = 4;
                    player.ctx.cmd.deceleration = 4;
                    player = null;
                }
            }
        }
        //exampleAction.performed or .started or .canceled += OnExamplePerform or OnExampleStart or OnExampleStop

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            eventHandler = player.GetComponentInChildren<PlayerAnimEventHandler>();

            entrySpeed = player.ctx.currentVelocity;
            entrySpeed.y = 0;
            entryRotation = eventHandler.transform.rotation.eulerAngles;

            player.Machine.ChangeState(player.Root.Leaf(), player.Root.frozen);
            player.SetVelocity(Vector3.zero);
            player.transform.position = transform.position;
            localModelPosition = player.gameObject.GetComponentInChildren<PlayerAnimEventHandler>().gameObject.transform.localPosition;
            player.gameObject.GetComponentInChildren<PlayerAnimEventHandler>().gameObject.transform.localPosition = Vector3.zero;
            CalculateAngles();
            hanging = true;
            player.SetTrigger("StartBarSwing");
        }
    }
    private void CalculateAngles()
    {
        float difference = transform.rotation.eulerAngles.y - entryRotation.y;
        Debug.Log(transform.rotation.eulerAngles.y + " - " + entryRotation.y + " = " + difference);

        Vector3 rotation = Vector3.zero;

        if ((difference > 90 && difference < 270) || (difference < -90 && difference > -270))
        {
            flipDirection = true;
            eventHandler.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);
        }
        else
        {
            flipDirection = false;
            eventHandler.transform.rotation = transform.rotation;
        }
    }
}