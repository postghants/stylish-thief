using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIMovement : MonoBehaviour
{
    public bool destroyOnEnd;
    //public UIMovementTargets[] targets;
    public List<UIMovementTargets> targets = new();
    [Header("Internal")]
    public int multipliedPoints;
    public int currentTarget;
    public float delayTimer;
    public float currentInterpolation;



    void Update()
    {
        if (currentTarget < targets.Count)
        {
            if (targets[currentTarget].target.position == transform.position)
            {
                delayTimer = 0;
                currentInterpolation = 0;
                targets[currentTarget].targetReachedAction?.Invoke();
                currentTarget++;
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

    //Now follows a bunch of functions that we can expand at any time. They are to be used by this script's own events
    public void SetTextToMultipliedPoints()
    {
        GetComponent<TMP_Text>().text = "+" + multipliedPoints;
    }
}

[Serializable]
public struct UIMovementTargets
{
    public Transform target;
    public float speed;
    public float delay;
    public UnityEvent targetReachedAction;
}