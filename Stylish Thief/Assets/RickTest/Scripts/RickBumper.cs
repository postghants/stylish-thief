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
    [Header("References, don't touch")]
    [Tooltip("Don't touch this one. It should just refer to the stopper.")] public GameObject stopper;
    [Tooltip("Don't touch this one. It should just refer to the gravity enabler.")] public GameObject gravityEnabler;
    void Start()
    {
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne.falling);
            player.gameObject.GetComponent<ActorPhysics>().gravity = Vector3.zero;
            player.DisableControls();
            player.ctx.cmd.deceleration = 0;
            player.ctx.cmd.maxSpeedDeceleration = 0;
            player.transform.position = transform.position;
            player.SetVelocity(transform.forward * startSpeed);
            player.ctx.hasGrabbed = false;
            CrimeSpreeManager.instance.DoMinorCrime(givenScore, crime);
        }
    }
}


//Todo
//Make this thing standard check for how long the player needs to travel in a straight line, before being let go and being able to do shit again. Probably by replacing Next In Sequence with a general target
    //To do this, I need to set a timer, start it OnTrigger, add a collider as child, and... That's it?