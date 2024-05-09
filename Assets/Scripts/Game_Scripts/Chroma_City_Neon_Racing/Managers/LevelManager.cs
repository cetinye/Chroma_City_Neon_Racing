using System.Collections;
using System.Collections.Generic;
using Chroma_City_Neon_Racing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;

    [Header("Level Variables")]
    [SerializeField] private int levelId;
    [SerializeField] private LevelSO levelSO;
    [SerializeField] private List<LevelSO> levels = new List<LevelSO>();

    private float minSpeed;
    private float maxSpeed;
    private float ballSpeedChangeAmount;
    private float speedPenatlyAmount;
    private int pathLength;
    private int shieldPowerupCount;
    private int speedPowerupCount;
    private int timePowerupCount;
    private int durationOfPowerups;
    private int timeLimit;
    private int maxScore;
    private bool isLevelTimerOn = false;
    private float levelTimer;

    [Header("Components")]
    [SerializeField] private Player player;
    [SerializeField] private RoadGenerator roadGenerator;
    [SerializeField] private CameraFollow mainCamera;
    [SerializeField] private GameObject checkpointGenerator;
    private FinishLine finish;

    [Header("Instantiate Prefabs")]
    [SerializeField] private TrafficLight trafficLightPref;
    [SerializeField] private FinishLine finishPref;

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadLevel(bool isNextLevel)
    {
        if (isNextLevel)
        {
            levelId++;
        }
        else
        {
            levelId--;
        }

        levelId = Mathf.Clamp(levelId, 0, levels.Count - 1);
        PlayerPrefs.SetInt("CCNR_levelId", levelId);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Start()
    {
        AudioManager.instance.Play(SoundType.Background);

        GameStateManager.OnGameStateChanged += OnGameStateChanged;
        GameStateManager.SetGameState(GameState.Idle);

        AssignLevelVariables();

        roadGenerator.SpawnLevel();

        ColorCheckpoints();
        AudioManager.instance.PlayOneShot(SoundType.MotorStart);
        StartTrafficLight();
    }

    void Update()
    {
        LevelTimer();

        uiManager.UpdateDebugTexts(GameStateManager.GetGameState().ToString(), levelId, roadGenerator.pointAmount, player.GetFollowSpeed());
    }

    void LevelTimer()
    {
        if (!isLevelTimerOn) return;

        levelTimer -= Time.deltaTime;
        uiManager.UpdateTimer(levelTimer);

        if (levelTimer <= 0)
        {
            isLevelTimerOn = false;
            levelTimer = 0;
            uiManager.UpdateTimer(levelTimer);
            GameStateManager.SetGameState(GameState.Failed);
        }
    }

    private void OnGameStateChanged()
    {
        switch (GameStateManager.GetGameState())
        {
            case GameState.Idle:
                break;

            case GameState.Racing:
                isLevelTimerOn = true;
                player.SetTargetSpeed(minSpeed);
                break;

            case GameState.Failed:
                isLevelTimerOn = false;
                break;

            case GameState.Success:
                isLevelTimerOn = false;
                player.SetTargetSpeed(minSpeed);
                mainCamera.DetachFromPlayer();
                break;

            default:
                break;
        }
    }

    private void AssignLevelVariables()
    {
        levelId = PlayerPrefs.GetInt("CCNR_levelId", 0);
        levelSO = levels[levelId];

        minSpeed = levelSO.minSpeedRange;
        maxSpeed = levelSO.maxSpeedRange;
        ballSpeedChangeAmount = levelSO.ballSpeedChangeAmount;
        speedPenatlyAmount = levelSO.speedPenatlyAmount;
        pathLength = levelSO.pathLength;

        shieldPowerupCount = levelSO.shieldPowerupCount;
        speedPowerupCount = levelSO.speedPowerupCount;
        timePowerupCount = levelSO.timePowerupCount;
        durationOfPowerups = levelSO.durationOfPowerups;

        timeLimit = levelSO.timeLimit;
        maxScore = levelSO.maxScore;

        player.SetSpeedChangeAmount(ballSpeedChangeAmount);
        player.SetSpeedPenaltyAmount(speedPenatlyAmount);
        player.SetMinMaxSpeed(minSpeed, maxSpeed);
        roadGenerator.SetPathLength(pathLength);
        levelTimer = timeLimit;
    }

    #region Input Controls

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

    #endregion

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
            finish = Instantiate(finishPref);
            finish.transform.position = lastActiveCheckpoint.position;
            lastActiveCheckpoint.gameObject.SetActive(false);
            finish.StartLightChange();
        }
    }

    public Vector3 GetFinishPosition()
    {
        return finish.transform.position;
    }
}
