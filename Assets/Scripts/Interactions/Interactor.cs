using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public static bool Disabled {get; set;}
    IInteractable selectedInteractable;

    private void Update() 
    {
        if (Disabled) return;
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Tried to select: " + selectedInteractable);
            selectedInteractable?.Interact(transform.root.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (Disabled) return;
        if (other.TryGetComponent<IInteractable>(out IInteractable inter))
        {
            selectedInteractable?.OutOfRange();
            selectedInteractable = inter;
            selectedInteractable?.InRange();
        }
    }

    private void OnTriggerStay(Collider other) 
    {
        if (Disabled) return;
        if (selectedInteractable == null)
        {
            if (other.TryGetComponent<IInteractable>(out IInteractable t))
            {
                selectedInteractable = t;
                selectedInteractable.InRange();
            }
        }
        else
        {
            if (selectedInteractable.Canceled)
            {
                selectedInteractable.InRange();
            }
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (Disabled) return;
        if (other.GetComponent<IInteractable>() == selectedInteractable)
        {
            selectedInteractable?.OutOfRange();
            selectedInteractable = null;
        }
    }
}
