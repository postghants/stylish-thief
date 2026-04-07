using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RickTimerChallenge : MonoBehaviour
{
    [Header("Stopwatch version")]
    [Tooltip("Should this count up or down? Stopwatch makes it count up infinitely")] public bool stopwatch;
    [Tooltip("DON'T TOUCH THIS")] public bool countUp;
    [Tooltip("DON'T TOUCH THIS")] public float stopwatchTime;
    [SerializeField] private TMP_Text stopwatchText;
    [SerializeField] private RickChangeReact stopwatchTextReact;

    [Header("Countdown version")]
    [Tooltip("How long does the player get?")] public float givenTime;
    [Tooltip("DON'T TOUCH THIS")] public float currentTime;
    [Tooltip("DON'T TOUCH THIS")] public bool countdown;
    [SerializeField] private Image countdownUI;
    [SerializeField] private Image countdownUIBackdrop;
    [SerializeField] private RickChangeReact WinText;
    [SerializeField] private RickChangeReact LoseText;

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
            stopwatchText.text = stopwatchTime.ToString();
        }
        else if (countdown)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                countdownUI.fillAmount = 1 / givenTime * currentTime;
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
                stopwatchTextReact.DoReaction(true, 0, 0);
            }
            else if (!countdown)
            {
                countdown = true;
                countdownUI.enabled = true;
                countdownUIBackdrop.enabled = true;
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
            stopwatchTextReact.DoReaction(true, 2, 0);
        }
    }
    public void WinCountDown()
    {
        if (countdown)
        {
            countdown = false;
            Debug.Log("You made it");
            currentTime = givenTime;
            CrimeSpreeManager.instance.DoMinorCrime(givenScore, crime);
            countdownUI.enabled = false;
            countdownUIBackdrop.enabled = false;
            WinText.DoReaction(true, 2, 1);
        }
    }
    public void FailCountDown()
    {
        if (countdown)
        {
            countdown = false;
            Debug.Log("You failed");
            currentTime = givenTime;
            countdownUI.enabled = false;
            countdownUIBackdrop.enabled = false;
            LoseText.DoReaction(true, 2, 1);
        }
    }
    public void CancelCountDown()
    {
        if (countdown)
        {
            countdown = false;
            Debug.Log("Timer cancelled");
            currentTime = givenTime;
            countdownUI.enabled = false;
            countdownUIBackdrop.enabled = false;
        }
    }
}
