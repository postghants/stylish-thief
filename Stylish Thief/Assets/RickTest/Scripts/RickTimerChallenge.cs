using UnityEngine;

public class RickTimerChallenge : MonoBehaviour
{
    [Header("Stopwatch version")]
    public bool stopwatch;
    public bool countUp;
    public float stopwatchTime;

    //[Header("Countdown version")]

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stopwatchTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (stopwatch && countUp)
        {
            stopwatchTime += Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            if (stopwatch && !countUp)
            {
                countUp = true;
            }
            else
            {

            }
        }
    }

    public void StopStopwatch()
    {
        countUp = false;
        Debug.Log(stopwatchTime);
        stopwatchTime = 0;
    }
}
