using System.Collections;
using System.Collections.Generic;
using Chroma_City_Neon_Racing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
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
    private int shieldPowerup;
    private int speedPowerup;
    private int timePowerup;
    private int durationOfPowerups;
    private int timeLimit;
    private int maxScore;
    private bool isLevelTimerOn = false;
    private float levelTimer;
    private List<Vector3> usedPositions = new List<Vector3>();
    private List<SpecialPowerUp> spawnedSpecialPowerUps = new List<SpecialPowerUp>();

    [Header("Components")]
    [SerializeField] private Player player;
    [SerializeField] private RoadGenerator roadGenerator;
    [SerializeField] private CameraFollow mainCamera;
    [SerializeField] private GameObject checkpointGenerator;
    private FinishLine finish;
    private TrafficLight trafficLight;

    [Header("Instantiate Prefabs")]
    [SerializeField] private TrafficLight trafficLightPref;
    [SerializeField] private FinishLine finishPref;
    [SerializeField] private SpecialPowerUp shieldPowerUpPref;
    [SerializeField] private SpecialPowerUp speedPowerUpPref;
    [SerializeField] private SpecialPowerUp timePowerUpPref;

    public void Restart()
    {
        levelId++;
        LoadLevel(false);
    }

    public void LoadLevel(bool isNextLevel)
    {
        //TODO: Oyunda her seviyeyi çarpmadan sorunsuz bir şekilde tamamlamak bir üst seviyeye geçmemizi sağlayacaktır.
        //TODO: Eğer bir bölümde çıkan enerj toplarından yanlış olanı alırsak, yani engele çarptığımız taktirde bölümü tekrar oynayacağız, 2 sefer daha aynı bölümü oynamamıza rağmen hala çarpma yaşıyorsak seviyemiz bir tane düşecektir.

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


        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        DeleteScene();
        StartGame();
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        LevelTimer();

        uiManager.UpdateDebugTexts(GameStateManager.GetGameState().ToString(), levelId, roadGenerator.pointAmount, player.GetFollowSpeed());
    }

    void StartGame()
    {
        AudioManager.instance.Play(SoundType.Background);

        GameStateManager.OnGameStateChanged += OnGameStateChanged;
        GameStateManager.SetGameState(GameState.Idle);

        AssignLevelVariables();

        roadGenerator.SpawnLevel();

        ColorCheckpoints();
        SpawnSpecialPowerUps();
        AudioManager.instance.PlayOneShot(SoundType.MotorStart);
        StartTrafficLight();
    }

    void DeleteScene()
    {
        CancelInvoke();
        isLevelTimerOn = false;
        player.Reset();
        mainCamera.Reset();
        roadGenerator.Reset();
        Destroy(finish.gameObject);
        Destroy(trafficLight.gameObject);
        usedPositions.Clear();

        foreach (SpecialPowerUp powerUp in spawnedSpecialPowerUps)
        {
            Destroy(powerUp.gameObject);
        }

        spawnedSpecialPowerUps.Clear();
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
                uiManager.SetButtonsState(false);
                break;

            case GameState.Racing:
                uiManager.SetButtonsState(true);
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
        //levelId = PlayerPrefs.GetInt("CCNR_levelId", 0);
        levelSO = levels[levelId];

        minSpeed = levelSO.minSpeedRange;
        maxSpeed = levelSO.maxSpeedRange;
        ballSpeedChangeAmount = levelSO.ballSpeedChangeAmount;
        speedPenatlyAmount = levelSO.speedPenatlyAmount;
        pathLength = levelSO.pathLength;

        shieldPowerup = levelSO.shieldPowerup;
        speedPowerup = levelSO.speedPowerup;
        timePowerup = levelSO.timePowerup;
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
        trafficLight = Instantiate(trafficLightPref);
        trafficLight.StartCountdown();
    }

    private void SpawnSpecialPowerUps()
    {
        Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);

        int shieldPowerupCount = Random.Range(1, 4);
        int speedPowerupCount = Random.Range(1, 4);
        int timePowerupCount = Random.Range(1, 4);

        if (shieldPowerup == 1)
        {
            for (int i = 0; i < shieldPowerupCount; i++)
            {
                SpecialPowerUp shield = Instantiate(shieldPowerUpPref, GetRandomPointPos(), rotation);
                shield.SetDuration(levelSO.durationOfPowerups);
                spawnedSpecialPowerUps.Add(shield);
            }
        }

        if (speedPowerup == 1)
        {
            for (int i = 0; i < speedPowerupCount; i++)
            {
                SpecialPowerUp speed = Instantiate(speedPowerUpPref, GetRandomPointPos(), rotation);
                speed.SetDuration(levelSO.durationOfPowerups);
                speed.SetAddSpeedAmount(1f);
                spawnedSpecialPowerUps.Add(speed);
            }
        }

        if (timePowerup == 1)
        {
            for (int i = 0; i < timePowerupCount; i++)
            {

                SpecialPowerUp time = Instantiate(timePowerUpPref, GetRandomPointPos(), rotation);
                time.SetDuration(levelSO.durationOfPowerups);
                time.SetTimeToAdd(5f);
                spawnedSpecialPowerUps.Add(time);
            }
        }

    }

    private Vector3 GetRandomPointPos()
    {
        Vector3 pos;
        do
        {
            pos = roadGenerator.GetRandomPointPos();

        } while (usedPositions.Contains(pos));

        usedPositions.Add(pos);

        pos = new Vector3(pos.x + 0.02f, 0.102f, pos.z);
        return pos;
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

    public void AddTime(float timeToAdd)
    {
        levelTimer += timeToAdd;
    }
}
