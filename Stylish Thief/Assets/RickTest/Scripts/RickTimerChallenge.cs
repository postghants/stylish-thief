using UnityEngine;

public class RickTimerChallenge : MonoBehaviour
{
    [Header("Stopwatch version")]
    [Tooltip("Should this count up or down? Stopwatch makes it count up infinitely")] public bool stopwatch;
    [Tooltip("DON'T TOUCH THIS")] public bool countUp;
    [Tooltip("DON'T TOUCH THIS")] public float stopwatchTime;

    [Header("Countdown version")]
    [Tooltip("How long does the player get?")] public float givenTime;
    [Tooltip("DON'T TOUCH THIS")] public float currentTime;
    [Tooltip("DON'T TOUCH THIS")] public bool countDown;

    [Header("Crime stuff")]
    [Tooltip("How many points does the player get from this?")] public float givenScore;
    [Tooltip("What is the name of this crime?")] public string crime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stopwatchTime = 0;
        currentTime = givenTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (stopwatch && countUp)
        {
            stopwatchTime += Time.deltaTime;
        }
        else if (countDown)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                FailCountDown();
            }
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
            else if (!countDown)
            {
                countDown = true;
            }
        }
    }

    public void StopStopwatch()
    {
        if (countUp)
        {
            countUp = false;
            Debug.Log(stopwatchTime);
            stopwatchTime = 0;

        }
    }
    public void WinCountDown()
    {
        if (countDown)
        {
            countDown = false;
            Debug.Log("You made it");
            currentTime = givenTime;
            CrimeSpreeManager.instance.DoMinorCrime(givenScore, crime);

        }
    }
    public void FailCountDown()
    {
        if (countDown)
        {
            countDown = false;
            Debug.Log("You failed");
            currentTime = givenTime;

        }
    }
    public void CancelCountDown()
    {
        if (countDown)
        {
            countDown = false;
            Debug.Log("Timer cancelled");
            currentTime = givenTime;

        }
    }
}
