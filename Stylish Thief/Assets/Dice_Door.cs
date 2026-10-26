using UnityEngine;

public class Dice_Door : MonoBehaviour
{
    public GameObject openDoor;
    public GameObject closedDoor;
    public int DoorNumber;
    public int RolledNumber;
    public Dice_Door_Manager DoorManager;

    public void Start()
    {
        DoorManager = GetComponentInParent<Dice_Door_Manager>();
    }


    void Update()
    {
        RolledNumber = GetComponentInParent<Dice_Door_Manager>().RolledNumber;
        if (RolledNumber == DoorNumber)
        {
            OpenDoorState();
        }
        else
        {
            CloseDoorState();
        }
    }

    public void OpenDoorState()
    {
        openDoor.SetActive(true);
        closedDoor.SetActive(false);
    }

    public void CloseDoorState()
    {
        closedDoor.SetActive(true);
        openDoor.SetActive(false);
    }
}
