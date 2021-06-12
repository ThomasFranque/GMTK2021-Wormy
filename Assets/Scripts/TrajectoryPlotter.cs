using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryPlotter : MonoBehaviour
{
    [SerializeField] private int pointsCount;
    [SerializeField, Range(0f, 0.3f)] private float timeBetweenPoints = 0.1f;
    [SerializeField] private LayerMask ignoreLayers;
    [Header("Visuals")]
    [SerializeField] private bool meshBased;
    [SerializeField] private GameObject prefabMesh;
    [SerializeField] private LineRenderer prefabLine;


    private LineRenderer lineRenderer;
    private List<GameObject> trajectoryPoints;

    private void Awake()
    {
        if (!meshBased)
        {
            if (prefabLine)
            {
                lineRenderer = Instantiate(prefabLine, transform);
            }
            else
            {
                lineRenderer = new GameObject("Trajectory Line").AddComponent<LineRenderer>();
                lineRenderer.positionCount = pointsCount;
                lineRenderer.transform.parent = transform;
            }

            lineRenderer.useWorldSpace = true;
        }
        else
        {
            trajectoryPoints = new List<GameObject>();
            GameObject holder = new GameObject("Trajectory Line");
            holder.transform.parent = transform;
            
            for (int i = 0; i < pointsCount; i++)
            {
                GameObject p = Instantiate(prefabMesh, holder.transform);
                trajectoryPoints.Add(p);
            }
        }

        SetLine(false);
    }

    public void PlotTrajectory(Vector3 startPos, Vector3 startVelocity)
    {
        List<Vector3> points = new List<Vector3>();

        for (float t = 0; t < pointsCount; t += timeBetweenPoints)
        {
            Vector3 newPoint = startPos + t * startVelocity;
            newPoint.y = startPos.y + startVelocity.y * t + (Physics.gravity.y / 2f) * t * t;
            points.Add(newPoint);

            if (Physics.CheckSphere(newPoint, 0.5f, ~ignoreLayers))
            {
                if (!meshBased) lineRenderer.positionCount = points.Count;
                break;
            }
        }

        PlotPoints(points);
    }

    private void PlotPoints(List<Vector3> trajectory)
    {
        if (!meshBased)
        {
            lineRenderer.SetPositions(trajectory.ToArray());
        }
        else
        {
            for (int i = 0; i < pointsCount; i++)
            {
                if (i < trajectory.Count)
                {
                    Vector3 dir = trajectoryPoints[i].transform.position - trajectoryPoints[i - 1 < 0 ? 0 : i - 1].transform.position;

                    trajectoryPoints[i].transform.up = dir.normalized; 

                    trajectoryPoints[i].SetActive(i != 0);
                    trajectoryPoints[i].transform.position = trajectory[i];
                }
                else
                {
                    trajectoryPoints[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetLine(bool value)
    {
        if (!meshBased)
        {
            lineRenderer.enabled = value;
        }
        else
        {
            for (int i = 0; i < pointsCount; i++)
            {
                trajectoryPoints[i].gameObject.SetActive(value);
            }
        }
    }
}
