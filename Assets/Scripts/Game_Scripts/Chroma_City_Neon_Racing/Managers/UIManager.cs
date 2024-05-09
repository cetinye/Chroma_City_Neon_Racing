using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text levelTimerText;
    [SerializeField] private TMP_Text gameStateText;
    [SerializeField] private TMP_Text levelIdText;
    [SerializeField] private TMP_Text pointAmountText;
    [SerializeField] private TMP_Text playerTargetSpeed;

    public void UpdateTimer(float timer)
    {
        levelTimerText.text = timer.ToString("F0");
    }

    public void UpdateDebugTexts(string text, int levelId, int pointAmount, float playerSpeed)
    {
        gameStateText.text = "CurrentGameState: " + text;
        levelIdText.text = "LevelID: " + levelId;
        pointAmountText.text = "PointAmount: " + pointAmount;
        playerTargetSpeed.text = "PlayerSpeed: " + playerSpeed.ToString("F2");
    }
}
