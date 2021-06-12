using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;

public class WormMovement : MonoBehaviour
{
    [SerializeField] private float _torque;
    [SerializeField] private Transform _head;

    private Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {            // Rotates the body to align to the floor normal
        if (Physics.Raycast(transform.position + (transform.forward * 0.2f), Vector3.down,
            out RaycastHit hit, 10f, LayerMask.GetMask("Default")))
        {
            Physics.Raycast(transform.position - (transform.forward * 0.2f), Vector3.down,
               out RaycastHit hit2, 10f, LayerMask.GetMask("Default"));

            Quaternion newRot = Quaternion.FromToRotation
                (transform.up, (hit.normal + hit2.normal).normalized) * transform.rotation;

            transform.rotation = Quaternion.Slerp(transform.rotation, newRot, 10f * Time.deltaTime);
        }
    }
    private void FixedUpdate()
    {
        if (!Physics.Raycast(_head.position, Vector3.down, 5))
        {
            _rb.MoveRotation(Quaternion.Euler(Vector3.up * _torque * Input.GetAxis("Horizontal") * Time.fixedDeltaTime) * _rb.rotation);
        }
        if (!Physics.Raycast(transform.position, Vector3.down, 1))
        {
            _rb.MovePosition(transform.position + (Vector3.up * Physics.gravity.y * 5f * Time.fixedDeltaTime));
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(_head.position, _head.position + Vector3.down * 5);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + (transform.forward * 0.2f), 0.1f);
        Gizmos.DrawSphere(transform.position - (transform.forward * 0.2f), 0.1f);
    }
}
