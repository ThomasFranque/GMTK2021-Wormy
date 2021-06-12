using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarryHole : MonoBehaviour
{
    [SerializeField] private float minForce;
    [SerializeField] private float maxForce;
    [SerializeField] private bool debug;
    [SerializeField] private LayerMask blockMask;

    private GarryController controller;
    private Rigidbody rb;
    private Transform holeTransform;
    private RaycastHit hit;
    private TrajectoryPlotter trajectory;
    private bool shootWorm;

    public float HoleShootForce => Mathf.Lerp(minForce, maxForce, UniversalGameData.TotalLeafs);

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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Shoot controller"))
        {
            Physics.Raycast(holeTransform.position, holeTransform.forward, out hit, 5f, ~blockMask);
            Debug.DrawRay(holeTransform.position, holeTransform.forward * 5f, Color.red, 0.5f);

            shootWorm = Vector3.Distance(holeTransform.position, hit.point) > 0.3f;

            trajectory.SetLine(shootWorm);
        }

        if (shootWorm)
        {
            DrawTrajectory();
        }

        if (Input.GetMouseButtonUp(0) || Input.GetButtonUp("Shoot controller"))
        {
            if (shootWorm)
            {
                ShootWorm();
            }
            else
            {
                ShootGary();
            }

            shootWorm = false;
            trajectory.SetLine(false);
        }
    }

    private void DrawTrajectory()
    {
        Debug.Log("Drawing Trajectory");
        trajectory?.PlotTrajectory(holeTransform.position, (holeTransform.forward * HoleShootForce) / 6f);
    }

    private void ShootGary()
    {
        rb.AddForce((-holeTransform.forward) * HoleShootForce, ForceMode.Impulse);
    }

    private void ShootWorm()
    {
        Debug.Log("Shoot Worm");
        // controller.Disabled = true;
    }
}
