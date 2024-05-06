using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject checkpointGenerator;

    [Header("Traffic Light Variables")]
    [SerializeField] private TrafficLight trafficLightPref;

    void Start()
    {
        GameStateManager.OnGameStateChanged += OnGameStateChanged;

        ColorCheckpoints();
        StartTrafficLight();
    }

    private void OnGameStateChanged()
    {
        switch (GameStateManager.GetGameState())
        {
            case GameState.Idle:
                break;

            case GameState.Racing:
                player.SetTargetSpeed(1f);
                break;

            case GameState.Failed:
                break;

            case GameState.Success:
                break;

            default:
                break;
        }
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

    private void StartTrafficLight()
    {
        TrafficLight trafficLight = Instantiate(trafficLightPref);
        trafficLight.StartCountdown();
    }
}
