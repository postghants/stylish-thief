using UnityEngine;

public class RickTimeStopper : MonoBehaviour
{
    public bool doOnUpdate;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (doOnUpdate)
        {
            Time.timeScale = 0;
        }
    }
}
