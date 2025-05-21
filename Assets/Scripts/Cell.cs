using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Cell
{
    public CellState State;
    public Vector2Int Pos;
}
