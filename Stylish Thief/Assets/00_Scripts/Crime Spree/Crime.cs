using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Crime : MonoBehaviour
{
    [Header("Standard Crime Data")]
    [Tooltip("What tier is this crime? Number from 1 to 5, each with increasingly high time rewards and reset times")] public int crimeTier;
    [Tooltip("What is the legal name of this crime?")] public string crimeName;
    [Tooltip("How many points and multiplier does the player get from this?")] public int crimeValue;

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
            manager.currentSpreeTime += manager.generalCrimeData.miniGainedTime / 100 * manager.maxSpreeTime;
            resetTimer = manager.generalCrimeData.miniResetTime; count = true;

        }
    }
    public void DoMinorCrime(string crimeName, int crimeValue)
    {
        if (manager != null)
        {
            manager.RegisterCrimeName(crimeName); //Also to be logged later in a Spree record
            manager.AddPoints(crimeValue);
            manager.currentSpreeTime += manager.generalCrimeData.minorGainedTime / 100 * manager.maxSpreeTime;
            resetTimer = manager.generalCrimeData.minorResetTime; count = true;

        }
    }
    public void DoMiddleCrime(string crimeName, int crimeValue)
    {
        if (manager != null)
        {
            manager.RegisterCrimeName(crimeName); //Also to be logged later in a Spree record
            manager.AddPoints(crimeValue);
            manager.currentSpreeTime += manager.generalCrimeData.middleGainedTime / 100 * manager.maxSpreeTime;
            resetTimer = manager.generalCrimeData.middleResetTime; count = true;

        }
    }
    public void DoMajorCrime(string crimeName, int crimeValue)
    {
        if (manager != null)
        {
            manager.RegisterCrimeName(crimeName); //Also to be logged later in a Spree record
            manager.AddPoints(crimeValue);
            manager.currentSpreeTime += manager.generalCrimeData.majorGainedTime / 100 * manager.maxSpreeTime;
            resetTimer = manager.generalCrimeData.majorResetTime; count = true;

        }
    }
    public void DoMegaCrime(string crimeName, int crimeValue)
    {
        if (manager != null)
        {
            if (!manager.activeSpree) { manager.activeSpree = true; }

            manager.RegisterCrimeName(crimeName); //Also to be logged later in a Spree record
            manager.AddPoints(crimeValue);
            manager.currentSpreeTime = manager.maxSpreeTime;
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