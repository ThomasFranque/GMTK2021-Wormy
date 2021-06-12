using UnityEngine;
using Cinemachine;
 
[RequireComponent(typeof(CinemachineBrain))]
 
public class CheckForCameraBlending : MonoBehaviour
{
    public delegate void CameraBlendStarted();
    public static event CameraBlendStarted onCameraBlendStarted;
 
    public delegate void CameraBlendFinished();
    public static event CameraBlendFinished onCameraBlendFinished;
 
 
    private CinemachineBrain cineMachineBrain;
 
    private bool wasBlendingLastFrame;
 
    void Awake()
    {
        cineMachineBrain = GetComponent<CinemachineBrain>();
    }
    void Start()
    {
        wasBlendingLastFrame = false;
    }
 
    void Update()
    {
        if (cineMachineBrain.IsBlending)
        {
            if (!wasBlendingLastFrame)
            {
                if (onCameraBlendStarted != null)
                {
                    onCameraBlendStarted();
                }
            }
 
            wasBlendingLastFrame = true;
        }
        else
        {
            if (wasBlendingLastFrame)
            {
                if (onCameraBlendFinished != null)
                {
                    onCameraBlendFinished();
                }
                wasBlendingLastFrame = false;
            }
        }
    }
}