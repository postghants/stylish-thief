using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RickChangeReact : MonoBehaviour
{
    public bool isText;
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


    RectTransform rect;
    TMP_Text text;
    Image image;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isText)
        {
            text = GetComponent<TMP_Text>();
            rect = GetComponent<RectTransform>();
            startColor = text.color;
        }
        else
        {
            image = GetComponent<Image>();
            rect = GetComponent<RectTransform>();
            startColor = image.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isText)
        {
            if (growAndShrinking)
            {
                if (!grown)
                {
                    rect.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growSpeed;
                    if (rect.localScale.x > growthLimit)
                    {
                        grown = true;
                    }
                }
                else
                {
                    rect.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * shrinkSpeed;
                    if (rect.localScale.x < 1)
                    {
                        rect.localScale = new Vector3(1, 1, 1);
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
                text.color = new Vector4(text.color.r, text.color.g, text.color.b, text.color.a) - new Vector4(0, 0, 0, 1) * Time.deltaTime * fadeRate;
            }
        }
        else
        {
            if (growAndShrinking)
            {
                if (!grown)
                {
                    rect.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growSpeed;
                    if (rect.localScale.x > growthLimit)
                    {
                        grown = true;
                    }
                }
                else
                {
                    rect.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * shrinkSpeed;
                    if (rect.localScale.x < 1)
                    {
                        rect.localScale = new Vector3(1, 1, 1);
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
                    image.enabled = false;
                }
                else
                {
                    currentTime += Time.deltaTime;
                }
            }
            if (fading)
            {
                image.color = new Vector4(image.color.r, image.color.g, image.color.b, image.color.a) - new Vector4(0, 0, 0, 1) * Time.deltaTime * fadeRate;
            }
        }
    }
    public void DoReaction(bool growAndShrink, float hideAfterSeconds, float fadeSpeed)
    {
        if (isText)
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
                    rect.localScale = new Vector3(1, 1, 1);
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
        else
        {
            image.enabled = true;
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
                    rect.localScale = new Vector3(1, 1, 1);
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
                image.color = startColor;
                fading = true;
                fadeRate = fadeSpeed;
            }
        }
        
    }
    
}
