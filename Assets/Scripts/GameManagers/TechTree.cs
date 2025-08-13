using UnityEngine;

using System.Collections.Generic;


public class TechNode
{
    public string name;
    public string description;
    public float cost;
    public bool isUnlocked;
    public TechNode(string name, string description, float cost)
    {
        this.name = name;
        this.description = description;
        this.cost = cost;
        this.isUnlocked = false;
    }
    public void Unlock()
    {
        isUnlocked = true;
    }
}

public class TechTree : MonoBehaviour
{
    public static TechTree instance;
    private Dictionary<string, TechNode> techNodes = new Dictionary<string, TechNode>();
    
}