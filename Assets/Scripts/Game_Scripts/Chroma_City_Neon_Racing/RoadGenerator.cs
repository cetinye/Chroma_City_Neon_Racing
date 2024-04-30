using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    [Header("Components")]
    public SplineComputer splineComputerRoad;
    public SplineComputer splineComputerBuildings;
    public SplineFollower splineFollower;
    public SplineMesh splineMesh;

    [Header("Variables")]
    public int pointAmount;
    public float distBetweenRoads;

    [Header("Building Variables")]
    [SerializeField] private float buildingOffsetToRoad;
    [SerializeField] private Transform buildingsParent;
    [SerializeField] private List<GameObject> buildings = new List<GameObject>();

    void Awake()
    {
        SpawnPoints(pointAmount);
        RandomizePointsOnX(-1f, 1f, 20);
        CreateBuildingsSpline();
    }

    void SpawnPoints(int pointAmount)
    {
        splineComputerRoad = gameObject.AddComponent<SplineComputer>();

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
        splineMesh.spline = splineComputerRoad;
        splineFollower.spline = splineComputerRoad;
    }

    void RandomizePointsOnX(float minX, float maxX, int pointAmountToRandomize)
    {
        SplinePoint[] points = new SplinePoint[splineComputerRoad.pointCount];
        points = splineComputerRoad.GetPoints();

        for (int i = 0; i < pointAmountToRandomize; i++)
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

    // void PlaceBuildings()
    // {
    //     SplinePoint[] points = new SplinePoint[splineComputerBuildings.pointCount];
    //     points = splineComputerBuildings.GetPoints();

    //     foreach (SplinePoint point in points)
    //     {
    //         Instantiate(GetRandomBuilding(), point.position, Quaternion.identity, buildingsParent);
    //     }
    // }

    // GameObject GetRandomBuilding()
    // {
    //     return buildings[UnityEngine.Random.Range(0, buildings.Count)];
    // }
}
