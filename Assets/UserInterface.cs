
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    private Image image;

    void Awake() 
    {
        image = GetComponent<Image>();
    }
    
    void Start()
    {
        AudioTrigger.OnStartThresholdCrossed += () => SetColor(Color.red);
        AudioTrigger.OnStopThresholdCrossed += () => SetColor(Color.green);
    }

    void SetColor(Color color)
    {
        image.color = color;
    }
}
