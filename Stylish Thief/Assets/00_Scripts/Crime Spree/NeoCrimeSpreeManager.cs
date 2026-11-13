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
    [Tooltip("Name of the last Crime committed")] public string lastCrimeCommitted;
    [Tooltip("How high the player's combo is. This doubles as the multiplier")] public float combo;
    [Tooltip("How high the player's combo is")] public float comboRaw;
    [Tooltip("Current combo multiplier")] public float currentMultiplier;
    [Tooltip("How long the combo currently has left")] public float comboTimer;

    public CrimeData crimeData;
    [Tooltip("Is there a Crime Spree happening right now?")] public bool activeSpree;
    public GameObject spreeHUD;
    public GameObject gameOverUIPrefab;

    public PlayerStateDriver player;

    private void Start()
    {
        if (player == null)
        {
            PlayerStateDriver playerToFind = FindAnyObjectByType<PlayerStateDriver>();
            player = playerToFind;
        }
        currentSpreeTime = maxSpreeTime;
        comboTimer = comboDuration;
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

            if (combo > 0)
            {
                comboTimer -= Time.deltaTime;
                if (comboTimer < 0)
                {
                    combo = 0;
                    comboTimer = comboDuration;
                }
            }
        }
        else
        {
            spreeHUD.SetActive(false);
        }
        currentMultiplier = 1 + (combo * multiplierStrength);
    }

    public void AddPoints(int pointsToGain)
    {
        lastGainedPointsRaw = pointsToGain;     //Logging this for UI

        if (combo != 0)
        {
            points += Mathf.CeilToInt(pointsToGain * currentMultiplier);
        }
        else
        {
            points += pointsToGain;
        }
        comboRaw += pointsToGain;
        currentMultiplier = 1 + comboRaw * multiplierStrength;
        comboTimer = comboDuration;
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