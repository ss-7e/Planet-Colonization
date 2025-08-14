using UnityEngine;
using Game.Towers.Factory;
using System.Collections.Generic;

public class TechManager : MonoBehaviour
{
    public static TechManager instance;

    [SerializeField]List<AssembleLine> assembleLines = new List<AssembleLine>();
    public List<AssembleLine> activeLines { get; private set; } = new List<AssembleLine>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        foreach (AssembleLine line in assembleLines)
        {
            ActiveAssembleLine(line);
        }
    }
    public void ActiveAssembleLine(AssembleLine line)
    {
        if (line == null || activeLines.Contains(line))
            return;
        activeLines.Add(line);
        if (!assembleLines.Contains(line))
        {
            assembleLines.Add(line);
        }
    }

}