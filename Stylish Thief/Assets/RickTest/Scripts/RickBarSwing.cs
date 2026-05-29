using UnityEngine;
using UnityEngine.InputSystem;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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
            CalculateAngles();
            hanging = true;
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






//Bugfixing again
//This one appears to contain -180. Let's see what happens when I reverse this one. Set it to false
//Ok... This has ruined its normal funcitoning. Clearly either -180 is a singular outlier, or there's a range I'm not covering.
//I'm gonna feed it false information and just rig the calculations. Let's see what happens at different ranges. Set it back to true btw.
//Right. It doesn't appear to be linked to the angle calculation. It's fine.
//Instead, something's off with the part that comes after!
//Things only seem to happen when you go backwards on the bar... Regardless of flipDirection...
//flipDirection is fine but launching is having weird dependency on the facing angle
//It has nothing to do with the rotation of the animation since that is dependant on flipDirection
//Some part of the actual movement application and calculation has a weird interaction with the rotation of the bar! CalculateAngles is safe

//Woah! It seems I forgot about the attempt to make the bar return your entry speed if you went faster than launch speed. This may be causing the issues by fucking up the new speed when you go backwards
//Normalising the player's current speed should work as a direction indicator, as long as the Y value is neutralised before normalising it. Investigate later if needed!
//Did I find the problem? We're calculating the difference separately twice. Once for the visuals in which we use entryRotation. Once for the physics in which we use...
//Bruh
//What
//Nah
//Why did I compare the player's entry rotation to... its own animation?????
//I DON'T EVEN USE THAT FUCKED UP DIFFERENCE CALCULATION????????? WHY IS IT THERE
//It's possible the issue is actually caused by -
//Uhm the problem is gone. All I think I've done-OOOOOHHHHHH
//The player starts off going into the bar with X speed which is typically below launchSpeed. Then the player launches themselves off the bar, which happens at launchSpeed.
//Then the piece of shit little bit of code I did for momentum conservation kicks in because you're going exactly at launchSpeed which is in fact not lower than launchSpeed!
//So that little bit of momentum conservation code has absolutely no consideration for going backwards. This explains why it only happened on the second bar and not the first!
//Now the animation's somehow fucked so I just fix that and I'm on my way out.
//That was easy enough. Just removed the rotation offset on the visuals when flipDirection is true. Man...
//This code is a mental recession indicator