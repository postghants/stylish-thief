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

    private Vector3 facingDirection;
    private PlayerAnimEventHandler eventHandler;
    Vector3 entryRotation;
    bool flipDirection;

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
                        Vector3 launchSpeeds = (transform.forward * forwardVelocity);
                        launchSpeeds.y = upVelocity;

                        float difference = entryRotation.y - eventHandler.transform.rotation.eulerAngles.y;
                        if (flipDirection)
                        {
                            launchSpeeds.x = -launchSpeeds.x;
                            launchSpeeds.z = -launchSpeeds.z;
                        }
                        /*if (difference > 90 && difference < 270)
                        {
                            Debug.Log("Over 90, turns back");
                            launchSpeeds.x = -launchSpeeds.x;
                            launchSpeeds.z = -launchSpeeds.z;
                        }
                        else if (difference >= 270)
                        {
                            Debug.Log("Over 270, won't turn back");
                        }
                        if (difference < -90 && difference > -270)
                        {
                            Debug.Log("Under -90, turns back");
                            launchSpeeds.x = -launchSpeeds.x;
                            launchSpeeds.z = -launchSpeeds.z;
                        }
                        else if (difference <= -270)
                        {
                            Debug.Log("Under -270, won't turn back");
                        }
                        Debug.Log(entryRotation.y + " - " + eventHandler.transform.rotation.eulerAngles.y + " = " + difference);*/

                        player.SetVelocity(launchSpeeds);
                        //player.SetTrigger("Hallo!");
                    }
                    else
                    {
                        Vector3 launchSpeeds = (transform.forward * entrySpeed.magnitude);
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
            eventHandler = player.GetComponentInChildren<PlayerAnimEventHandler>();

            entrySpeed = player.ctx.currentVelocity;
            entrySpeed.y = 0;
            entryRotation = eventHandler.transform.rotation.eulerAngles;

            player.Machine.ChangeState(player.Root.Leaf(), player.Root.frozen);
            player.SetVelocity(Vector3.zero);
            player.transform.position = transform.position;
            CalculateAngles();
            hanging = true;
        }
    }
    private void CalculateAngles()
    {
        float difference = transform.rotation.eulerAngles.y - entryRotation.y;
        if (difference > 90 && difference < 270)
        {
            Debug.Log("Over 90, turns back");
            flipDirection = true;
        }
        else if (difference >= 270)
        {
            Debug.Log("Over 270, won't turn back");
            flipDirection = false;
        }
        if (difference < -90 && difference > -270)
        {
            Debug.Log("Under -90, turns back");
            flipDirection = true;
        }
        else if (difference <= -270)
        {
            Debug.Log("Under -270, won't turn back");
            flipDirection = false;
        }
        if (difference >= -90 && difference <= 90)
        {
            flipDirection = false;
        }
        Debug.Log(transform.rotation.eulerAngles.y + " - " + entryRotation.y + " = " + difference);
        
        Vector3 rotation = eventHandler.transform.rotation.eulerAngles;
        rotation.y = transform.rotation.eulerAngles.y;
        if (flipDirection)
        {
            rotation.y = transform.rotation.eulerAngles.y + 90;
            rotation = -rotation;
        }
        eventHandler.transform.rotation = Quaternion.Euler(rotation);
    }
}

//Making the player launch in 2 directions
//Detect the facing direciton of the player
//Extrapolate which side of the bar that is
    //If the bar (normalised) is facing at .5, .5 and the player .5, .5 then the difference on both axes is 0. The player is facing diagonally forward here (not accurate number)
    //If the bar is facing 1, 0 and the player 0, 1 then the difference is 1, 1. The player is facing directly to the right here
    //if the bar is facing .5, .5 and the player 0, 1 then the difference is .5, 1
//Change the player's facing direction to be the closest of 2 facing directions on the bar
//Launch the player in the facing direction

//Making the player capable of changing direction
//Read the move direction of the player! It tells me exactly where the player is inputting so I can adjust things based on that
//Just swap the facing direction when the player turns