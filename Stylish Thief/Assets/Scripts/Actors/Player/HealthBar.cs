using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Image image;
    void Start()
    {
        image = GetComponent<Image>();
    }

    public void SetFill(float fill)
    {
        image.fillAmount = fill;
    }
}
