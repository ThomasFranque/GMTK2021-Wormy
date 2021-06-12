using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventInteraction : MonoBehaviour, IInteractable
{
    public UnityEvent inRange;
    public UnityEvent outOfRange;
    public UnityEvent interact;
    public void InRange()
    {
        inRange?.Invoke();
    }

    public void Interact(GameObject interactor)
    {
        interact?.Invoke();
    }

    public void OutOfRange()
    {
        outOfRange?.Invoke();
    }

}
