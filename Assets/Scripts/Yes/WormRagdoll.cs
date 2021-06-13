using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormRagdoll : MonoBehaviour
{
    [SerializeField] private Transform _charStart;
    private Rigidbody _rb;
    private List<Transform> _reset;
    private List<(Vector3, Quaternion)> _resetValues;
    private Rigidbody[] _rbs;
    private NewWormMovement _movement;

    // Start is called before the first frame update
    void Awake()
    {
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

        _movement = GetComponent<NewWormMovement>();
        DisableRagdoll();
    }
    public void EnableRagdoll()
    {
        SwitchRigidBodyStates(false, true);
    }

    private IEnumerator LerpTransforms()
    {
        _movement.enabled = false;

        SwitchRigidBodyStates(true, false);

        float time = 0;
        Vector3 target = transform.position + Vector3.up * 0.01f;

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

        SwitchRigidBodyStates(true, true);

        _movement.enabled = true;
        onRagDollEnd?.Invoke();
    }

    private void SwitchRigidBodyStates(bool state, bool gravityState)
    {
        _rb.useGravity = gravityState;
        _rb.isKinematic = state;

        for (int i = 0; i < _rbs.Length; i++)
        {
            _rbs[i].useGravity = gravityState;
            _rbs[i].isKinematic = state;
        }
    }
    public void DisableRagdoll()
    {
        StartCoroutine(LerpTransforms());
    }

    public static Action onRagDollEnd;
    public static Action onRagDollStart;
}
