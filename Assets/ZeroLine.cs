using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroLine : MonoBehaviour
{

    public void DrawZeroLine(Texture2D texture, int width, int height)
    {
        int middle = height / 2;
        for (int i = 0; i < width; i++)
        {
            texture.SetPixel(i, middle, Color.black);
        }
    }

}
