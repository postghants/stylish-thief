using System;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CrimeSpreeHUD : MonoBehaviour
{
    public Vector2 crimeNameStartPos;

    public UIBar spreeTimer;
    public TMP_Text points;
    public TMP_Text committedCrime;
    public TMP_Text multiplier;

    public Image[] lamps;

    [Header("Internal")]
    public GameObject crimeNamePrefab;
    public GameObject crimeNameInstance;
    public UIMovement movement;

    public UIMovementTargets[] crimeNameTargets;

    private NeoCrimeSpreeManager manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GetComponentInParent<NeoCrimeSpreeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (manager.activeSpree)
        {
            spreeTimer.SetFill(manager.currentSpreeTime / manager.maxSpreeTime);
            //comboTimer.SetFill(manager.comboTimer / manager.comboDuration);

            Lamps();

            points.text = manager.points.ToString();
            
            float decimalNumber = manager.currentMultiplier - Mathf.CeilToInt(manager.currentMultiplier);
            float number = manager.currentMultiplier - decimalNumber;
            if (manager.currentMultiplier < 100)
            {
                multiplier.text = /*"<size=40%>x1.<size=100%>" + */manager.currentMultiplier.ToString();
            }
            else
            {
                multiplier.text = /*"<size=40%>x<size=100%>" + */manager.currentMultiplier.ToString();
            }
            
        }
    }
    private void Lamps()
    {
        if (manager.comboTimer < manager.comboDuration)
        {
            for (int i = 0; i < lamps.Length; i++)
            {
                if (i <= manager.comboTimer / manager.comboDuration * lamps.Length)
                {
                    lamps[i].enabled = true;
                }
                else
                {
                    lamps[i].enabled = false;
                }
            }
        }
        else
        {
            foreach (Image lamp in lamps)
            {
                lamp.enabled = false;
            }
        }
    }
    public void SpawnCrime(string name, float points)
    {
        if (movement != null)
        {
            if (movement.currentTarget == 0) { movement.delayTimer = 99; }
        }
        crimeNameInstance = Instantiate(crimeNamePrefab, transform.position, Quaternion.identity);
        movement = crimeNameInstance.GetComponent<UIMovement>();
        crimeNameInstance.transform.parent = gameObject.transform;
        crimeNameInstance.GetComponent<TMP_Text>().text = "+" + points.ToString() + " <size=40%>" + name;
        movement.multipliedPoints = manager.lastGainedPointsMultiplied;

        foreach (var item in crimeNameTargets)
        {
            movement.targets.Add(item);
        }
    }
}

//
//If position of spawned ui isn't position of target, keep increasing lerp position
