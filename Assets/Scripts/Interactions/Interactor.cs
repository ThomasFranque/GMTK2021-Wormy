using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    IInteractable selectedInteractable;

    private void Update() 
    {
        if (Input.GetButtonDown("Fire1"))
        {
            selectedInteractable?.Interact(transform.root.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.TryGetComponent<IInteractable>(out IInteractable inter))
        {
            selectedInteractable?.OutOfRange();
            selectedInteractable = inter;
            selectedInteractable?.InRange();
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        selectedInteractable?.OutOfRange();
        selectedInteractable = null;
    }
}
