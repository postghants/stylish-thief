using System;
using UnityEngine;

public class NeoCrimeSpreeManager : Singleton<NeoCrimeSpreeManager>
{
    [Header("Chase")]
    [Tooltip("How much time the player can have")] public float maxSpreeTime;
    [Tooltip("How much time the player has right now")] public float currentSpreeTime;

    [Tooltip("How many points the player has")] public int points;
    [Tooltip("How long the combo currently has left")] public float lastGainedPointsRaw;
    [Tooltip("How long the combo currently has left")] public string lastCrimeCommitted;
    [Tooltip("How high the player's combo is")] public int combo;
    [Tooltip("Should gained points be affected by a multiplier during a combo?")] public bool doMultiplier;
    [Tooltip("How strongly your combo multiplies new gained points")] public float multiplierStrength;
    [Tooltip("Current combo multiplier")] public float currentMultiplier;
    [Tooltip("How long a combo stays without being reset")] public float comboDuration;
    [Tooltip("How long the combo currently has left")] public float comboTimer;

    public CrimeData crimeData;
    [Tooltip("Is there a Crime Spree happening right now?")] public bool activeSpree;
    public GameObject spreeHUD;
    public GameObject gameOverUIPrefab;

    public PlayerStateDriver player;

    private void Start()
    {
        if (player  == null)
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
        currentMultiplier =  1+ (combo * multiplierStrength);
    }

    public void AddPoints(int pointsToGain)
    {
        lastGainedPointsRaw = pointsToGain;
        if (!doMultiplier)
        {
            points += pointsToGain;
        }
        else
        {
            if (combo != 0)
            {
                points += Mathf.FloorToInt(pointsToGain * currentMultiplier);
            }
            else
            {
                points += pointsToGain;
            }
        }
    }
    public void RegisterCrimeName(string name)
    {
        lastCrimeCommitted = name;
    }
    public void ResetComboTimer(int addition)
    {
        combo += addition;
        comboTimer = comboDuration;
        Debug.Log("Setting combo");
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
        //enabled = false;
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

    [Header("Combo additions")]
    public int miniComboAddition; //is zero
    public int minorComboAddition;
    public int middleComboAddition;
    public int majorComboAddition;
    public int megaComboAddition;
}