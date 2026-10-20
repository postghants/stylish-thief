using UnityEngine;
using UnityEngine.SceneManagement;

public class RickSceneChanger : MonoBehaviour
{
    public int sceneID;

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
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneID);
    }
}
