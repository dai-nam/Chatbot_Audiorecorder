using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMarkers : MonoBehaviour
{
    public void DrawTimeline(Texture2D texture, int width, int height, float clipLength)
    {
        int timelineHeight = 20; // Height of the timeline area
        int startY = height - timelineHeight;
        float pixelsPerSecond = width / clipLength; // Pixels per second

        for (int second = 0; second <= clipLength; second++)
        {
            int xPosition = Mathf.RoundToInt(second * pixelsPerSecond);
            if (xPosition < width) // Ensure we don't draw outside the texture
            {
                DrawTimeMarker(texture, xPosition, startY, timelineHeight, second);
            }
        }
    }

    public void DrawTimeMarker(Texture2D texture, int x, int startY, int timelineHeight, int second)
    {
        int markerHeight = 10; // Height of the marker
        for (int i = startY; i < startY + markerHeight; i++)
        {
            texture.SetPixel(x, i, Color.black);
        }
    }
}
