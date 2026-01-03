using UnityEngine;

public class GameModuleBase : MonoBehaviour
{
    private void Awake()
    {
        GameEntry.RegisterModule(this);
    }
}