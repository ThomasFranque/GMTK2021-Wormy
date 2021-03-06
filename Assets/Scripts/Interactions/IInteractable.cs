using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact(GameObject interactor);
    void InRange();
    void OutOfRange();
    bool Canceled {get; set;}
}
