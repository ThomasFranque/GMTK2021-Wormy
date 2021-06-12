using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormVisuals : MonoBehaviour
{
    [SerializeField] private Texture2D[] _faces;
    [SerializeField] private Renderer _targetRenderer;
    [SerializeField] private FIMSpace.FTail.TailAnimator2 _tailAnimator;
    [Space]
    [SerializeField] private bool _lockFace;
    [SerializeField] private bool _lockHue;

    private void Awake()
    {

    }

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
    }

    private void RandomizeHue()
    {
        _targetRenderer.materials[0].SetFloat("_Hue", Random.Range(-0.8f, 0.1f));

    }

    private void RandomizeFace()
    {
        _targetRenderer.materials[1].SetTexture("_MainTex", _faces[Random.Range(0, _faces.Length)]);
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
    //

    private Coroutine TailAmmountAnimatorCor;
    private IEnumerator CAnimateTailAnimatorAmount(float to, float mod = 1)
    {
        while(Mathf.Abs(to - _tailAnimator.TailAnimatorAmount) >= 0.02f)
        {
            _tailAnimator.TailAnimatorAmount = Mathf.Lerp(_tailAnimator.TailAnimatorAmount, to, 0.7f * Time.deltaTime * mod);
            yield return default;
        }

        _tailAnimator.TailAnimatorAmount = to;
        TailAmmountAnimatorCor = default;
    }
}