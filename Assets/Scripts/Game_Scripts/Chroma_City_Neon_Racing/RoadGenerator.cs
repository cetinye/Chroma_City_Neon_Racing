using Dreamteck.Splines;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public SplineComputer splineComputer;
    public Spline spline;
    public SplineFollower splineFollower;
    public SplineMesh splineMesh;
    public int pointAmount;
    public float distBetweenRoads;

    // Start is called before the first frame update
    void Awake()
    {
        SpawnPoints(pointAmount);
        RandomizePointsOnX(-1f, 1f, 20);
    }

    void SpawnPoints(int pointAmount)
    {
        //Add a Spline Computer component to this object
        splineComputer = gameObject.AddComponent<SplineComputer>();
        //Create a new array of spline points
        SplinePoint[] points = new SplinePoint[pointAmount];
        //Set each point's properties
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new SplinePoint();
            points[i].position = Vector3.forward * (-i) * distBetweenRoads;
            points[i].normal = Vector3.up;
            points[i].size = 1f;
            points[i].color = Color.white;
        }
        //Write the points to the spline
        splineComputer.SetPoints(points);
        splineMesh.spline = splineComputer;
        splineFollower.spline = splineComputer;
    }

    void RandomizePointsOnX(float minX, float maxX, int pointAmountToRandomize)
    {
        SplinePoint[] points = new SplinePoint[splineComputer.pointCount];
        points = splineComputer.GetPoints();

        for (int i = 0; i < pointAmountToRandomize; i++)
        {
            points[i].position = new Vector3(UnityEngine.Random.Range(minX, maxX), points[i].position.y, points[i].position.z);
        }

        splineComputer.SetPoints(points);
    }
}
