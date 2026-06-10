using UnityEngine;

public class AlternatingDoor : MonoBehaviour
{
    public GameObject openDoor;
    public GameObject closedDoor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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
