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
        Time.timeScale = 0.5f;
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void FixedUpdate()
    {
        if (!Physics.Raycast(_head.position, Vector3.down, 5))
        {
            _rb.MoveRotation(Quaternion.Euler(Vector3.up * _torque * Input.GetAxis("Horizontal") * Time.fixedDeltaTime) * _rb.rotation);
        }
        if (!Physics.Raycast(transform.position, Vector3.down, 3))
        {
            _rb.MovePosition(transform.position + (Vector3.up * Physics.gravity.y * 5f * Time.fixedDeltaTime));
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(_head.position, _head.position + Vector3.down * 5);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_head.position + Vector3.up * 10f, Vector3.down * 10f + _head.position);
        Gizmos.DrawSphere(_head.position + Vector3.forward * 5 + Vector3.up * 10f, 0.1f);
    }
}
