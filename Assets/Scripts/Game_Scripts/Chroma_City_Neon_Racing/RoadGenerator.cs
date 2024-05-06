using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
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
}
