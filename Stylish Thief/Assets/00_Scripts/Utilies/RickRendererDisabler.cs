using UnityEngine;

public class RickRendererDisabler : MonoBehaviour
{
    MeshRenderer meshRenderer;
    public GameObject disableTarget;
    public int timeScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GetComponent<MeshRenderer>() != null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DisableObjectOfChoice()
    {
        Time.timeScale = timeScale;
        disableTarget.SetActive(false);
    }
}
