using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartOptions : MonoBehaviour
{
    public TMP_Dropdown playAgainst;
    public TMP_Dropdown difficulty;
    public TMP_Dropdown playAs;
    public TextMeshProUGUI difficultyText;


    void Start() {
        playAgainst.onValueChanged.AddListener(OnPlayAgainstChanged);
        difficulty.onValueChanged.AddListener(OnDifficultyChanged);
        playAs.onValueChanged.AddListener(OnPlayAsChanged);
    }

    void OnPlayAgainstChanged(int value) {
        GameSettings.playAgainstAI = value;
        difficultyText.gameObject.SetActive(value == 0);
        difficulty.gameObject.SetActive(value == 0);
    }

    void OnDifficultyChanged(int value) {
        GameSettings.aiDifficulty = value;
    }

    void OnPlayAsChanged(int value) {
        GameSettings.playerColor = value;
    }

    public void OnStartButtonPress() {
        SceneManager.LoadScene("Main");
    }
}
