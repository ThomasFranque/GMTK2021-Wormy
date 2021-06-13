using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour, IInteractable
{
    [SerializeField] private List<DialogueData> firstTimeDialogues;
    [SerializeField] List<DialogueData> randomDialogues;
    [SerializeField] private Transform popUpPosition;

    public DialogueData ForceNextDialogue { get; set; }
    public bool LastWasRandom { get; private set; }
    public bool Canceled { get; set; }

    private DialogueData lastDialogue;
    public void InRange()
    {
        DialogueRenderer.Indicator(popUpPosition);
        Canceled = false;
    }

    public void Interact(GameObject interactor)
    {
        DialogueData dialogue = NextDialogue();
        lastDialogue = dialogue;
        Canceled = false;
        DialogueRenderer.Show(dialogue, popUpPosition);
        DialogueRenderer.endCallback += EndInteract;
        Interactor.Disabled = true;
    }

    public void EndInteract()
    {
        if (lastDialogue.giveLeaf && !UniversalGameData.dialoguesWithLeaf.Contains(lastDialogue))
        {
            UniversalGameData.dialoguesWithLeaf.Add(lastDialogue);
            UniversalGameData.Leafs++;
        }
        
        LeanTween.delayedCall(gameObject, 0.1f, () =>
        {
            GarryController.Disabled = false;
            Interactor.Disabled = false;
        });

        Canceled = true;
    }

    public void OutOfRange()
    {
        DialogueRenderer.Indicator();
        Canceled = true;
    }

    private DialogueData NextDialogue()
    {
        if (ForceNextDialogue)
        {
            LastWasRandom = false;
            return ForceNextDialogue;
        }

        if (firstTimeDialogues != null && firstTimeDialogues.Count > 0)
        {
            DialogueData d = firstTimeDialogues[0];
            firstTimeDialogues.Remove(d);
            LastWasRandom = false;
            lastDialogue = d;
            return d;
        }

        List<DialogueData> tempList = new List<DialogueData>(randomDialogues);
        if (lastDialogue && tempList.Count > 1 && tempList.Contains(lastDialogue))
        {
            tempList.Remove(lastDialogue);
        }

        LastWasRandom = true;
        return tempList[Random.Range(0, tempList.Count)];
    }
}
