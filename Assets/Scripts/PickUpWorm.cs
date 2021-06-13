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

    private void OnTriggerEnter(Collider other) 
    {
        // need the controller for this
        if (other.transform.root.CompareTag("Worm"))
        {
            grabber?.PickWorm(other.transform.root.gameObject);
            Debug.Log("Worm pick up");
        }
    }
}
