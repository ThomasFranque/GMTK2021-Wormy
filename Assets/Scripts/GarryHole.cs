using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarryHole : MonoBehaviour, IWormGrabber
{
    [SerializeField, MinMaxSlider(1f, 20f)] Vector2 garryForce;
    [SerializeField, MinMaxSlider(1f, 10f)] Vector2 wormForce;
    [SerializeField] private bool debug;
    [SerializeField] private LayerMask blockMask;
    [SerializeField] private GameObject wormPrefab;
    [SerializeField] private Collider trigger;

    private GarryController controller;
    private Rigidbody rb;
    private Transform holeTransform;
    private RaycastHit hit;
    private TrajectoryPlotter trajectory;
    private bool shootWorm;
    private bool released;
    private GameObject worm;

    public bool Held { get; private set; }

    public float HoleShootForce(Vector2 range) => Mathf.Lerp(range.x, range.y, UniversalGameData.TotalLeafs);

    private void Awake()
    {
        controller = GetComponent<GarryController>();
        rb = GetComponent<Rigidbody>();
        trajectory = GetComponent<TrajectoryPlotter>();
        holeTransform = transform.Find("GarryHole");

        if (debug)
        {
            UniversalGameData.Leafs = UniversalGameData.TOTAL_GAME_LEAFS;
        }

        Debug.Log("Gary Hole force: " + HoleShootForce(garryForce));
    }

    private void Start()
    {
        if (worm == null)
        {
            worm = Instantiate(wormPrefab, Vector3.zero, Quaternion.identity);
        }

        worm.layer = LayerMask.NameToLayer("Player");
        worm.SetActive(false);

        Pole.current.AddSource(worm.transform);
        Pole.current.constraint.constraintActive = true;
    }

    public bool Obstructed()
    {
        Physics.Raycast(holeTransform.position, holeTransform.forward, out hit, 5f, ~blockMask);
        return !(Vector3.Distance(holeTransform.position, hit.point) > 0.3f);
    }

    private void FixedUpdate()
    {
        if (Held && released)
        {
            Physics.Raycast(holeTransform.position, holeTransform.forward, out hit, 5f, ~blockMask);
            Debug.DrawRay(holeTransform.position, holeTransform.forward * 5f, Color.red, 0.5f);

            shootWorm = Vector3.Distance(holeTransform.position, hit.point) > 0.3f;

            Held = released = false;
            if (shootWorm)
            {
                ShootWorm();
            }
            else
            {
                ShootGary();
            }

            onShoot?.Invoke(shootWorm);

            shootWorm = false;
            trajectory.SetLine(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GarryController.Disabled) return;

        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Shoot controller"))
        {
            Held = true;
            Physics.Raycast(holeTransform.position, holeTransform.forward, out hit, 5f, ~blockMask);
            Debug.DrawRay(holeTransform.position, holeTransform.forward * 5f, Color.red, 0.5f);

            shootWorm = Vector3.Distance(holeTransform.position, hit.point) > 0.3f;

            trajectory.SetLine(shootWorm);
        }

        if (!Obstructed() && Held)
        {
            DrawTrajectory();
        }

        if (Input.GetMouseButtonUp(0) || Input.GetButtonUp("Shoot controller"))
        {
            released = true;
            trajectory.SetLine(false);
        }
    }

    private void DrawTrajectory()
    {
        trajectory?.PlotTrajectory(holeTransform.position, (holeTransform.forward * HoleShootForce(wormForce)));
    }

    private void ShootGary()
    {
        // rb.AddForce((-holeTransform.forward) * HoleShootForce(garryForce), ForceMode.Impulse);
        rb.velocity = (-holeTransform.forward).normalized * controller.JumpSpeed(HoleShootForce(garryForce));
    }

    private void ShootWorm()
    {
        Debug.Log("Shoot Worm");
        GarryController.Disabled = true;

        worm.SetActive(true);
        worm.transform.position = holeTransform.position;
        worm.transform.forward = holeTransform.forward;
        trigger.enabled = false;
        FollowTarget.Current.followTarget = Pole.current.transform;
        worm.GetComponent<NewWormMovement>().
        ThrowWorm(holeTransform.forward * HoleShootForce(wormForce * -Physics.gravity.y));

        LeanTween.delayedCall(1f, () =>
        {
            worm.layer = LayerMask.NameToLayer("Ignore");
            trigger.enabled = true;
        });
    }

    public void PickWorm(GameObject pickedWorm)
    {
        if (worm == null)
        {
            worm = pickedWorm;
        }
        if (pickedWorm != worm) return;

        worm.layer = LayerMask.NameToLayer("Player");
        worm.SetActive(false);
        // Change camera
        FollowTarget.Current.followTarget = transform.GetChild(0);
        GetComponentInChildren<GarryVisuals>().WormEntered(() => GarryController.Disabled = false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered");
        if (other.transform.root.gameObject == worm)
        {
            Debug.Log(other.name);
            PickWorm(other.transform.root.gameObject);
        }
    }

    public event System.Action<bool> onShoot;
}