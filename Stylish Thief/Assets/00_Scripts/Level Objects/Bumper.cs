using UnityEngine;

public class Bumper : MonoBehaviour
{
    public float speed;
    public Transform target;
    private PlayerStateDriver player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            float percentage = 0;
            percentage += speed / Vector3.Distance(transform.position, target.position) * Time.deltaTime;
            player.transform.position = Vector3.LerpUnclamped(transform.position, target.position, percentage);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.GetComponentInParent<PlayerStateDriver>();
        }
    }
}

//Want: Percentage to move each frame
//Have: 100% being the theoretical maximum
//Have: The distance from A to B
//Have: Distance to travel each frame
//Percentage to move = distance to travel / total distance * 100
//Final number needs to be /100 since it's a range from 0 to 1. Just remove the * 100 at the end
//Every frame, add the tiny percentage number to the total percentage