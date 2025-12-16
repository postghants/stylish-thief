using UnityEngine;
using TMPro;

public class RickChangeReact : MonoBehaviour
{
    [Header("Growing and shrinking")]
    public float growthLimit;
    public float growSpeed;
    public float shrinkSpeed;
    bool growAndShrinking;
    bool grown;

    [Header("Hiding after seconds")]
    float maxTime;
    float currentTime;
    bool countingDown;

    [Header("Fade over time")]
    bool fading;
    float fadeRate;
    Color startColor;


    RectTransform textBox;
    TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text>();
        textBox = GetComponent<RectTransform>();
        startColor = text.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (growAndShrinking)
        {
            if (!grown)
            {
                textBox.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growSpeed;
                if (textBox.localScale.x > growthLimit)
                {
                    grown = true;
                }
            }
            else
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
        if (countingDown)
        {
            if (maxTime <= currentTime)
            {
                currentTime = 0;
                text.enabled = false;
            }
            else
            {
                currentTime += Time.deltaTime;
            }
        }
        if (fading)
        {
            text.color = new Vector4 (text.color.r, text.color.g, text.color.b, text.color.a) - new Vector4(0, 0, 0, 1) * Time.deltaTime * fadeRate;
        }
    }
    public void DoReaction(bool growAndShrink, float hideAfterSeconds, float fadeSpeed)
    {
        text.enabled = true;
        if (growAndShrink)
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

        if (hideAfterSeconds > 0)
        {
            maxTime = hideAfterSeconds;
            currentTime = 0;
            countingDown = true;
        }

        if (fadeSpeed > 0)
        {
            text.color = startColor;
            fading = true;
            fadeRate = fadeSpeed;
        }
    }
    
}
