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
    private Transform[] _reset;
    private Rigidbody[] _rbs;

    // Start is called before the first frame update
    void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _rbs = GetComponentsInChildren<Rigidbody>();

        DisableRagdoll();

        _reset = new Transform[_charStart.childCount];

        for (int i = 0; i < _charStart.childCount; i++)
        {
            _reset[i] = _charStart.GetChild(i);
        }
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
        _anim.enabled = true;
        _anim.enabled = false;
    }

    public void DisableRagdoll()
    {
        _rb.isKinematic = true;

        for (int i = 0; i < _rbs.Length; i++)
        {
            _rbs[i].isKinematic = true;
        }

        _anim.enabled = true;
        _anim.enabled = false;
        _anim.enabled = true;
    }
}
