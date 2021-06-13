using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;

public class GarryVisuals : MonoBehaviour
{
    [SerializeField] private GameObject _mesh;

    [Header("Worm Struggle Animation")]
    [SerializeField] private GameObject _wormStruggleParent;
    [SerializeField] private ParticleSystem _slurpParticles;
    [Header("Garry Movement")]
    [SerializeField] private StaticParticles _highFallParticles;
    [SerializeField] private float fallHeight = 3f;

    [Header("Shooting")]
    [SerializeField] private ParticleSystem _shootParticles;
    [Header("Audio")]
    [SerializeField] private FMODUnity.StudioEventEmitter _grassHit;
    [SerializeField] private FMODUnity.StudioEventEmitter _grassWalk;
    [SerializeField] private FMODUnity.StudioEventEmitter _shootPop;
    [SerializeField] private FMODUnity.StudioEventEmitter _shootGarrySound;

    private GarryHole _hole;
    private GarryController _garryControl;

    private Coroutine _wormStruggleCor;

    private void Awake()
    {
        _hole = transform.root.GetComponent<GarryHole>();
        _garryControl = transform.root.GetComponent<GarryController>();
    }

    private void OnEnable()
    {
        _hole.onShoot += ShootEffects;
        _garryControl.groundHit += GroundHit;

    }

    private void OnDisable()
    {
        _hole.onShoot -= ShootEffects;
        _garryControl.groundHit -= GroundHit;
    }

    public void WormEntered(Action animationEndCallback = default, WormVisuals wormVisuals = default)
    {
        if (_wormStruggleCor != default)
        {
            SetWormStruggleparent(false);
            StopCoroutine(_wormStruggleCor);
        }

        _wormStruggleCor = StartCoroutine(CWormStruggle(animationEndCallback));
    }

    public void ShootEffects(bool shootWorm)
    {
        if (_mesh)
        {
            LeanTween.scale(_mesh, Vector3.one * 1.4f, 0.8f).setEasePunch();
        }

        if (shootWorm && _shootParticles != null)
        {
            _shootParticles.Play();
            _shootPop?.Play();
        }
        
        _shootGarrySound?.Play();
    }

    public void GroundHit(float distance)
    {
        if (distance >= fallHeight)
        {
            Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, ~LayerMask.GetMask("Player"));
            Vector3 point = hit.point;
            point.y += 0.2f;

            _highFallParticles.PlayPS(point, hit.normal);
            _grassHit?.Play();
        }
    }

    private IEnumerator CWormStruggle(Action callback)
    {
        if (!_hole.Obstructed())
        {
            // Worm animation
            _wormStruggleParent.transform.localScale = Vector3.one;
            SetWormStruggleparent(true);
            LeanTween.scale(_wormStruggleParent, Vector3.one * 1.2f, 0.8f).setEasePunch();
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.8f, 1.8f));
            LeanTween.cancel(_wormStruggleParent);
            LeanTween.scale(_wormStruggleParent, Vector3.one * 0.01f, 0.4f).setEaseInBack().setOnComplete(WormEnteredEnd);
        }

        // Ploop animation
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