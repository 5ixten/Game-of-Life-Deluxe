using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedText : MonoBehaviour
{
    private GameBoard gameBoard;
    private TextMeshProUGUI textGui;

    private void Start() {
        gameBoard = GameBoard.Instance;
        textGui = GetComponent<TextMeshProUGUI>();
    }
    void Update() {
        textGui.text = "Speed (" + gameBoard.updateInterval + "x):";
    }
}
