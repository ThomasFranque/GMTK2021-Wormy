using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class DialogueRenderer : MonoBehaviour
{
    private static DialogueRenderer current;
    public static event System.Action endCallback;
    [SerializeField] private CanvasGroup indicatorObject;
    [SerializeField] private CanvasGroup dialogueObject;
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private CinemachineVirtualCamera dialogueCamera;

    public static bool Displaying { get; private set; }
    private DialogueData currentlyShowing;
    private Camera mainCamera;
    private LTDescr indicatorTween;
    private LTDescr dialogueTween;

    private Transform activeIndicator;
    private Transform activeDialogue;
    private bool dialogueQueued;
    private int dialogueLine = 0;
    private int totalLines = 0;

    private void Awake()
    {
        current = this;
        mainCamera = Camera.main;

        indicatorObject.alpha = 0;
        dialogueObject.alpha = 0;
        SetIndicator(null);
    }

    private void OnEnable()
    {
        CheckForCameraBlending.onCameraBlendFinished += OnCameraReady;
    }

    private void OnDisable()
    {
        CheckForCameraBlending.onCameraBlendFinished -= OnCameraReady;
    }

    public static void Indicator(Transform point = null)
    {
        if (current == null) return;
        current.SetIndicator(point);
    }

    public static void Show(DialogueData dialogue, Transform point)
    {
        if (current == null) return;

        current.currentlyShowing = dialogue;
        current.totalLines = dialogue.lines.Length;
        current.dialogueLine = 0;
        Debug.Log("Dialogue: Started new dialogue");
        current.SetDialogue(point);
    }

    private void SetIndicator(Transform point)
    {
        activeIndicator = point;
        if (indicatorTween != null)
        {
            LeanTween.cancel(indicatorObject.gameObject);
            LeanTween.cancel(indicatorTween.id);
        }

        if (point == null)
        {
            indicatorObject.LeanAlpha(0f, 1f);
            indicatorTween = LeanTween.scale(indicatorObject.transform.GetChild(0).gameObject, new Vector3(0.001f, 0.001f, 1f), .4f).
                setEaseInBack().setOnComplete(() => indicatorObject.gameObject.SetActive(false));
            return;
        }
        else
        {
            indicatorObject.gameObject.SetActive(true);
            indicatorObject.transform.GetChild(0).localScale = new Vector3(1.4f, 1.4f, 1.4f);
        }

        SetPosition(indicatorObject.transform, point.position);
        indicatorObject.LeanAlpha(1f, 0.1f);
        indicatorTween = LeanTween.scale
            (indicatorObject.transform.GetChild(0).gameObject, new Vector3(2f, 2f, 1f), .8f).
            setEasePunch();
    }

    private void Update()
    {
        if (activeIndicator)
        {
            SetPosition(indicatorObject.transform, activeIndicator.position);
        }

        if (activeDialogue)
        {
            SetPosition(dialogueObject.transform, activeDialogue.position);
        }

        if (currentlyShowing)
        {
            if (Input.GetButtonDown("Fire1") && Displaying)
            {
                NextLine();
            }
        }
    }

    private void SetDialogue(Transform point)
    {
        activeDialogue = point;

        if (point == null)
        {
            dialogueCamera.gameObject.SetActive(false);
            GarryController.Disabled = false;

            indicatorObject.LeanAlpha(0f, 1f);
            dialogueTween = LeanTween.scale(dialogueObject.transform.GetChild(0).gameObject, new Vector3(0.001f, 0.001f, 1f), .4f).
                setEaseInBack().setOnComplete(() => dialogueObject.gameObject.SetActive(false));
            return;
        }

        dialogueObject.transform.GetChild(0).localScale = Vector3.one;
        dialogueObject.alpha = 0;
        GarryController.Disabled = true;

        dialogueCamera.LookAt = point;
        dialogueCamera.Follow = point;

        dialogueCamera.gameObject.SetActive(true);
        dialogueObject.gameObject.SetActive(true);
        NameTag tag = dialogueObject.GetComponentInChildren<NameTag>();
        tag.SetTag(currentlyShowing);
        SetPosition(dialogueObject.transform, point.position);
        NextLine();
        dialogueQueued = true;
    }

    /// <summary>
    /// Shows the dialogue
    /// </summary>
    private void OnCameraReady()
    {
        if (!dialogueQueued) return;

        dialogueQueued = false;

        dialogueObject.LeanAlpha(1.0f, 0.2f);
        dialogueTween = LeanTween.scale
            (dialogueObject.transform.GetChild(0).gameObject, new Vector3(1.5f, 1.5f, 1f), .8f).
            setEasePunch();

        Displaying = true;
    }

    private void NextLine()
    {
        if (dialogueLine >= totalLines)
        {
            dialogueCamera.gameObject.SetActive(false);
            GarryController.Disabled = false;

            indicatorObject.LeanAlpha(0f, 1f);
            dialogueTween = LeanTween.scale(dialogueObject.transform.GetChild(0).gameObject, new Vector3(0.001f, 0.001f, 1f), .4f).
                setEaseInBack().setOnComplete(() => dialogueObject.gameObject.SetActive(false));
            Displaying = false;

            endCallback?.Invoke();
            endCallback = null;
            return;
        }

        Debug.Log("Dialogue: " + dialogueLine);
        textBox.text = currentlyShowing.lines[dialogueLine++];

        if (dialogueTween != null)
        {
            LeanTween.cancel(dialogueTween.uniqueId);
        }

        dialogueTween = LeanTween.scale
            (dialogueObject.transform.GetChild(0).gameObject, new Vector3(1.2f, 1.2f, 1f), .8f).
            setEasePunch();
    }

    private void SetPosition(Transform obj, Vector3 position)
    {
        obj.position = mainCamera.WorldToScreenPoint(position);
    }
}
