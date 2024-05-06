using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text gameStateText;

    public void UpdateGameStateText(string text)
    {
        gameStateText.text = "CurrentGameState: " + text;
    }
}
