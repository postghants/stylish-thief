using System;
using UnityEngine;

public class NeoCrimeSpreeManager : Singleton<NeoCrimeSpreeManager>
{
    [Header("Crime Spree settings")]
    [Tooltip("How much time the player can have")] public float maxSpreeTime;
    [Tooltip("How much time the player has right now")] public float currentSpreeTime;
    [Tooltip("How strongly do gained points affect the combo multiplier?")] public float multiplierStrength;
    [Tooltip("How long a combo stays without being reset")] public float comboDuration;
    [Tooltip("Whether to multiply all the extra points immediately, or to store them and multiply them at once with the current bigger multiplier")] public bool doMultiplicationInOneGo;

    [Header("Internal")]
    [Tooltip("How many points the player has")] public int points;
    [Tooltip("How many points the last Crime gave without multiplier effects. Used for UI")] public float lastGainedPointsRaw;
    [Tooltip("How many points the last Crime gave without multiplier effects. Used for UI")] public int lastGainedPointsMultiplied;
    [Tooltip("Name of the last Crime committed")] public string lastCrimeCommitted;
    //[Tooltip("How high the player's combo is. This doubles as the multiplier")] public float combo;
    [Tooltip("How high the player's combo is")] public float comboRaw;
    [Tooltip("Current combo multiplier")] public float currentMultiplier = 1;
    [Tooltip("How long the combo currently has left")] public float comboTimer;
    [Tooltip("Is there a Crime Spree happening right now?")] public bool activeSpree;

    [Header("References")]
    public CrimeData crimeData;
    public GameObject spreeHUD;
    public GameObject gameOverUIPrefab;
    public PlayerStateDriver player;
    public CrimeSpreeHUD hud;

    private void Start()
    {
        if (player == null)
        {
            PlayerStateDriver playerToFind = FindAnyObjectByType<PlayerStateDriver>();
            player = playerToFind;
        }
        currentSpreeTime = maxSpreeTime;
        comboTimer = comboDuration;
        hud = GetComponentInChildren<CrimeSpreeHUD>();
    }
    private void Update()
    {
        if (activeSpree)
        {
            spreeHUD.SetActive(true);
            currentSpreeTime -= Time.deltaTime;
            if (currentSpreeTime < 0)
            {
                LoseSpree();
                activeSpree = false;
            }
            else if (currentSpreeTime > maxSpreeTime)
            {
                currentSpreeTime = maxSpreeTime;
            }

            if (currentMultiplier > 1)
            {
                comboTimer -= Time.deltaTime;
                if (comboTimer < 0)
                {
                    currentMultiplier = 1;
                    comboRaw = 0;
                    comboTimer = comboDuration;
                }
            }
        }
        else
        {
            spreeHUD.SetActive(false);
        }
    }

    public void AddPoints(int pointsToGain)
    {
        if (activeSpree)
        {
            lastGainedPointsRaw = pointsToGain;     //Logging this for UI

            if (currentMultiplier != 1)
            {
                lastGainedPointsMultiplied = Mathf.CeilToInt(pointsToGain * currentMultiplier);
                points += lastGainedPointsMultiplied;
            }
            else
            {
                lastGainedPointsMultiplied = pointsToGain;
                points += pointsToGain;
            }
            comboRaw += pointsToGain;
            currentMultiplier = 1 + comboRaw * multiplierStrength;
            comboTimer = comboDuration;
            hud.SpawnCrime(lastCrimeCommitted, lastGainedPointsRaw, points, currentMultiplier);
        }
    }
    public void RegisterCrimeName(string name)
    {
        lastCrimeCommitted = name;
    }
    public void DealDamage(float lostTime)
    {
        if (!player.ctx.iFramesOn)
        {
            currentSpreeTime -= lostTime;
            player.ctx.iFramesOn = true;
        }
    }
    private void LoseSpree()
    {
        Instantiate(gameOverUIPrefab);

        Cursor.lockState = CursorLockMode.None;
    }
}

[Serializable]
public class CrimeData
{
    [Header("Gained Spree Time")]
    public float miniGainedTime;
    public float minorGainedTime;
    public float middleGainedTime;
    public float majorGainedTime;
    public float megaGainedTime;

    [Header("Object Reset Times")]
    public float miniResetTime;
    public float minorResetTime;
    public float middleResetTime;
    public float majorResetTime;
    //public float megaResetTime; DOES NOT EXIST
}