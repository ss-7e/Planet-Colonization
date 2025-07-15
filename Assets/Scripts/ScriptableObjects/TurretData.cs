using UnityEngine;

[CreateAssetMenu(fileName = "New Turret", menuName = "Turret")]
public class TurretData : ScriptableObject
{
    public string turretName;
    public GameObject prefab;
    public int cost;
    public Sprite icon;
    [TextArea]
    public string description;
}
