using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildButton : MonoBehaviour
{
    private BuildSystem buildSystem;
    private TextMeshProUGUI textGui;

    private void Start() {
        buildSystem = BuildSystem.Instance;
        textGui = GetComponent<TextMeshProUGUI>();
    }
    void Update() {
        textGui.text = buildSystem.BuildState == BuildState.Place ? "Remove Block" : "Place Block";
    }
}
