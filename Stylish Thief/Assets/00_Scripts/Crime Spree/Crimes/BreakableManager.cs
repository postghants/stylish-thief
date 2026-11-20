using System.Collections.Generic;
using UnityEngine;

public class BreakableManager : MonoBehaviour
{
    public float horizontalSpeedLimit;
    public float verticalSpeedLimit;
    [SerializeField] private List<Breakable> breakables;
    private PlayerStateDriver player;
    private float playerHorizontalSpeed;
    private float playerVerticalSpeed;
    private bool brokeHorizontalSpeedLimit;
    private bool brokeVerticalSpeedLimit;
    private bool horizontalSignalSent;
    private bool verticalSignalSent;

    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Breakable>() != null)
            {
                breakables.Add(child.GetComponent<Breakable>());
            }
        }
        if (NeoCrimeSpreeManager.instance != null)
        {
            player = NeoCrimeSpreeManager.instance.player;
        }
        else
        {
            player = FindFirstObjectByType<PlayerStateDriver>();
        }
    }

    void Update()
    {
        if (player != null)
        {
            Vector3 playerHorizontalVelocity = player.ctx.currentVelocity;
            playerHorizontalVelocity.y = 0;
            playerHorizontalSpeed = playerHorizontalVelocity.magnitude;
            playerVerticalSpeed = Mathf.Abs(player.ctx.currentVelocity.y);

            if (!horizontalSignalSent)                                                          //If we haven't given the breakables a signal with the current state yet
            {
                foreach(Breakable child in breakables)                                          //Every single Breakable in the list of breakables
                {
                    child.TriggerTrigger(brokeHorizontalSpeedLimit, brokeVerticalSpeedLimit);   //Perform the function with the current value of whether or not the speed limit is broken right now
                }
                horizontalSignalSent = true;                                                    //We've sent the signal!
            }
            if (!verticalSignalSent)
            {
                foreach(Breakable child in breakables)
                {
                    child.TriggerTrigger(brokeHorizontalSpeedLimit, brokeVerticalSpeedLimit);
                }
                verticalSignalSent = true;
            }

            if (playerHorizontalSpeed >= horizontalSpeedLimit && !brokeHorizontalSpeedLimit)    //If this is true and it hasn't been true before
            {
                brokeHorizontalSpeedLimit = true;                                               //Register that is has now happened AKA it has been true before
                horizontalSignalSent = false;                                                   //We haven't given this new signal to the breakables yet
            }
            else if (playerHorizontalSpeed < horizontalSpeedLimit && brokeHorizontalSpeedLimit) //If above is false and it hasn't been false before
            {
                brokeHorizontalSpeedLimit = false;                                              //Register that is has now happened AKA it has been false before
                horizontalSignalSent = false;                                                   //We haven't given this new signal to the breakables yet
            }


            if (playerVerticalSpeed >= verticalSpeedLimit && !brokeVerticalSpeedLimit)
            {
                brokeVerticalSpeedLimit = true;
                verticalSignalSent = false;
            }
            else if (playerVerticalSpeed < verticalSpeedLimit && brokeVerticalSpeedLimit)
            {
                brokeVerticalSpeedLimit = false;
                verticalSignalSent = false;
            }
        }
        else
        {
            if (NeoCrimeSpreeManager.instance != null)
            {
                player = NeoCrimeSpreeManager.instance.player;
            }
            else
            {
                PlayerStateDriver playerToFind = FindAnyObjectByType<PlayerStateDriver>();
                player = playerToFind;
            }
        }
    }
}
