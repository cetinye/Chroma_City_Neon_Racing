using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text levelTimerText;
    [SerializeField] private TMP_Text gameStateText;

    public void UpdateTimer(float timer)
    {
        levelTimerText.text = timer.ToString("F0");
    }

    public void UpdateGameStateText(string text)
    {
        gameStateText.text = "CurrentGameState: " + text;
    }
}
