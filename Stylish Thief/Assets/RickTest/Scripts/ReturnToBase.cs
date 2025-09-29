using UnityEngine;

public class ReturnToBase : MonoBehaviour
{
    public float selectedSpeed;
    bool launching;
    int timesHit = 0;

    private void FixedUpdate()
    {
        if (launching)
        {
            GetLaunched(selectedSpeed);
        }
    }
    public void InitiateLaunch(float speed)
    {
        transform.localPosition = Vector3.zero;
        launching = true;
        selectedSpeed = speed;
    }
    public void GetLaunched(float speed)
    {
        transform.Translate(Vector3.forward * speed);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.ToString() == "6");
        {
            timesHit++;
            Debug.Log("Hits: " + timesHit);
        }
        if (other.gameObject.name.Contains("Player"))
        {

        }
    }
}
