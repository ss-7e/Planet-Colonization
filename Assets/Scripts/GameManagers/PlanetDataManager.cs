using UnityEngine;

public class PlanetDataManager : MonoBehaviour
{
    
    public static PlanetDataManager instance;
    public Vector3 gravity = new Vector3(0, -2f, 0);
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
