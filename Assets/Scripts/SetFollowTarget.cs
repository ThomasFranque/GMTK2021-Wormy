using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class SetFollowTarget : MonoBehaviour
{
    CinemachineVirtualCamera vc;


    // Start is called before the first frame update
    void Start()
    {
        vc = GetComponent<CinemachineVirtualCamera>();

        if (FollowTarget.Current != null)
        {
            vc.LookAt = FollowTarget.Current.transform;
            vc.Follow = FollowTarget.Current.transform;
        }
        else
        {
            Debug.LogWarning("Your scene is missing a FollowTarget aimed at garry, add an object with the FollowTarget component");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
