using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class WalkAudio : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter walkEmitter;
    
    private GarryController controller;

    private void Awake() 
    {
        controller = GetComponentInParent<GarryController>();
    }

    private void FixedUpdate() 
    {
        if (controller.Velocity.magnitude > 0.6f && controller.Grounded && !walkEmitter.IsPlaying())
        {
            walkEmitter.Play();
        }
    }
}
