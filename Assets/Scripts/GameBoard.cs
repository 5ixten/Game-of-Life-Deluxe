using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class GameBoard : MonoBehaviour {
    private Dictionary<Vector2Int, Cell> currentState = new();
    private Dictionary<Vector2Int, Cell> nextState = new();

    [SerializeField] private Cell[] startState;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile aliveTile;
    
    // Properties
    public bool paused = true;
    public float updateInterval = 0.15f;

    private const float SPEEDCHANGE = 0.05f;
    private const float MINSPEED = 0.05f;
    private const float MAXSPEED = 1f;

    // Singleton
    public static GameBoard Instance { get; private set; }

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
        foreach (Cell cell in startState) {
            UpdateCell(currentState, cell.Pos, cell.State);
        }

        nextState = CloneState(currentState);
        StartCoroutine(UpdateBoard());
    }

    // Actions
    public void TogglePause() {
        paused = !paused;
    }

    public void SpeedUp() {
        updateInterval = Mathf.Max(updateInterval - SPEEDCHANGE, MINSPEED);
        updateInterval = (float)Math.Round(updateInterval, 2);

    }

    public void SpeedDown() {
        updateInterval = Mathf.Min(updateInterval + SPEEDCHANGE, MAXSPEED);
        updateInterval = (float)Math.Round(updateInterval, 2);

    }

    private Dictionary<Vector2Int, Cell> CloneState(Dictionary<Vector2Int, Cell> source) {
        var clone = new Dictionary<Vector2Int, Cell>();
        foreach (var kvp in source) {
            clone[kvp.Key] = new Cell {
                Pos = kvp.Key,
                State = kvp.Value.State
            };
        }
        return clone;
    }

    private List<Cell> GetAliveNeighbours(Cell cell, Dictionary<Vector2Int, Cell> grid) {
        List<Cell> alive = new();

        foreach (Cell n in GetNeighbours(cell, grid)) {
            if (n.State == CellState.Alive) {
                alive.Add(n);
            }
        }

        return alive;
    }

    private List<Cell> GetNeighbours(Cell cell, Dictionary<Vector2Int, Cell> grid) {
        List<Cell> neighbours = new();

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) continue;

                Vector2Int pos = cell.Pos + new Vector2Int(x, y);
                if (grid.TryGetValue(pos, out var neighbour)) {
                    neighbours.Add(neighbour);
                }
            }
        }

        return neighbours;
    }

    private void UpdateNextState() {
        if (paused) return;

        nextState = CloneState(currentState);
        HashSet<Vector2Int> processed = new();

        foreach (var kvp in currentState) {
            Cell cell = kvp.Value;

            // Values
            int aliveNeighbours = GetAliveNeighbours(cell, currentState).Count;
            List<Cell> neighbours = GetNeighbours(cell, currentState);

            // Dead cells with 3 alive neighbours become alive
            foreach (Cell neighbour in neighbours) {
                // Skip if alive or already processed
                if (neighbour.State == CellState.Alive || processed.Contains(neighbour.Pos)) continue;

                int nCount = GetAliveNeighbours(neighbour, currentState).Count;
                if (nCount == 3) {
                    UpdateCell(nextState, neighbour.Pos, CellState.Alive);
                    processed.Add(neighbour.Pos);
                }
            }

            // Cells with 2 or 3 alive neighbours stay alive
            if (cell.State == CellState.Alive && (aliveNeighbours == 2 || aliveNeighbours == 3)) {
                continue;
            }

            // Cells with less than 2 or more than 3 alive neighbours die
            if (cell.State == CellState.Alive && (aliveNeighbours < 2 || aliveNeighbours > 3)) {
                UpdateCell(nextState, cell.Pos, CellState.Dead);
            }
        }
    }

    private void ClenseCells(Dictionary<Vector2Int, Cell> grid) {
        List<Vector2Int> toRemove = new();
        foreach (var kvp in grid) {
            Cell cell = kvp.Value;
            if (cell.State == CellState.Dead && GetAliveNeighbours(cell, grid).Count == 0) {
                toRemove.Add(kvp.Key);
            }
        }
        // Remove empty cells
        foreach (var pos in toRemove) {
            grid.Remove(pos);
        }
    }

    public void EditCell(Vector2Int pos, CellState state) {
        UpdateCell(nextState, pos, state);
    }

    private void UpdateCell(Dictionary<Vector2Int, Cell> grid, Vector2Int pos, CellState state) {
        // Update state if cell exists
        if (grid.TryGetValue(pos, out var cell)) {
            cell.State = state;
            grid[pos] = cell;
        } else {
            // Create new cell if it doesn't exist
            grid[pos] = new Cell { Pos = pos, State = state };
        }

        if (state == CellState.Dead) {
            return;
        }

        // Add neighbours around state
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                // Skip the cell itself
                if (x == 0 && y == 0) continue;

                Vector2Int nPos = pos + new Vector2Int(x, y);
                // Create new neighbour cell if it doesn't exist
                if (!grid.ContainsKey(nPos)) {
                    grid[nPos] = new Cell { Pos = nPos, State = CellState.Dead };
                }
            }
        }

    }

    private IEnumerator UpdateBoard() {
        while (true) {
            tilemap.ClearAllTiles();
            foreach (var kvp in nextState) {
                if (kvp.Value.State == CellState.Alive) {
                    tilemap.SetTile((Vector3Int)kvp.Key, aliveTile);
                }
            }

            ClenseCells(nextState);
            currentState = CloneState(nextState);
            yield return new WaitForSeconds(updateInterval);
            UpdateNextState();
        }
    }
}
