using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RickButtonChanger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            TMP_Text text = GetComponentInChildren<TMP_Text>();
            text.text = "BIG LEVEL";
            RickSceneChanger changer = GetComponent<RickSceneChanger>();
            changer.sceneID = 3;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            TMP_Text text = GetComponentInChildren<TMP_Text>();
            text.text = "SMALL LEVEL";
            RickSceneChanger changer = GetComponent<RickSceneChanger>();
            changer.sceneID = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
