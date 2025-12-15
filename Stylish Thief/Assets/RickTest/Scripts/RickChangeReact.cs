using UnityEngine;
using TMPro;

public class RickChangeReact : MonoBehaviour
{
    public float growthLimit;
    public float growSpeed;
    public float shrinkSpeed;
    bool growAndShrinking;
    bool grown;
    RectTransform textBox;
    TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text>();
        textBox = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (growAndShrinking && !grown)
        {
            textBox.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growSpeed;
            if (textBox.localScale.x > growthLimit)
            {
                grown = true;
            }
        }
        else if (growAndShrinking && grown)
        {
            textBox.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * shrinkSpeed;
            if (textBox.localScale.x < 1)
            {
                textBox.localScale = new Vector3(1, 1, 1);
                growAndShrinking = false;
                grown = false;
            }
        }
    }
    public void GrowAndShrink()
    {
        if (!growAndShrinking)
        {
            growAndShrinking = true;
        }
        else
        {
            growAndShrinking = true;
            grown = false;
            textBox.localScale = new Vector3(1, 1, 1);
        }
    }
}
