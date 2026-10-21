using UnityEngine;

public class AlternatingDoor : MonoBehaviour
{
    public GameObject openDoor;
    public GameObject closedDoor;
    void Start()
    {
        
    }

    void Update()
    {
        
    }


    public void AlternateDoorState()
    {
        if (openDoor.activeSelf)
        {
            closedDoor.SetActive(true);
            openDoor.SetActive(false);
        }
        else
        {
            openDoor.SetActive(true);
            closedDoor.SetActive(false);
        }
    }
}
