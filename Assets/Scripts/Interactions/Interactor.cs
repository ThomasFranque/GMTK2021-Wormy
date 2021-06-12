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
            Debug.Log("Tried to select: " + selectedInteractable);
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

    private void OnTriggerStay(Collider other) 
    {
        if (selectedInteractable == null)
        {
            if (other.TryGetComponent<IInteractable>(out IInteractable t))
            {
                selectedInteractable = t;
                selectedInteractable.InRange();
            }
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.GetComponent<IInteractable>() == selectedInteractable)
        {
            selectedInteractable?.OutOfRange();
            selectedInteractable = null;
        }
    }
}
