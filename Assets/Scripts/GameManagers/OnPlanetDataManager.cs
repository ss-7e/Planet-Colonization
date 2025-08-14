using UnityEngine;

public class OnPlanetDataManager : MonoBehaviour
{
    
    public static OnPlanetDataManager instance;
    public Vector3 gravity = new Vector3(0, -2f, 0);
    public int fixingRobotCount = 0;
    public int transportRobotCount = 0;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
