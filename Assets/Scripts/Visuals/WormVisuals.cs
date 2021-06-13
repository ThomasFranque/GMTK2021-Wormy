using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormVisuals : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FIMSpace.FTail.TailAnimator2 _tailAnimator;
    [SerializeField] private Renderer _targetRenderer;
    [SerializeField] private Transform _headProps;
    [SerializeField] private Transform _headPropHolder;

    [Header("Visuals tweak")]
    [SerializeField] private Texture2D[] _faces;
    [SerializeField, Tooltip("X: Min, Y: Max")] private Vector2 _scaleRange = new Vector2(0.5f, 1.5f);
    [SerializeField, Tooltip("X: Min, Y: Max")] private Vector2 _hueRange = new Vector2(-0.8f, 0.1f);

    [Header("Locks")]
    [SerializeField] private bool _lockFace;
    [SerializeField] private bool _lockHue;
    [SerializeField] private bool _lockBlendShapes;
    [SerializeField] private bool _lockScale = true;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (!_lockHue)
            RandomizeHue();
        if (!_lockFace)
            RandomizeFace();
        if (!_lockBlendShapes)
            RandomizeShapeKeys();
        if (!_lockScale)


        _headProps.SetParent(_headPropHolder);
    }

    private void RandomizeHue()
    {
        _targetRenderer.materials[0].SetFloat("_Hue", Random.Range(_hueRange.x, _hueRange.y));

    }

    private void RandomizeFace()
    {
        _targetRenderer.materials[1].SetTexture("_MainTex", _faces[Random.Range(0, _faces.Length)]);
    }

    private void RandomizeShapeKeys()
    {
        SkinnedMeshRenderer skinned = _targetRenderer as SkinnedMeshRenderer;
        for (int i = 0; i < skinned.sharedMesh.blendShapeCount; i++)
        {
            skinned.SetBlendShapeWeight(i, Random.value * 100);
        }
    }

    private void RandomizeScale()
    {
        transform.localScale = Vector3.one * Random.Range(_scaleRange.x, _scaleRange.y);
    }

    // Animator events
    public void ActivateTailAnimator()
    {
        if (TailAmmountAnimatorCor != default)
            StopCoroutine(TailAmmountAnimatorCor);
        TailAmmountAnimatorCor = StartCoroutine(CAnimateTailAnimatorAmount(1));
    }
    public void DeActivateTailAnimator()
    {
        if (TailAmmountAnimatorCor != default)
            StopCoroutine(TailAmmountAnimatorCor);
        TailAmmountAnimatorCor = StartCoroutine(CAnimateTailAnimatorAmount(0, 1.9f));
    }
    
    public void ActivateTailAnimator(float to, float speedModifer)
    {
        if (TailAmmountAnimatorCor != default)
            StopCoroutine(TailAmmountAnimatorCor);
        TailAmmountAnimatorCor = StartCoroutine(CAnimateTailAnimatorAmount(to, speedModifer));
    }
    public void DeActivateTailAnimator(float speedModifer)
    {
        if (TailAmmountAnimatorCor != default)
            StopCoroutine(TailAmmountAnimatorCor);
        TailAmmountAnimatorCor = StartCoroutine(CAnimateTailAnimatorAmount(0, speedModifer));
    }
    //

    private Coroutine TailAmmountAnimatorCor;
    private IEnumerator CAnimateTailAnimatorAmount(float to, float mod = 1)
    {
        while (Mathf.Abs(to - _tailAnimator.TailAnimatorAmount) >= 0.02f)
        {
            _tailAnimator.TailAnimatorAmount = Mathf.Lerp(_tailAnimator.TailAnimatorAmount, to, 0.7f * Time.deltaTime * mod);
            yield return default;
        }

        _tailAnimator.TailAnimatorAmount = to;
        TailAmmountAnimatorCor = default;
    }
}