using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWormMovement : MonoBehaviour
{
    private WormRagdoll _ragdoll;
    public bool isRagDolling { get; private set; }

    // Update is called once per frame
    //void Update()
    //{
    //    if (_rb.velocity.magnitude <= 2 && !_controller.enabled && isRagDolling)
    //    {
    //        isRagDolling = false;
    //        StartCoroutine(Switch());
    //    }
    //}


    [SerializeField] private float accelleration;
    [SerializeField] private float jumpHeight;
    [Header("Some Visual things")]
    //[SerializeField] private ParticleSystem highFallParticles;
    [SerializeField] private float fallForParticles = 3f;

    private static float gravity = Physics.gravity.y;
    private Vector3 velocity;

    public static bool Disabled { get; set; }
    public bool Grounded { get; set; }
    public Vector3 Velocity { get; set; }
    public Vector3 InputDirection { get; private set; }

    private Rigidbody rb;
    private Transform cameraTransform;
    private bool jump;
    public bool Jumping { get; private set; }
    private float endHeight;
    private float initialHeight;
    private bool lastFrameGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
        _ragdoll = GetComponent<WormRagdoll>();
        Grounded = true;
    }

    private void FixedUpdate()
    {
        if (Disabled) return;

        if (Grounded)
        {
            UpdateMovement();

            if (jump)
            {
                jump = false;
                ThrowWorm();
            }
        }

        Align();
        if (!Grounded && lastFrameGrounded)
        {
            // initial height
            initialHeight = transform.position.y;
        }
        else if (Grounded && !lastFrameGrounded)
        {
            endHeight = transform.position.y;
            if (isRagDolling)
            {
                isRagDolling = false;
                StartCoroutine(Switch());
            }

            Debug.Log("Reached Ground");
            if (Mathf.Abs(endHeight - initialHeight) >= fallForParticles)
            {
                //Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, ~LayerMask.GetMask("Player"));
                //highFallParticles.transform.forward = hit.normal;
                //highFallParticles.Play();
            }
        }


        if (rb.velocity.magnitude < 0.01f)
            Grounded = true;

        lastFrameGrounded = Grounded;
    }

    // Update is called once per frame
    void Update()
    {
        if (Disabled) return;

        // Input
        InputDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        InputDirection.Normalize();
        InputDirection = RelativeVectorTo(InputDirection, cameraTransform);

        if (!jump)
            jump = Input.GetButtonDown("Jump");

        if (Jumping && Grounded)
        {
            Jumping = false;
        }
    }

    void UpdateMovement()
    {
        rb.AddForce(InputDirection * accelleration, ForceMode.Acceleration);
        // rb.velocity = Vector3.ClampMagnitude(rb.velocity, topSpeed);
    }
    private Vector3 RelativeVectorTo(Vector3 original, Transform relative)
    {
        Vector3 right = relative.right;
        right.y = 0f;
        right.Normalize();

        Vector3 forward = relative.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 finalDir = (forward * original.z) + (right * original.x);
        finalDir.Normalize();
        return finalDir;
    }


    private void Align()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 0.5f))
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation * transform.rotation, 20 * Time.deltaTime);
        }
    }

    public void ThrowWorm()
    {
        _ragdoll.EnableRagdoll();
        rb.AddForce((transform.forward + Vector3.up) * jumpHeight, ForceMode.VelocityChange);
        isRagDolling = true;
    }

    private IEnumerator Switch()
    {
        yield return new WaitForSeconds(3);
        _ragdoll.DisableRagdoll();
    }
    private void OnCollisionEnter(Collision other)
    {
        Grounded = true;
    }

    private void OnCollisionStay(Collision other)
    {
        Grounded = true;
    }

    private void OnCollisionExit(Collision other)
    {
        Grounded = false;
    }

}
