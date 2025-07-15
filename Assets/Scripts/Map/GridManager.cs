using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
public enum MapType
{
    Square,
    Hexagon
}
public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    public MapType mapType;
    public Grid[,] grid;
    public int length { get; set; }
    public int width { get; set; }
    void Awake()
    {
        instance = this;
    }
}
