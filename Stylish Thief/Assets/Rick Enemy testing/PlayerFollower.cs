using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    public GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = player.transform.position;
    }
    private void FixedUpdate()
    {
        gameObject.transform.position = player.transform.position;
    }
}
