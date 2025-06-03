using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildSystem : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile placeTile;
    [SerializeField] private Tile removeTile;

    // Build state
    public BuildState BuildState = BuildState.Place;

    private GameBoard gameBoard;
    private Vector2Int currentCell;

    // Singleton
    public static BuildSystem Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        } else {
            DontDestroyOnLoad(this);
            Instance = this;
        }
    }

    void Start() {
        gameBoard = GameBoard.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // Clear tilemap if game is running
        if (!gameBoard.paused) {
            tilemap.ClearAllTiles();
            return;
        };
        tilemap.enabled = true;
        
        Tile tile = BuildState == BuildState.Place ? placeTile : removeTile; // Select tile
        currentCell = GetMouseCellPosition();
        tilemap.ClearAllTiles(); // Clear previous tile
        tilemap.SetTile(new Vector3Int(currentCell.x, currentCell.y, 0), tile); // Set new tile at current cell

        // Place cell
        if (Input.GetMouseButton(0)) {
            gameBoard.EditCell(currentCell, BuildState == BuildState.Place ? CellState.Alive : CellState.Dead);
        }
    }

    private Vector2Int GetMouseCellPosition() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z); // Camera distance
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos); // Get worldpos from mousepos
        Vector3Int cellPos = tilemap.WorldToCell(worldPos); // Get cellpos to tilepos
        return new Vector2Int(cellPos.x, cellPos.y);
    }

    public void ToggleState() {
        BuildState = BuildState == BuildState.Place ? BuildState.Remove : BuildState.Place;
    }
}
