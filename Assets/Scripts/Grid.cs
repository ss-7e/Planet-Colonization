using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum GridType
{
    Empty,
    Wall,
    Start,
    End,
    Path
}
public class Grid
{
    public Grid(Vector3 gridPos)
    {
        pos = gridPos;
    }
    public Vector3 pos { get; set; }
    GridType type;
}
