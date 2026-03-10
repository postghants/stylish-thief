using UnityEngine;

public class RickRevealer : MonoBehaviour
{
    public int timeScale;
    public GameObject enableTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RevealObject()
    {
       Time.timeScale = timeScale;
       enableTarget.SetActive(true);
    }
}
