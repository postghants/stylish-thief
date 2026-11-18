using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIMovement : MonoBehaviour
{
    public bool move;
    public bool autoCycleTargets;
    public bool destroyOnEnd;
    //public UIMovementTargets[] targets;
    [Tooltip("An event that triggers at the start of this script")] public UnityEvent crimeNameStartEvent;
    [Tooltip("A list of places to go along with the speed to move at and how long to wait before moving. This shouldn't be done here but in the CrimeSpreeHUD script")] public List<UIMovementTargets> targets = new();
    [Tooltip("Events that play when reaching the targets. Match their IDs with their targets! This must be set here.")] public List<UnityEvent> events = new();
    [Header("Internal")]
    public int multipliedPoints;
    public float multiplier;
    public float totalPoints;
    public int currentTarget;
    public float delayTimer;
    public float currentInterpolation;
    public CrimeSpreeHUD HUD;

    private void Start()
    {
        crimeNameStartEvent?.Invoke();
    }
    void Update()
    {
        if (move)
        {
            if (currentTarget < targets.Count)
            {
                if (targets[currentTarget].target.position == transform.position)
                {
                    delayTimer = 0;
                    currentInterpolation = 0;
                    events[currentTarget]?.Invoke();
                    if (autoCycleTargets)
                    {
                        currentTarget++;
                    }
                }
                else
                {
                    if (delayTimer < targets[currentTarget].delay)
                    {
                        delayTimer += Time.deltaTime;
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(transform.position, targets[currentTarget].target.position, currentInterpolation);
                        if (currentInterpolation < 1)
                        {
                            currentInterpolation += targets[currentTarget].speed * Time.deltaTime;
                        }
                    }
                }
            }
            if (destroyOnEnd && currentTarget >= targets.Count)
            {
                Destroy(gameObject);
            }
        }
    }

    //Now follows a bunch of functions that we can expand at any time. They are to be used by this script's own events
    public void SetTextToMultipliedPoints()
    {
        GetComponent<TMP_Text>().text = "+" + multipliedPoints;
    }
    public void IncreaseComboCount()
    {
        HUD.multiplier.text = multiplier.ToString();
    }
    public void IncreasePointCount()
    {
        HUD.points.text = totalPoints.ToString();
    }
}
