using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class WalkAudio : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter walkEmitter;
    
    private FMOD.Studio.EventInstance walkInstance;
    private GarryController controller;
    private float volume;
    private void Awake()
    {
        controller = GetComponentInParent<GarryController>();
    }

    private void Start() 
    {
        walkInstance = walkEmitter.EventInstance;
    }

    private void FixedUpdate()
    {
        if (controller.Velocity.magnitude > 0.4f && controller.Grounded && !walkEmitter.IsPlaying())
        {
            volume = Mathf.Clamp(controller.Velocity.magnitude * 0.6f, 0, 1);
            walkEmitter.SetParameter("Volume", volume);
            Debug.Log(volume);
            walkEmitter.Play();
        }
    }

    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }
}
