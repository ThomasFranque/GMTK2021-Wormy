using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarryController : MonoBehaviour, IWormGrabber
{
    [SerializeField] private float accelleration;
    [SerializeField] private float jumpHeight;
    [Header("Some Visual things")]
    [SerializeField] private StaticParticles highFallParticles;
    [SerializeField] private float fallForParticles = 3f;

    private static float gravity = Physics.gravity.y;
    private Vector3 velocity;

    public Vector3 Velocity { get; set; }
    public static bool Disabled { get; set; }
    public bool Grounded {get; set;}
    public Vector3 InputDirection { get; private set; }

    private Rigidbody rb;
    private Transform cameraTransform;
    private bool jump;
    public bool Jumping {get; private set;}
    private float endHeight;
    private bool lastFrameGrounded;
    private RaycastHit hit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
        Grounded = true;
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
        
        if (!Grounded)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 20f, ~LayerMask.GetMask("Player")))
            {
                float dist = Vector3.Distance(hit.point, transform.position);
                if (dist > endHeight)
                {
                    endHeight = dist;
                }
            }
        }

        else if (Grounded && !lastFrameGrounded)
        {
            Debug.Log("Reached Ground");
            if (endHeight >= fallForParticles)
            {
                Physics.Raycast(transform.position, Vector3.down,out RaycastHit hit, 1f, ~LayerMask.GetMask("Player"));
                Vector3 point = hit.point;
                point.y += 0.2f;
                highFallParticles.PlayPS(point, hit.normal);
            }

            endHeight = 0;
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

    private void OnCollisionExit(Collision other) {
        Grounded = false;
    }

    public void PickWorm()
    {
        Disabled = false;
    }
}
