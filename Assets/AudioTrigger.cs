using System;
using System.Linq;
using UnityEngine;
using EasyButtons;
using System.Collections;
using System.Collections.Generic;

public class AudioTrigger : MonoBehaviour
{
    #region attributes and properties
    public static event Action OnStartThresholdCrossed;
    public static event Action OnStopThresholdCrossed;

    [Range(0f, 1f)] public float attackThreshold = 0.5f;
    [Range(0f, 1f)] public float releaseThreshold = 0.5f;
    float avgFrameRate;

    //private float[] attackLevels;
    //private float[] releaseLevels;
    private Queue<float> attackLevels;
    private Queue<float> releaseLevels;

    int fps = 50;   //init value to avoid division by 0


    private int attack;
    public int Attack
    {
        get => attack;  
        set
        {
            attack = value;
            attackLevels = new Queue<float>(value);
            for (int i = 0; i < value; i++)
            {
                attackLevels.Enqueue(0f);
            }
        }
    }

    private int release;
    public int Release
    {
        get => release;
        set
        {
            release = value;
            releaseLevels = new Queue<float>(value);
            for (int i = 0; i < value; i++)
            {
                releaseLevels.Enqueue(1f);
            }   
        }
    }

    public float inputLevel;
   public float InputLevel
   {
        get => inputLevel;
        set 
        { 
            inputLevel = value;    //values between 0. and 1.
            TrackInputLevel(value); //is called once every frame, because inputSignal is updated on every frame (not sample rate!)
        } 
    }

#endregion

    void Awake() 
    {
        SetAttackRelease(100, 1000);
    }

    void Start()
    {
        StartCoroutine(CalculateRunningAverageOfFps());
    }
    
    void Update()
    {
        fps++;
    }


    private void TrackInputLevel(float level)
    {
        if(!LoopRecorder.Instance.isActive)
        {
            return;
        }

        attackLevels.Enqueue(level);
        releaseLevels.Enqueue(level);

        if(attackLevels.Average() > attackThreshold && !LoopRecorder.Instance.isRecording)
        {
            OnStartThresholdCrossed?.Invoke();
        }
        else if(releaseLevels.Average() < releaseThreshold && LoopRecorder.Instance.isRecording)
        {
            OnStopThresholdCrossed?.Invoke();        
        }
    }   


    void SetAttackRelease(int attInMs, int relInMs)
    {
        if(attInMs <= 0 || relInMs <= 0)
        {
            Debug.LogError("Attack and Release must be greater than 0");
            return;
        }

        Attack = (int) ((attInMs / 1000f) * avgFrameRate);
        Release = (int) ((relInMs / 1000f) * avgFrameRate);
    }



    private IEnumerator CalculateRunningAverageOfFps()
    {
        const float updateTime = 1f;
        Queue<float> values = new Queue<float>();
        const int MaxValues = 5;

        while(true)
        {
            if (values.Count > MaxValues)
            {
                values.Dequeue();
            }
            values.Enqueue(fps);
            fps = 0;

            avgFrameRate = values.Average();
            //print("Average fps: "+avgFrameRate);
            yield return new WaitForSeconds(updateTime);
        }
    }


}






