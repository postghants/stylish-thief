using FMODUnity;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Random = UnityEngine.Random;

public class NeoCrimeSpreeManager : Singleton<NeoCrimeSpreeManager>
{
    [Header("Chase")]
    [Tooltip("How much time the player can have")] public float maxSpreeTime;
    [Tooltip("How much time the player has right now")] public float currentSpreeTime;

    [Tooltip("How many points the player has")] public int points;
    [Tooltip("How high the player's combo is")] public int combo;
    [Tooltip("Should gained points be affected by a multiplier during a combo?")] public bool doMultiplier;
    [Tooltip("How strongly your combo multiplies new gained points")] public float multiplierStrength;
    [Tooltip("Current combo multiplier")] public float currentMultiplier;
    [Tooltip("How long a combo stays without being reset")] public float comboDuration;
    [Tooltip("How long the combo currently has left")][SerializeField] private float comboTimer;

    public CrimeData crimeData;
    [Tooltip("Is there a Crime Spree happening right now?")] public bool activeSpree;

    public PlayerStateDriver player;

    private void Start()
    {
        PlayerStateDriver playerToFind = FindAnyObjectByType<PlayerStateDriver>();
        player = playerToFind;
        currentSpreeTime = maxSpreeTime;
        comboTimer = comboDuration;
        activeSpree = true;
    }
    private void Update()
    {
        if (activeSpree)
        {
            currentSpreeTime -= Time.deltaTime;
            if (currentSpreeTime < 0)
            {
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
        currentMultiplier = combo * multiplierStrength;
    }

    public void AddPoints(int pointsToGain)
    {
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
    public void ResetComboTimer(int addition)
    {
        combo += addition;
        comboTimer = comboDuration;
        Debug.Log("Setting combo");
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
    //public int miniComboAddition; DOES NOT EXIST
    public int minorComboAddition;
    public int middleComboAddition;
    public int majorComboAddition;
    public int megaComboAddition;
}