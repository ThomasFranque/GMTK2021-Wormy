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

    private GarryController controller;
    private Rigidbody rb;
    private Transform holeTransform;
    private RaycastHit hit;
    private TrajectoryPlotter trajectory;
    private bool shootWorm;
    private bool held;
    private bool released;
    private GameObject worm;

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

    public bool Obstructed()
    {
        Physics.Raycast(holeTransform.position, holeTransform.forward, out hit, 5f, ~blockMask);
        return !(Vector3.Distance(holeTransform.position, hit.point) > 0.3f);
    }

    private void FixedUpdate()
    {
        if (held && released)
        {
            Physics.Raycast(holeTransform.position, holeTransform.forward, out hit, 5f, ~blockMask);
            Debug.DrawRay(holeTransform.position, holeTransform.forward * 5f, Color.red, 0.5f);

            shootWorm = Vector3.Distance(holeTransform.position, hit.point) > 0.3f;

            held = released = false;
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
            held = true;
            Physics.Raycast(holeTransform.position, holeTransform.forward, out hit, 5f, ~blockMask);
            Debug.DrawRay(holeTransform.position, holeTransform.forward * 5f, Color.red, 0.5f);

            shootWorm = Vector3.Distance(holeTransform.position, hit.point) > 0.3f;

            trajectory.SetLine(shootWorm);
        }

        if (!Obstructed() && held)
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
        Debug.Log("Drawing Trajectory");
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
        if (worm == null)
        {
            worm = Instantiate(wormPrefab, Vector3.zero, Quaternion.identity);
        }

        worm.transform.position = holeTransform.position;
        worm.transform.forward = holeTransform.forward;
        FollowTarget.Current.followTarget = worm.transform.Find("Pole");
        worm.GetComponent<Rigidbody>().AddForce(holeTransform.forward * HoleShootForce(wormForce));
    }

    public void PickWorm(GameObject pickedWorm)
    {
        if (worm == null)
        {
            worm = pickedWorm;
        }
        if (pickedWorm != worm) return;

        worm.gameObject.SetActive(false);
        // Change camera
        FollowTarget.Current.followTarget = transform.GetChild(0);
        GetComponentInChildren<GarryVisuals>().WormEntered(() => GarryController.Disabled = false);
    }

    public event System.Action<bool> onShoot;
}
