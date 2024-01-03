using UnityEngine;
using UnityEngine.UI;
using EasyButtons;
using System.Collections.Generic;

public class WaveformVisualizer : MonoBehaviour
{
    public RawImage waveformImage;
    public ZeroLine zeroLine;
    public TimeMarkers timeMarkers;
    public const int Padding = 10; // Padding from the edges

    AudioClip audioClip;

    private void Awake() 
    {
        waveformImage = GetComponentInChildren<RawImage>();
        zeroLine = GetComponentInChildren<ZeroLine>();
        timeMarkers = GetComponentInChildren<TimeMarkers>();
    }

    void Start()
    {
        LoopRecorder.OnNewAudioClipAvailable += SetAudioClip;
    }

    void SetAudioClip(AudioClip clip)
    {
        this.audioClip = clip;
        GenerateWaveform();
    }

    [Button]
    public void GenerateWaveform()
    {
        Texture2D waveformTexture = GenerateWaveformTexture(audioClip);
        waveformImage.texture = waveformTexture;

        // Update timeline texts
        WaveformTimelineText timelineTextScript = GetComponentInChildren<WaveformTimelineText>();
        if (timelineTextScript != null)
        {
            timelineTextScript.CreateTextElements(audioClip.length, GetComponent<RectTransform>());
        }
    }

    private Texture2D GenerateWaveformTexture(AudioClip clip)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        int width = (int)rectTransform.rect.width - 2 * Padding;
        int height = (int)rectTransform.rect.height - 2 * Padding;

        Texture2D texture = new Texture2D(width, height);

        // Fill the texture with a white background
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                texture.SetPixel(i, j, Color.white);
            }
        }

        zeroLine.DrawZeroLine(texture, width, height);
        timeMarkers.DrawTimeline(texture, width, height, clip.length);

        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        int step = Mathf.CeilToInt((float)samples.Length / width);
        for (int i = 0; i < width; i++)
        {
            float wavePeak = 0f;
            int baseSample = i * step;
            for (int j = 0; j < step && baseSample + j < samples.Length; j++)
            {
                wavePeak = Mathf.Max(wavePeak, Mathf.Abs(samples[baseSample + j]));
            }

            int middle = height / 2;
            int scaledHeight = (int)(wavePeak * middle);
            for (int j = middle - scaledHeight; j < middle + scaledHeight; j++)
            {
                texture.SetPixel(i, j, Color.blue);
            }
        }

        texture.Apply();
        return texture;
    }

 
}
