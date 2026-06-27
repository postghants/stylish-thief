using System.Collections.Generic;
using UnityEngine;

public class RickBumper : MonoBehaviour
{
    PlayerStateDriver player;
    [Header("Crime stuff")]
    [Tooltip("How many points does the player get from this?")] public float givenScore;
    [Tooltip("What is the name of this crime?")] public string crime;
    [Header("Bumper setup")]
    [Tooltip("How fast should the player go?")] public float startSpeed;
    [Tooltip("Where is this bumper pointing to?")] public GameObject target;
    [Tooltip("Are you using the stopper?")] public bool usingStopper;
    public float failsafeTime;
    [Header("References, don't touch")]
    [Tooltip("Don't touch this one. It should just refer to the stopper.")] public GameObject stopper;
    [Tooltip("Don't touch this one. It should just refer to the gravity enabler.")] public GameObject gravityEnabler;
    public float fsTimer;
    public bool countFailsafe;
    public GameObject playerPrefab;
    PlayerStateDriver prefabPlayer;


    [Header("Bumper setup")]
    public bool cycleTargets;
    public float cycleTime;
    float timer;
    int currentTarget;
    public List<GameObject> targets;
    void Start()
    {
        prefabPlayer = playerPrefab.GetComponent<PlayerStateDriver>();
        transform.LookAt(target.transform);
        if (!usingStopper)
        {
            stopper.SetActive(false);
        }
        if (target.gameObject != gravityEnabler)
        {
            gravityEnabler.SetActive(false);
        }
    }
    private void Update()
    {
        if (cycleTargets)
        {
            if (timer < cycleTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0;
                target = targets[currentTarget];
                transform.LookAt(target.transform);
                currentTarget++;
                if (currentTarget == targets.Count)
                {
                    currentTarget = 0;
                }
            }
        }
        if (countFailsafe)
        {
            if (fsTimer < failsafeTime)
            {
                fsTimer += Time.deltaTime;
                if (player.ctx.cmd.deceleration == prefabPlayer.ctx.airMoveData.deceleration  || player.gameObject.GetComponent<ActorPhysics>().gravity == -Vector3.up)
                {
                    fsTimer = 0; ;
                    countFailsafe = false;
                }
            }
            else
            {
                fsTimer = 0;
                countFailsafe = false;
                player.EnableControls();
                player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne.falling);
                player.gameObject.GetComponent<ActorPhysics>().gravity = -Vector3.up;
                player.ctx.useGravity = true;
                player.ctx.cmd.deceleration = prefabPlayer.ctx.airMoveData.deceleration;
                player.ctx.cmd.maxSpeedDeceleration = prefabPlayer.ctx.airMoveData.maxSpeedDeceleration;
                //player.SetVelocity(transform.forward * endSpeed);
                player.ctx.hasGrabbed = false;
                player.gameObject.GetComponent<ActorPhysics>().gravity = -Vector3.up;
                //player.transform.position = target.transform.position;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne.falling);
            //player.gameObject.GetComponent<ActorPhysics>().gravity = Vector3.zero;
            player.ctx.useGravity = false;
            player.DisableControls();
            player.ctx.cmd.deceleration = 0;
            player.ctx.cmd.maxSpeedDeceleration = 0;
            player.transform.position = transform.position;
            player.SetVelocity(transform.forward * startSpeed);
            player.ctx.hasGrabbed = false;
            CrimeSpreeManager.instance.DoMinorCrime(givenScore, crime, gameObject);
            countFailsafe = true;
        }
    }
}


//Todo
//Make this thing standard check for how long the player needs to travel in a straight line, before being let go and being able to do shit again. Probably by replacing Next In Sequence with a general target
    //To do this, I need to set a timer, start it OnTrigger, add a collider as child, and... That's it?