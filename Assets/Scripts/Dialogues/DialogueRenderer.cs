using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueRenderer : MonoBehaviour
{
    private static DialogueRenderer current;
    private DialogueData currentlyShowing;

    private void Awake() 
    {
        current = this;
    }

    public static void Indicator(Transform point = null)
    {
        if (current == null) return;
    }

    public static void Show(DialogueData dialogue)
    {
        if (current == null) return;

        current.currentlyShowing = dialogue;
        
    }
}
