using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarryVisuals : MonoBehaviour
{
    [SerializeField] private GameObject _mesh;

    [Header("Worm Struggle Animation")]
    [SerializeField] private GameObject _wormStruggleParent;
    [SerializeField] private ParticleSystem _slurpParticles;

    private Coroutine _wormStruggleCor;

    private void Start()
    {
        //WormEntered(default);
    }

    public void WormEntered(Action animationEndCallback = default, WormVisuals wormVisuals = default)
    {
        if (_wormStruggleCor != default)
            StopCoroutine(_wormStruggleCor);
        _wormStruggleCor = StartCoroutine(CWormStruggle(animationEndCallback));
    }

    private IEnumerator CWormStruggle(Action callback)
    {
        yield return new WaitForSeconds(1);
        _wormStruggleParent.transform.localScale = Vector3.one;
        SetWormStruggleparent(true);
        LeanTween.scale(_wormStruggleParent, Vector3.one * 1.2f, 0.8f).setEasePunch();
        yield return new WaitForSeconds(3);
        LeanTween.cancel(_wormStruggleParent);
        LeanTween.scale(_wormStruggleParent, Vector3.one * 0.01f, 0.4f).setEaseInBack().setOnComplete(WormEnteredEnd);
        LeanTween.scale(_mesh, Vector3.one * 1.3f, 0.6f).setEasePunch().setDelay(0.2f);
        _slurpParticles.Play();
        callback?.Invoke();
        _wormStruggleCor = default;
    }

    private void WormEnteredEnd()
    {
        SetWormStruggleparent(false);
    }

    private void SetWormStruggleparent(bool state)
    {
        _wormStruggleParent.SetActive(state);
    }

}