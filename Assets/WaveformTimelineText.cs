using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WaveformTimelineText : MonoBehaviour
{
    private const int NumTextElements = 5; // Number of text elements to display
    private List<GameObject> createdTextElements = new List<GameObject>();

    public Font textFont; // Assign a font in the inspector

    public void CreateTextElements(float clipLength, RectTransform parentRectTransform)
    {
        ClearOldTextElements();
        float width = parentRectTransform.rect.width - 2 * WaveformVisualizer.Padding;
        float pixelsPerSecond = width / clipLength;

        // Calculate the interval for placing the text elements
        float interval = clipLength / (NumTextElements + 1);

        for (int i = 1; i <= NumTextElements; i++)
        {
            float second = interval * i;
            int xPosition = Mathf.RoundToInt(second * pixelsPerSecond) + WaveformVisualizer.Padding;

            GameObject textObj = new GameObject("Timestamp_" + i, typeof(Text));
            textObj.transform.SetParent(this.transform, false);

            Text text = textObj.GetComponent<Text>();
            text.text = Mathf.RoundToInt(second).ToString();
            text.alignment = TextAnchor.UpperCenter;
            text.color = Color.black;
            text.font = textFont; // Set the font

            // Adjust the font size and position
            text.fontSize = 12; // Reduced font size
            text.rectTransform.sizeDelta = new Vector2(50, 20);
            
            // Adjusted to align above the marker
            float verticalOffset = -30; // Adjust this value as needed to align with the markers
            text.rectTransform.anchoredPosition = new Vector2(xPosition - parentRectTransform.rect.width / 2, verticalOffset);

            createdTextElements.Add(textObj);
        }
    }

    private void ClearOldTextElements()
    {
        foreach (var textElement in createdTextElements)
        {
            Destroy(textElement);
        }
        createdTextElements.Clear();
    }
}
