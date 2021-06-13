using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWormMovement : MonoBehaviour
{
    [SerializeField] private float _playerSpeed = 2.0f;
    [SerializeField] private float _jumpHeight = 1.0f;
    [SerializeField] private float _gravityValue = -9.81f;
    [SerializeField] private float _stayRagDollTime = 3;

    private CharacterController _controller;
    private Vector3 _playerVelocity;
    private Rigidbody _rb;
    private Transform _cam;
    private WormRagdoll _ragdoll;

    public bool isRagDolling { get; private set; }
    private Vector3 Forward => new Vector3(_cam.forward.x, 0, _cam.forward.z).normalized;
    private Vector3 Right => Vector3.Cross(Vector3.up, Forward).normalized;

    private bool _jump;
    Vector3 move;
    bool isOnGround;

    // Start is called before the first frame update
    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _rb = GetComponent<Rigidbody>();
        _ragdoll = GetComponent<WormRagdoll>();
        _cam = Camera.main.transform;
    }

    private void Update()
    {
        if (!_controller.enabled) return;
        bool isOnGround = _controller.isGrounded;

        move = (Right * Input.GetAxis("Horizontal") + Forward * Input.GetAxis("Vertical"));

        if (Input.GetButtonDown("Jump") && isOnGround)
        {
            _jump = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_rb.velocity.magnitude <= 2 && !_controller.enabled && isRagDolling)
        {
            isRagDolling = false;
            StartCoroutine(Switch());
        }
        if (_controller.enabled)
        {
            Move();
            Align();
        }
    }

    private void Align()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 0.5f))
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation * transform.rotation, 10 * Time.deltaTime);
        }
    }
    private void Move()
    {
        if (isOnGround && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }
        _controller.Move(move * Time.deltaTime * _playerSpeed);

        if (move != Vector3.zero)
        {
            transform.forward = Vector3.Lerp(transform.forward, move, 10f * Time.deltaTime);
        }

        // Changes the height position of the player..
        if (_jump)
        {
            ThrowWorm();
        }
        _playerVelocity.y += _gravityValue * Time.deltaTime;
        _controller.Move(_playerVelocity * Time.deltaTime);
    }

    public void ThrowWorm()
    {
        _controller.enabled = false;
        _ragdoll.EnableRagdoll();


        Vector3 currentForce = _rb.velocity;
        currentForce.y = 0;

        _rb.velocity = currentForce + ((transform.forward + Vector3.up) * Mathf.Sqrt(2 * _jumpHeight * -Physics.gravity.y));

        _jump = false;

        isRagDolling = true;
    }

    public void ThrowWorm(Vector3 velocity, bool force = false)
    {
        _controller.enabled = false;
        _ragdoll.EnableRagdoll();


        Vector3 currentForce = _rb.velocity;
        currentForce.y = 0;

        if (force)
        {
            _rb.AddForce(velocity, ForceMode.Force);
        }
        else
        {
            _rb.velocity = velocity;
        }    

        _jump = false;
        isRagDolling = true;
    }

    private IEnumerator Switch()
    {
        yield return new WaitForSeconds(_stayRagDollTime);
        _ragdoll.DisableRagdoll();
        _controller.enabled = true;
    }
}