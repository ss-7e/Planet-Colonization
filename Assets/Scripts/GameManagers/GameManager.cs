using Factory;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
   


    //玩家参数，可以打包为一个类
    public float GalacticCredit { get; private set; } = 1000f;
    public float TechPoints { get; private set; } = 0f; 
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        if(UIManager.instance != null)
        {
            UIManager.instance.SetGalaxyCredit((int)GalacticCredit);
        }
    }

    public void Update()
    {
        
    }
    public void AddGalacticCredit(float amount)
    {
        GalacticCredit += amount;
        UIManager.instance.SetGalaxyCredit((int)GalacticCredit);
    }

    public bool CostGalacticCredit(float amount)
    {
        if (GalacticCredit >= amount)
        {
            GalacticCredit -= amount;
            UIManager.instance.SetGalaxyCredit((int)GalacticCredit);
            return true;
        }
        Debug.LogWarning("Not enough Galactic Credits. Required: " + amount + ", Available: " + GalacticCredit);
        return false;
    }
}