using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using FMOD;
using FMODUnity;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private UnityEvent _onJumpOnButton;
    [SerializeField] private GarryController _garry;
    [SerializeField] private StudioEventEmitter _pressedSound;
    [SerializeField] private StudioEventEmitter _enteredSound;

    private bool _entered;
    private TextMeshPro _text;
    private string _originalText;

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshPro>();
        _originalText = _text.text;

        _garry.groundHit += TryActivation;
    }

    private void TryActivation(float maxHeight)
    {
        if (_entered)
        {
            _onJumpOnButton?.Invoke();
            _pressedSound.Play();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        _entered = true;
        _enteredSound.Play();
        _text.text = "> " + _originalText + " <";
    }
    private void OnTriggerExit(Collider other)
    {
        _entered = false;
        _text.text = _originalText;
    }
}
