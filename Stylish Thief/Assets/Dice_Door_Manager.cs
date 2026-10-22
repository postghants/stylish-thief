using System.Collections.Generic;
using UnityEngine;

public class Dice_Door_Manager : MonoBehaviour
{
    [SerializeField] private List<Dice_Door> doors;
    [SerializeField] private List<Dice_Roll> dice;

    public int RolledNumber;

    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Dice_Door>() != null)
            {
                doors.Add(child.GetComponent<Dice_Door>());
            }
        }

        foreach (Transform child in transform)
        {
            if (child.GetComponent<Dice_Roll>() != null)
            {
                dice.Add(child.GetComponent<Dice_Roll>());
            }
        }
    }

    void Update()
    {

    }


}
