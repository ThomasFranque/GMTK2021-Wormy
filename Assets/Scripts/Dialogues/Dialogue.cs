using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour, IInteractable
{
    [SerializeField] List<DialogueData> randomDialogues;
    [SerializeField] private Transform popUpPosition;

    public DialogueData ForceNextDialogue {get; set;}
    private bool lastWasRandom;
    private DialogueData lastDialogue;
    public void InRange()
    {
        DialogueRenderer.Indicator(popUpPosition);
    }

    public void Interact(GameObject interactor)
    {
        DialogueData dialogue = NextDialogue();
        lastDialogue = dialogue;
        DialogueRenderer.Show(dialogue);
    }

    public void OutOfRange()
    {
        DialogueRenderer.Indicator();
    }

    private DialogueData NextDialogue()
    {
        if (ForceNextDialogue)
        {
            lastWasRandom = false;
            return ForceNextDialogue;
        }

        List<DialogueData> tempList = new List<DialogueData>(randomDialogues);
        if (lastDialogue)
        {
            tempList.Remove(lastDialogue);
        }

        lastWasRandom = true;
        return tempList[Random.Range(0, tempList.Count)];
    }
}
