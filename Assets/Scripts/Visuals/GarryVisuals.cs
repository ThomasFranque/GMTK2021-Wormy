using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;

public class GarryVisuals : MonoBehaviour
{
    [SerializeField] private GameObject _mesh;

    [Header("Worm Struggle Animation")]
    [SerializeField] private GameObject _wormStruggleParent;
    [SerializeField] private ParticleSystem _slurpParticles;
    [Header("Garry Movement")]
    [SerializeField] private StaticParticles _highFallParticles;
    [SerializeField] private StaticParticles _lowFallParticles;
    [SerializeField] private float fallHeight = 3f;

    [Header("Shooting")]
    [SerializeField] private ParticleSystem _shootParticles;

    [Header("Peekaboo")]
    [SerializeField] private GameObject _peekabooWormParent;

    [Header("Audio")]
    [SerializeField] private FMODUnity.StudioEventEmitter _grassHit;
    [SerializeField] private FMODUnity.StudioEventEmitter _grassWalk;
    [SerializeField] private FMODUnity.StudioEventEmitter _shootPop;
    [SerializeField] private FMODUnity.StudioEventEmitter _shootGarrySound;

    private GarryHole _hole;
    private GarryController _garryControl;
    private Coroutine _wormStruggleCor;
    private Vector3 _initialMeshScale;
    bool holdingScale;

    private bool _peekaBooing;
    private bool _peekaBooingObstructedSuppression;
    private float _peekTimer;

    private void Awake()
    {
        _hole = transform.root.GetComponent<GarryHole>();
        _garryControl = transform.root.GetComponent<GarryController>();
        _initialMeshScale = _mesh.transform.localScale;
        _peekTimer = UnityEngine.Random.Range(5f, 12f);
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

    private void Update()
    {
        DoScaleOnHoldBehavior();
        DoPeekabooBehaviour();
    }

    private void DoScaleOnHoldBehavior()
    {
        if (_hole.Held && !holdingScale)
        {
            holdingScale = true;
            LeanTween.scale(_mesh, _initialMeshScale * 1.15f, 1.5f).setEaseOutCirc();
        }
    }

    private void DoPeekabooBehaviour()
    {
        //! Swap to something that tells us that garry is inside
        bool garryInside = !GarryController.Disabled;
        SetPeekabooVisibility(garryInside);
        
        if (!garryInside) return;

        _peekTimer -= Time.deltaTime;
        if (_peekTimer <= 0)
        {
            Peekaboo(!_peekaBooing);
            _peekTimer = UnityEngine.Random.Range(5f, 12f);
        }
        
        // Check for obstruction
        if (_peekaBooing)
        {
            if (_hole.Obstructed())
            {
                Peekaboo(false);
                _peekaBooingObstructedSuppression = true;
            }
        }
        else if (_peekaBooingObstructedSuppression)
        {
            if (!_hole.Obstructed())
            {
                Peekaboo(true);
                _peekaBooingObstructedSuppression = false;
            }
        }

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

    private void Peekaboo(bool state)
    {
        GameObject peekWorm = _peekabooWormParent.transform.GetChild(0).gameObject;
        if (!_peekaBooing && state) // Pop out
        {
            _peekaBooing = state;
            LeanTween.cancel(peekWorm);
            LeanTween.moveLocalZ(peekWorm, -0.045f, 0.8f).setEaseOutBack();
        }
        else if (_peekaBooing && !state) // yeet in
        {
            _peekaBooing = state;
            LeanTween.cancel(peekWorm);
            LeanTween.moveLocalZ(peekWorm, -0.364f, 0.4f).setEaseOutCirc();
        }
    }

    private void SetPeekabooVisibility(bool state)
    {
        _peekabooWormParent.SetActive(state);
    }

    public void ShootEffects(bool shootWorm)
    {
        holdingScale = false;
        if (_mesh)
        {
            // Scale towards movement vector later
            LeanTween.cancel(_mesh);
            //_mesh.transform.localScale = _initialMeshScale * 0.8f;
            LeanTween.scale(_mesh, _initialMeshScale, 0.6f).setEaseOutBack();
        }

        if (shootWorm)
        {
            _shootParticles?.Play();
            _shootPop?.Play();
            if (_peekaBooing)
            {
                Peekaboo(false);
            }
            _peekaBooingObstructedSuppression = false;
        }

        _shootGarrySound?.Play();
    }

    public void GroundHit(float distance)
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, ~LayerMask.GetMask("Player"));
        Vector3 point = hit.point;
        point.y += 0.2f;

        if (distance >= fallHeight)
        {
            _highFallParticles.PlayPS(point, hit.normal);
            _grassHit?.Play();

            if (!holdingScale)
            {
                // Scale towards movement vector later
                LeanTween.cancel(_mesh);
                _mesh.transform.localScale = _initialMeshScale;
                LeanTween.scale(_mesh, _initialMeshScale * 1.4f, 0.4f).setEasePunch();
            }
        }
        else
        {
            _lowFallParticles.PlayPS(point, hit.normal);
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