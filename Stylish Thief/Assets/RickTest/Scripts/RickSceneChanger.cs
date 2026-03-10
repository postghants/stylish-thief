using UnityEngine;
using UnityEngine.SceneManagement;

public class RickSceneChanger : MonoBehaviour
{
    public int sceneID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneID);
    }
    public void ResetScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
