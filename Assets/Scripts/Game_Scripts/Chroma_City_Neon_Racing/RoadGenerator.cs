using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;

    [Header("Components")]
    public SplineComputer splineComputerRoad;
    public SplineComputer splineComputerBuildings;
    public SplineComputer splineComputerCheckpoints;
    public SplineComputer splineComputerPowerUps;
    public SplineFollower splineFollower;

    [Header("Variables")]
    public int pointAmount;
    public int pointAmountToRandomize;
    public float minX;
    public float maxX;
    public float distBetweenRoads;

    [Header("Building Variables")]
    [SerializeField] private float buildingOffsetToRoad;

    void Awake()
    {
        SpawnPoints(pointAmount);
        RandomizePointsOnX(minX, maxX, pointAmountToRandomize);
        CreateBuildingsSpline();
        CreateCheckpointsSpline();
        CreatePowerUpsSpline();
        Invoke(nameof(RemoveExcessObjects), 1f);
    }

    void SpawnPoints(int pointAmount)
    {
        SplinePoint[] points = new SplinePoint[pointAmount];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new SplinePoint();
            points[i].position = Vector3.forward * (-i) * distBetweenRoads;
            points[i].normal = Vector3.up;
            points[i].size = 1f;
            points[i].color = Color.white;
        }

        splineComputerRoad.SetPoints(points);
        splineFollower.spline = splineComputerRoad;
    }

    void RandomizePointsOnX(float minX, float maxX, int pointAmountToRandomize)
    {
        SplinePoint[] points = new SplinePoint[splineComputerRoad.pointCount];
        points = splineComputerRoad.GetPoints();

        for (int i = 2; i < pointAmountToRandomize + 2; i++)
        {
            points[i].position = new Vector3(UnityEngine.Random.Range(minX, maxX), points[i].position.y, points[i].position.z);
        }

        splineComputerRoad.SetPoints(points);
    }

    void CreateBuildingsSpline()
    {
        SplinePoint[] points = new SplinePoint[splineComputerRoad.pointCount];
        points = splineComputerRoad.GetPoints();

        for (int i = 0; i < points.Length; i++)
        {
            points[i].position = new Vector3(points[i].position.x + buildingOffsetToRoad, -0.11f, points[i].position.z);
        }

        splineComputerBuildings.SetPoints(points);
    }

    void CreateCheckpointsSpline()
    {
        SplinePoint[] points = new SplinePoint[splineComputerRoad.pointCount];
        points = splineComputerRoad.GetPoints();

        for (int i = 0; i < points.Length; i++)
        {
            points[i].position = new Vector3(points[i].position.x, points[i].position.y, points[i].position.z);
        }

        splineComputerCheckpoints.SetPoints(points);
    }

    void CreatePowerUpsSpline()
    {
        SplinePoint[] points = new SplinePoint[splineComputerRoad.pointCount];
        points = splineComputerRoad.GetPoints();

        for (int i = 0; i < points.Length; i++)
        {
            points[i].position = new Vector3(points[i].position.x, points[i].position.y, points[i].position.z);
        }

        splineComputerPowerUps.SetPoints(points);
    }

    void RemoveExcessObjects()
    {
        RemoveExcessObjectsInSpline(splineComputerRoad);
        RemoveExcessObjectsInSpline(splineComputerBuildings);
        RemoveExcessObjectsInSpline(splineComputerPowerUps);
        RemoveExcessObjectsInSpline(splineComputerCheckpoints);

        levelManager.SpawnFinish();
    }

    private void RemoveExcessObjectsInSpline(SplineComputer splineComputer)
    {
        if (splineComputer == null || splineComputer.transform == null)
        {
            return;
        }

        for (int i = 0; i < splineComputer.transform.childCount - 1; i++)
        {
            var child = splineComputer.transform.GetChild(i);
            var nextChild = splineComputer.transform.GetChild(i + 1);

            if (nextChild == null)
            {
                break;
            }

            if (child.position == nextChild.position)
            {
                child.gameObject.SetActive(false);
                nextChild.gameObject.SetActive(false);
            }
        }
    }
}
