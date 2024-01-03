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
    float framerate = 500f; //roughly set the framerate here in which the project runs on your machine. The Attack and release times are dependent on this value.

    private Queue<float> attackLevels;
    private Queue<float> releaseLevels;

    private int attack;
    private int release;

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
        ResetAttackLevels(300);
        ResetReleaseLevels(1000);
    }



    private void TrackInputLevel(float level)
    {
        if(!LoopRecorder.Instance.isActive)
        {
            return;
        }

        attackLevels.Dequeue();
        releaseLevels.Dequeue();

        attackLevels.Enqueue(level);
        releaseLevels.Enqueue(level);

        if(attackLevels.Average() > attackThreshold && !LoopRecorder.Instance.isRecording)
        {
            OnStartThresholdCrossed?.Invoke();
            ResetAttackLevels(300);
        }
        else if(releaseLevels.Average() < releaseThreshold && LoopRecorder.Instance.isRecording)
        {
            OnStopThresholdCrossed?.Invoke();
            ResetReleaseLevels(1000);
        }
    }   


    void ResetAttackLevels(int attInMs)
    {
        attack = (int) ((attInMs / 1000f) * framerate); //attack in frames
        attackLevels = new Queue<float>();
        for (int i = 0; i < attack; i++)
        {
            attackLevels.Enqueue(0f);
        }
    }

    void ResetReleaseLevels(int relInMs)
    {
   
        release = (int)((relInMs / 1000f) * framerate); //release in frames
        releaseLevels = new Queue<float>();
        for (int i = 0; i < release; i++)
        {
            releaseLevels.Enqueue(1f);
        }
    }




}






