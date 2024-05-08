using UnityEngine;

public class LevelSO : ScriptableObject
{
    public int levelId;
    public float minSpeedRange;
    public float maxSpeedRange;
    public float ballSpeedChangeAmount;
    public float speedPenatlyAmount;
    public int pathLength;
    public int shieldPowerupCount;
    public int speedPowerupCount;
    public int timePowerupCount;
    public int durationOfPowerups;
    public int timeLimit;
    public int maxScore;
}