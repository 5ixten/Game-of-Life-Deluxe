using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseButton : MonoBehaviour {
    private GameBoard gameBoard;
    [SerializeField] TextMeshProUGUI buttonText;

    private void Start() {
        gameBoard = GameBoard.Instance;
    }
    void Update()
    {
        buttonText.text = gameBoard.paused ? "Resume" : "Pause";
    }
}
