using UnityEngine;
using UnityEngine.InputSystem;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class RickBarSwing : MonoBehaviour
{
    PlayerStateDriver player;
    public bool countDown;
    public float timeUntilControl;
    private float timer;
    public float forwardVelocity;
    public float upVelocity;
    private bool hanging;
    private Vector3 entrySpeed;

    [Header("Input")]
    private InputAction jump;

    public JumpData jumpData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        jump = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        //Jump.PerformJump(player.ctx);
        if (player != null)
        {
            if (hanging)
            {
                if (jump.WasPerformedThisFrame())
                {
                    if (entrySpeed.magnitude < forwardVelocity)
                    {
                        Vector3 launchSpeeds = (Vector3.forward * forwardVelocity);
                        launchSpeeds.y = upVelocity;
                        player.SetVelocity(launchSpeeds);
                    }
                    else
                    {
                        Vector3 launchSpeeds = (Vector3.forward * entrySpeed.magnitude);
                        launchSpeeds.y = upVelocity;
                        player.SetVelocity(launchSpeeds);
                    }
                        
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

            entrySpeed = player.ctx.currentVelocity;
            entrySpeed.y = 0;

            player.Machine.ChangeState(player.Root.Leaf(), player.Root.frozen);
            player.SetVelocity(Vector3.zero);
            player.transform.position = transform.position;
            hanging = true;
        }
    }
}
