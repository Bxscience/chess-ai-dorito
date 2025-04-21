using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour {
    public GameObject panel;
    public TextMeshProUGUI gameOverText;
    
    public void DisplayGameOver(string message) {
        gameOverText.text = message;
        panel.SetActive(true);
    }

    public void OnPlayAgainButton() {
        GameSettings.playAgainstAI = 0;
        GameSettings.aiDifficulty = 0;
        GameSettings.playerColor = 0;
        SceneManager.LoadScene("Start");
    }
}
