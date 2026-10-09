using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrimeSpreeHUD : MonoBehaviour
{
    public UIBar spreeTimer;
    public UIBar comboTimer;
    public TMP_Text points;
    public TMP_Text gainedPoints;
    public TMP_Text crimeName;
    public TMP_Text multiplier;

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
            comboTimer.SetFill(manager.comboTimer / manager.comboDuration);

            points.text = manager.points.ToString();
            gainedPoints.text = "+" + manager.lastGainedPointsRaw.ToString();
            crimeName.text = manager.lastCrimeCommitted;
            
            float decimalNumber = manager.currentMultiplier - Mathf.CeilToInt(manager.currentMultiplier);
            float number = manager.currentMultiplier - decimalNumber;
            if (manager.combo < 100)
            {
                multiplier.text = "<size=40%>x1.<size=100%>" + manager.combo.ToString();
            }
            else
            {
                multiplier.text = "<size=40%>x<size=100%>" + manager.currentMultiplier.ToString();
            }
        }
    }
}
