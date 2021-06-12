using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarryController : MonoBehaviour, IWormGrabber
{
    [SerializeField] private float accelleration;
    [SerializeField] private float jumpHeight;

    private static float gravity = Physics.gravity.y;
    private Vector3 velocity;

    public Vector3 Velocity { get; set; }
    public bool Disabled { get; set; }
    public bool Grounded {get; set;}
    public Vector3 InputDirection { get; private set; }

    private Rigidbody rb;
    private Transform cameraTransform;
    private bool jump;
    public bool Jumping {get; private set;}

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
    }

    // Start is called before the first frame update
    void Start()
    {

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
                JumpGarryJump();
            }
        }

        Grounded = false;
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

        Debug.Log($"{name}: Jump --- {jump}, Grounded: {Grounded}, Jumping: {Jumping}");
    }

    void UpdateMovement()
    {
        rb.AddForce(InputDirection * accelleration, ForceMode.Acceleration);
        // rb.velocity = Vector3.ClampMagnitude(rb.velocity, topSpeed);
    }

    void JumpGarryJump()
    {
        Jumping = true;
        rb.velocity = new Vector3(rb.velocity.x, JumpSpeed(jumpHeight), rb.velocity.z);
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

    public float JumpSpeed(float jumpHeight) => Mathf.Sqrt(2 * jumpHeight * -gravity);

    private void OnCollisionEnter(Collision other) 
    {
        Grounded = true;
    }

    private void OnCollisionStay(Collision other) 
    {
        Grounded = true;
    }

    public void PickWorm()
    {
        Disabled = false;
    }
}
