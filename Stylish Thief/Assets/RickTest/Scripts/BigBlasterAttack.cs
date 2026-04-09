using HSM;
using System;
using UnityEngine;
using UnityEngine.AI;

public class BigBlasterAttack : MonoBehaviour//EnemyAttack
{
    [Header("Big Blast")]
    //public BigBlasterJumpData bigBlastData;
    public JumperContext jumperContext = new();
    //public float dropkickSpeed;
    //public float recoveryTime = 4;

    //public float hitboxDelay;
    //public float hitboxActiveTime;
    //public float grabDamage;
    //public float grabKbHorizontal;
    //public float grabKbVertical;

    private float timer = 0;
    //private float recoveryTimer = 0;
    //private bool playedRecovery = false;

    [Header("References")]
    private NavMeshAgent agent;
    private ActorPhysics rb;
    [SerializeField] private Transform rotationTf;
    //public Collider grabHitbox;

    void Start() //Looks like I just don't touch Start, apart from adding things. All of this seems to always apply to every enemy
    {
        agent = GetComponent<NavMeshAgent>(); //Register navmesh agent
        rb = GetComponent<ActorPhysics>(); //Register actorphysics rb
        jumperContext.rb = rb; //Register that the jumper here is in fact this object's actorphysics script
        //if (ctr == null)
        {
            if (TryGetComponent(out EnemyController controller)) //Reads and registers the controller, followed by saving the animation IDs
            {
                //ctr = controller;
                //ctr.AddAnimationNames(animationCodeNames);
            }
        }
    }

    //public override void OnEnter()
    //{
        //agent.enabled = false;
        //timer = 0;
        //ctr.PlayAnimation("JumpAttack");
        //jumperContext.currentJumpData = bigBlasterData;
    //}

    private /*public override*/ void WouldaBeenOnEnter() //Damn what's the override do? Anyway this piece runs each time you initiate this script. The script seems to always stay active, so start wouldn't work for this!
    {
        timer = 0; //Reset timers, just like you would on Start
        //ctr.PlayAnimation("JumpAttack"); //Plays an animation from its animator by name
        agent.enabled = false; //Deactivate pathfinding! Important!!! Otherwise your little buddy will go careening off during their attack
        //jumperContext.currentJumpData = bigBlasterData; //Set the Jumper logic's data to that of your enemy. This explains a bit of confusion. "DropKickerJumpData" is a script found elsewhere, so I need to make my own later
        //EnemyJump.PerformJump(jumperContext); //Appears to tell a separate script to perform the actual jump portion of the attack. It's based on the old player jump. Contains no mentions of attacking
        //I guess this means that the Dropkicker is coded to always jump instantly upon the activation of its attack script, with the actual attack logic coming later
        rb.velocity += jumperContext.currentVelocity; //Seems to take the current speed and add... the current speed? Ah this just contains a Y value! Nevermind. Currently irrelevant for me. I'll skip other jump stuff

        //Vector3 dist = ctr.ctx.player.transform.position - rb.transform.position; //Make a vector and set it to the exact distance on all axes between the player and the enemy
        //dist.y = 0; //Delete the Y value... I always do this in an unwieldly extra calculation inside the initial vector creation whenever I do this... Why didn't I do it like this? It's so clean
        //rb.velocity += /*dropkickSpeed **/ dist.normalized; //Now set the enemy's speed to the desired speed, and multiply it by the normalised distance to give it a direction (toward the player)

        //Vector3 lookPos = ctr.ctx.player.transform.position; //Set up a Vector 3 and make it represent the player's location
        //lookPos.y = agent.transform.position.y; //Reset the Y value of lookPos to just be the enemy's height, preventing them from looking up or down
        //rotationTf.LookAt(lookPos); //Now just make a child containing all the visuals rotate to lookPos (real useful for Big Blaster)
    }

    void OnUpdate()
    {
        
    }

    private void WouldaBeenOnUpdate() //So... What's the difference here? Does OnUpdate only run as long as the script is called, while Update runs regardless of it all?
    {
        timer += Time.deltaTime; //Timer starts counting immediately! Remember that this stuff only runs during the actual attack, so no waiting for a funciton to be called. This is the function
        //Wait huh? Just deltaTime? It's giving an error here so is there some kinda reference to Time elsewhere in the script to make it part of this script's pool of variables?
        //Now follows mostly animation logic. This will be an issue for later! My only concern right now is getting each function in place
        //ctr.ExitAttack(this); //Stops this script from being executed, ending the attack sequence. Seems to be a custom function from somewhere. Gotta make sure this script is reset properly in OnUpdate
        //More jump stuff. Basically just continues (by repeating) the functions called in the OnEnter function
        
        //if (timer > hitboxDelay) //Now this one's useful. I can just copy this straight up (as it seems to be from the Grab to the Dropkick). Times hitbox activation and deactivation. Neat!
        { //I'm commenting all of it out for now since I don't want this giving errors in my cheat sheet
            /*if (timer < hitboxDelay + hitboxActiveTime)
            {
                grabHitbox.gameObject.SetActive(true);
            }
            else
            {
                grabHitbox.gameObject.SetActive(false);
            }*/
        }

        rb.isGrounded = rb.IsGrounded(); //Good to know I can do ground checks like this!
        if (rb.isGrounded)
        {
            
        }
    }

    private void WouldaBeenOnHit() //This seems to be called by an Event Trigger on the relevant hitbox. Gotta rewrite the attack to work like this. Perhaps absorb it into this script instead
    {
        //Vector3 kb = rb.velocity * grabKbHorizontal;
        //kb.y = grabKbVertical;
        //ctr.ctx.player.TakeKnockback(kb);
        //ctr.ctx.player.TakeDamage(grabDamage);
    }
    //public override void OnExit() //Seems like this is called when the script is stopped, no matter what. Its last hurrah. Or in other words: I don't have to worry about resetting it. Just do it here.
    //{
        //timer = 0;
        //agent.enabled = true;
        //recoveryTimer = 0;
        //playedRecovery = false;
        //rb.velocity = Vector3.zero;
        //rb.isGrounded = false;
        //grabHitbox.gameObject.SetActive(false);
    }

    //protected override void Reset() //Now what the fuck is this? Seems to add animations? No clue what protected and override mean. Seems to set "base" (EnemyAttack) back to its default values
    //{
        //animationCodeNames.Add("JumpAttack");
        //animationCodeNames.Add("JumpAttackRecoverTrigger");
        //base.Reset();
    //}
//}
//Uses Jumper system to follow you
//Faster than player
//Chooses a random spot around you to pathfind towards
//When area reached, do a full attack

//To do in this script:
//When attacking, disable pathfinding
//Then spawn or enable the telegraph decal (maybe spawn the whole attack separately but instantly make it a child of the enemy to make tracking work easily) (def do enable to prevent lag spikes)
//Then track player horizontally for a few seconds, rotating the decal too
//Then stop tracking for a few seconds
//Then spawn or enable the beam
//When the beam stops, wait a few seconds
//End script and activate pathfinding... in opposite order