using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Crime : MonoBehaviour
{
    private NeoCrimeSpreeManager manager;
    private float resetTimer = 0;
    public UnityEvent resetCrime;
    private bool count;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = NeoCrimeSpreeManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (resetTimer > 0 && count)
        {
            resetTimer -= Time.deltaTime;
        }
        else if (count)
        {
            resetCrime?.Invoke();
            count = false;
        }
    }
    public void DoMiniCrime(string crimeName, int crimeValue)
    {
        if (manager != null)
        {
            manager.RegisterCrimeName(crimeName); //Also to be logged later in a Spree record
            manager.AddPoints(crimeValue);
            manager.currentSpreeTime += manager.crimeData.miniGainedTime / 100 * manager.maxSpreeTime;
            resetTimer = manager.crimeData.miniResetTime; count = true;

        }
    }
    public void DoMinorCrime(string crimeName, int crimeValue)
    {
        if (manager != null)
        {
            manager.RegisterCrimeName(crimeName); //Also to be logged later in a Spree record
            manager.AddPoints(crimeValue);
            manager.currentSpreeTime += manager.crimeData.minorGainedTime / 100 * manager.maxSpreeTime;
            resetTimer = manager.crimeData.minorResetTime; count = true;

        }
    }
    public void DoMiddleCrime(string crimeName, int crimeValue)
    {
        if (manager != null)
        {
            manager.RegisterCrimeName(crimeName); //Also to be logged later in a Spree record
            manager.AddPoints(crimeValue);
            manager.currentSpreeTime += manager.crimeData.middleGainedTime / 100 * manager.maxSpreeTime;
            resetTimer = manager.crimeData.middleResetTime; count = true;

        }
    }
    public void DoMajorCrime(string crimeName, int crimeValue)
    {
        if (manager != null)
        {
            manager.RegisterCrimeName(crimeName); //Also to be logged later in a Spree record
            manager.AddPoints(crimeValue);
            manager.currentSpreeTime += manager.crimeData.majorGainedTime / 100 * manager.maxSpreeTime;
            resetTimer = manager.crimeData.majorResetTime; count = true;

        }
    }
    public void DoMegaCrime(string crimeName, int crimeValue)
    {
        if (manager != null)
        {
            manager.RegisterCrimeName(crimeName); //Also to be logged later in a Spree record
            manager.AddPoints(crimeValue);
            manager.currentSpreeTime = manager.maxSpreeTime;
            if (!manager.activeSpree) { manager.activeSpree = true; }

        }
    }
    /// <summary>
    /// Tier 1 through 5 are mini, minor, middle, major, and mega respectively. Update this tooltip if other tiers are made
    /// </summary>
    public void DoCrimeOfTier(int tier, string crimeName, int crimeValue)
    {
        if (tier == 1)
        {
            DoMiniCrime(crimeName, crimeValue);
        }
        else if (tier == 2)
        {
            DoMinorCrime(crimeName, crimeValue);
        }
        else if (tier == 3)
        {
            DoMiddleCrime(crimeName, crimeValue);
        }
        else if (tier == 4)
        {
            DoMajorCrime(crimeName, crimeValue);
        }
        else if (tier == 5)
        {
            DoMegaCrime(crimeName, crimeValue);
        }
    }
}
