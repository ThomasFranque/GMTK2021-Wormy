using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormRagdoll : MonoBehaviour
{
    private Rigidbody _rb;
    private Animator _anim;
    public bool toggleRagdoll;
    public bool untoggleRagdoll;

    [SerializeField] private Transform _charStart;
   
    private List<Transform> _reset;
    private List<(Vector3, Quaternion)> _resetValues;
    private Rigidbody[] _rbs;
    private WormMovement _movement;

    // Start is called before the first frame update
    void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _rbs = GetComponentsInChildren<Rigidbody>();

        _reset = new List<Transform>();
        _resetValues = new List<(Vector3, Quaternion)>();

        Transform current = _charStart;

        while (current != null) 
        {
            _reset.Add(current);
            _resetValues.Add((current.localPosition, current.localRotation));
            if (current.childCount > 0)
            {
                current = current.GetChild(0);
            }
            else
            {
                current = null;
            }
        }

        _movement = GetComponent<WormMovement>();
        DisableRagdoll();
    }

    private void Update()
    {
        if (toggleRagdoll)
        {
            EnableRagdoll();
            toggleRagdoll = false;
        }
        if (untoggleRagdoll)
        {
            DisableRagdoll();
            untoggleRagdoll = false;
        }
    }
    public void EnableRagdoll()
    {
        _rb.isKinematic = false;

        for (int i = 0; i < _rbs.Length; i++)
        {
            _rbs[i].isKinematic = false;
        }

        _anim.enabled = false;
    }

    private IEnumerator LerpTransforms()
    {
        _movement.enabled = false;
        _rb.useGravity = false;
        _rb.isKinematic = true;

        for (int i = 0; i < _rbs.Length; i++)
        {
            _rbs[i].useGravity = false;
            _rbs[i].isKinematic = true;
        }

        float time = 0;
        Vector3 target = transform.position + Vector3.up;

        while(time < 2)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, target, 2f * Time.deltaTime);

            for (int i = 0; i < _reset.Count; i++)
            {
                _reset[i].localPosition = Vector3.Lerp(_reset[i].localPosition, _resetValues[i].Item1, 2f * Time.deltaTime);
                _reset[i].localRotation = Quaternion.Lerp(_reset[i].localRotation, _resetValues[i].Item2, 2f * Time.deltaTime);
            }
            yield return new WaitForEndOfFrame();
        }

        _anim.enabled = true;
        _rb.useGravity = true;
        
        for (int i = 0; i < _rbs.Length; i++)
        {
            _rbs[i].useGravity = true;
            _rbs[i].isKinematic = true;
        }
        _movement.enabled = true;
    }
    public void DisableRagdoll()
    {
        StartCoroutine(LerpTransforms());
    }
}
