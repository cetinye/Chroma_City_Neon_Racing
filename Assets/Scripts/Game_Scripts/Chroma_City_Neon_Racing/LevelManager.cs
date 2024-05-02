using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject checkpointGenerator;

    void Start()
    {
        ColorCheckpoints();
    }

    public void RightPressed()
    {
        Debug.LogWarning("Right");
        player.SwitchLane(true);
    }

    public void LeftPressed()
    {
        Debug.LogWarning("Left");
        player.SwitchLane(false);
    }

    private void ColorCheckpoints()
    {
        List<Checkpoint> checkpoints = new List<Checkpoint>();

        for (int i = 0; i < checkpointGenerator.transform.childCount; i++)
        {
            checkpoints.Add(checkpointGenerator.transform.GetChild(i).GetComponent<Checkpoint>());
        }

        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.SetRandomColor();
        }
    }
}
