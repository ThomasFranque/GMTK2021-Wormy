using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarryHole : MonoBehaviour
{
    [SerializeField, MinMaxSlider(1f, 20f)] Vector2 garryForce;
    [SerializeField, MinMaxSlider(1f, 10f)] Vector2 wormForce;
    [SerializeField] private bool debug;
    [SerializeField] private LayerMask blockMask;

    private GarryController controller;
    private Rigidbody rb;
    private Transform holeTransform;
    private RaycastHit hit;
    private TrajectoryPlotter trajectory;
    private bool shootWorm;
    private bool held;
    private bool released;

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
        return Vector3.Distance(holeTransform.position, hit.point) > 0.3f;
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
        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Shoot controller"))
        {
            held = true;
            Physics.Raycast(holeTransform.position, holeTransform.forward, out hit, 5f, ~blockMask);
            Debug.DrawRay(holeTransform.position, holeTransform.forward * 5f, Color.red, 0.5f);

            shootWorm = Vector3.Distance(holeTransform.position, hit.point) > 0.3f;

            trajectory.SetLine(shootWorm);
        }

        if (!Obstructed())
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
        // controller.Disabled = true;
    }

    public event System.Action<bool> onShoot;
}
