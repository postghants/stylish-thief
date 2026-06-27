using UnityEngine;
using UnityEngine.SceneManagement;

public class RickSceneChanger : MonoBehaviour
{
    public int sceneID;
    MusicSystem musicSystem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicSystem = FindFirstObjectByType<MusicSystem>();
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
    public void DisablePauseScreen()
    {
        if (GetComponentInParent<PauseScreen>() != null)
        {
            GetComponentInParent<PauseScreen>().pauseScreen.gameObject.SetActive(false);
            Debug.Log("Pause Screen disabled");
        }
    }
    public void BackToTitle()
    {
        Debug.Log("serotbun");
        musicSystem.DeleteMusic();
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneID);
    }
}
