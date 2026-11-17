using UnityEngine;

public class Number_Detector : MonoBehaviour
{

    Dice_Roll dice;
    void Start()
    {
        dice = GetComponentInParent<Dice_Roll>();
    }


    private void OnTriggerStay(Collider other)
    {
        if (dice != null)
        {
            if (dice.GetComponent<Rigidbody>().angularVelocity == Vector3.zero)
            {
                dice.DiceNumber = int.Parse(this.name);
            }
        }
    }

}
