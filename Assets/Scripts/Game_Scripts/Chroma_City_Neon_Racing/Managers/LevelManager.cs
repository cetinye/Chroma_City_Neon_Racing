using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Player player;
    [SerializeField] private CameraFollow mainCamera;
    [SerializeField] private GameObject checkpointGenerator;

    [Header("Instantiate Prefabs")]
    [SerializeField] private TrafficLight trafficLightPref;
    [SerializeField] private FinishLine finishPref;

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Start()
    {
        GameStateManager.OnGameStateChanged += OnGameStateChanged;
        GameStateManager.SetGameState(GameState.Idle);

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
                player.SetTargetSpeed(1f);
                mainCamera.DetachFromPlayer();
                break;

            default:
                break;
        }

        uiManager.UpdateGameStateText(GameStateManager.GetGameState().ToString());
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

    public void SpawnFinish()
    {
        Transform[] childs = checkpointGenerator.transform.GetComponentsInChildren<Transform>(false);

        Transform lastActiveCheckpoint = null;

        foreach (Transform child in childs)
        {
            if (child.gameObject.activeSelf)
            {
                lastActiveCheckpoint = child;
            }
        }

        if (lastActiveCheckpoint != null)
        {
            FinishLine finish = Instantiate(finishPref);
            finish.transform.position = lastActiveCheckpoint.position;
            lastActiveCheckpoint.gameObject.SetActive(false);
            finish.StartLightChange();
        }
    }
}
