using UnityEngine;
using UnityEngine.UI;

public class ButtonLightEffect : MonoBehaviour
{
    private Image buttonImage;
    private Color originalColor;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        originalColor = buttonImage.color;
    }

    public void TurnOnLight()
    {
        buttonImage.color = Color.yellow;
    }

    public void TurnOffLight()
    {
        buttonImage.color = originalColor;
    }
}
