using UnityEngine;

public class Bumper : MonoBehaviour
{
    public float speed;
    public Transform target;
    public Transform visualsToRotate;
    private PlayerStateDriver player;
    private float currentInterpolationValue;
    private float convertedSpeed;
    private bool launching;
    void Start()
    {
        visualsToRotate.transform.LookAt(target.transform);
    }

    void Update()
    {
        if (player != null) 
        {
            if (launching)
            {
                if (player.Root.Leaf().ToString() == "HSM.IntBumper")
                {
                    if (currentInterpolationValue < 1)
                    {
                        player.transform.position = Vector3.Lerp(transform.position, target.position, currentInterpolationValue);
                        currentInterpolationValue += convertedSpeed * Time.deltaTime; 
                    }
                    else
                    {
                        Debug.Log("Target reached. Transitioning to airborne");
                        player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne);
                        currentInterpolationValue = 0;
                        launching = false;
                    }
                }
                else
                {
                    launching = false;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.GetComponentInParent<PlayerStateDriver>();
            player.Machine.ChangeState(player.Root.Leaf(), player.Root.intBumper);

            player.ctx.rb.velocity = visualsToRotate.transform.forward * speed;
            convertedSpeed = speed / Vector3.Distance(transform.position, target.position);
            currentInterpolationValue = 0;
            launching = true;
        }
    }
    
}

//To figure out: How to convert a constant speed to a lerp value
//Both the world speed and the interpolation speed are in units per second
//I have the distance it needs to travel and the speed I'd like to travel at. 
//A distance of 40 is covered in 1 second by a speed of 40. A distance of 80 is covered in 2 seconds.
//I need to figure out how many seconds to cover the distance in to match the speed... Then I convert that amount of seconds to a value between 0 and 1? No right?

//Lerp goes from 0 to 100% percent of the total distance. The interpolation value is the percentage I set it to. I want to go from 0 to 100%, thus traveling the distance, at a specific rate.
//Ignoring deltaTime:
//Distance is my maximum speed. Entering a number higher than or equal to distance should move me to the end in a single update.
//Therefore, distance is equal to 100%. The speed I enter should be divided by distance and multiplied by 100. That's my movement in a single update.
//Then I just add those movements on top of each other every single update. That number is what I enter into the lerp value (divided by 100).