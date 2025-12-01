using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct GridPathDirection
{
    public Vector2 direction;
}

public class GridPathManager : MonoBehaviour
{
    [SerializeField] private Vector2Int destination;
    [SerializeField] private GameObject debugArrowPrefab;
    public void SetupGridPathfinding()
    {
        //BFS 给每个格子上流场
        GridManager gridManager = GridManager.instance;
        int length = gridManager.length;
        int width = gridManager.width;
        int[] distances = new int[length * width];
        Array.Fill(distances, int.MaxValue);
        distances[destination.y * width + destination.x] = 0;
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(destination);
        while(queue.Count > 0)
        {
            
        }
    }
}