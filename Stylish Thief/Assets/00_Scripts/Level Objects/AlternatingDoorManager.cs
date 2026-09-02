using System.Collections.Generic;
using UnityEngine;

public class AlternatingDoorManager : MonoBehaviour
{
    public float cycleTime;
    float timer;
    int currentDoor = 0;
    [SerializeField] private List<AlternatingDoor> doors;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<AlternatingDoor>() != null)
            {
                doors.Add(child.GetComponent<AlternatingDoor>());
            }
        }
        doors[currentDoor].AlternateDoorState();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < cycleTime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            doors[currentDoor].AlternateDoorState();
            timer = 0;
            currentDoor++;
            if (currentDoor == doors.Count)
            {
                currentDoor = 0;
            }
            doors[currentDoor].AlternateDoorState();
        }
    }
}
//Have a list of doors
//Tell the first door to perform its state swap function in Start
//In Update, wait for a timer to expire
//Reset the timer upon expiring
//When timer resets, give current door the instruction again
//Make current door the next one in the list
//Give the new current door the instruction