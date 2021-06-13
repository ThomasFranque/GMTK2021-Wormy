using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpWorm : MonoBehaviour
{
    [SerializeField] private bool jumpPickUp;
    
    private IWormGrabber grabber;
    private void Awake() 
    {
        grabber = GetComponent<IWormGrabber>();
    }

    private void OnCollisionEnter(Collision other) 
    {
        // need the controller for this

        if (other.gameObject.CompareTag("Worm"))
        {
            grabber?.PickWorm(other.gameObject);
        }
    }
}
