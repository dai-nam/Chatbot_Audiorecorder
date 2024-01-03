using System.Collections;
using UnityEngine;
using EasyButtons;
using System;

public class LoopRecorder : MonoBehaviour
{

#region attributes and properties
    public static LoopRecorder Instance;
    public  bool isActive = false;
    public  bool isRecording = false;
    private AudioSource audioSource;

    private string currentMicDevice = null;
    int ringbufferLengthInSeconds = 10;
    public float timer = 0f;
    [SerializeField] int trimmedLengthInSmps = 192000;
    private int id = 0;
    public static event Action<AudioClip> OnNewAudioClipAvailable;

    private AudioClip rawRecording;
    [HideInInspector] public AudioClip rawRecordingCopy;
    [HideInInspector] public AudioClip trimmedRecording;
    private int sampleRate = 48000;

    private int startIndex;
    private int endIndex;
    [Range(-500f, 3000f)] public int preRollInMs = 0;
    [Range(-500f, 1000f)] public int postRollInMs = 0;
#endregion


    void Start()
    {
        Instance = this;
        SetActive(true);
        audioSource = GetComponent<AudioSource>();
        SetMicrophone();
        Record();

        AudioTrigger.OnStartThresholdCrossed += StartNewRecording;
        AudioTrigger.OnStopThresholdCrossed += StopRecording;
    }

    void Update() 
    {
        IncrementTimer();
    }

    private void SetMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            currentMicDevice = Microphone.devices[0];
        }
        else
        {
            Debug.LogError("No microphone devices found.");
        }
    }


    void IncrementTimer()
    {
        timer += Time.deltaTime;
        if (timer >= ringbufferLengthInSeconds)
        {
            timer = 0f;
        }
    }

  void Record()
    {
        rawRecording = Microphone.Start(Microphone.devices[0], true, ringbufferLengthInSeconds, sampleRate);
        rawRecordingCopy = AudioClip.Create(rawRecording.name+"_copy", rawRecording.samples, rawRecording.channels, rawRecording.frequency, false);
    }

    void StartNewRecording()
    {
        isRecording = true;
        print("Recording started...");

        startIndex = SecondsToSmps(timer);
        id++;
        rawRecording.name = "rawRecording_"+id;
    }


    void StopRecording()
    {
        isRecording = false;
        SetActive(false);  
        print("Recording stopped!");
       // Microphone.End(Microphone.devices[0]);
        endIndex = SecondsToSmps(timer);

        float[] copy = new float[rawRecording.samples];
        rawRecording.GetData(copy, 0);
        rawRecordingCopy.SetData(copy, 0);
        TrimRecording();
    }

    [Button]
    void TrimRecording()
    {
        trimmedLengthInSmps = endIndex > startIndex ? endIndex - startIndex : rawRecordingCopy.samples + (endIndex - startIndex);
        int totalLength = trimmedLengthInSmps + MsToSmps(preRollInMs+postRollInMs);

        trimmedRecording = AudioClip.Create("trimmedRecording", totalLength, 1, sampleRate, false);
        float[] rawData = new float[rawRecordingCopy.samples];
        rawRecordingCopy.GetData(rawData, 0);
        float[] trimmedData = new float[trimmedRecording.samples];

        for(int i = 0; i < trimmedData.Length; i++)
        {
            int j = GetIndex(i);
            trimmedData[i] = rawData[j];
        }

        trimmedRecording.SetData(trimmedData, 0);
        OnNewAudioClipAvailable?.Invoke(trimmedRecording);
    }

    private int MsToSmps(int valueInMs)
    {
        return (int) (valueInMs*(sampleRate/1000f));
    }

  private int SecondsToSmps(float valueInSeconds)
    {
        return (int) (valueInSeconds*sampleRate);
    }

    private int GetIndex(int i)
    {
        i = i+startIndex-MsToSmps(preRollInMs);

        if(i < 0)
            return rawRecordingCopy.samples + i;
        else
            return i % rawRecordingCopy.samples;
    }

    void Play(AudioClip clip)
    {
        StopMicrophone();
        audioSource.clip = clip;
        audioSource.Play();
    }

    [Button]
    void PlayRaw()
    {
        StopMicrophone();
        Play(rawRecording);
    }

    [Button]
    void PlayTrimmed()
    {
        StopMicrophone();
        Play(trimmedRecording);
    }

    void SetActive(bool param)
    {
        isActive = param;
    }

    [Button]
    void StopMicrophone()
    {
       Microphone.End(Microphone.devices[0]);
    }



}
