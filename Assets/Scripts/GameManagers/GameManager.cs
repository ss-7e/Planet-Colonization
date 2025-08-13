using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float galacticCredit { get; private set; } = 1000f;
    public float techPoints { get; private set; } = 0f; 
    private void Awake()
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
    private void Start()
    {
        if(UIManager.instance != null)
        {
            UIManager.instance.SetGalaxyCredit((int)galacticCredit);
        }
    }

    public void Update()
    {
        
    }
    public void AddGalacticCredit(float amount)
    {
        galacticCredit += amount;
        UIManager.instance.SetGalaxyCredit((int)galacticCredit);
    }

    public bool CostGalacticCredit(float amount)
    {
        if (galacticCredit >= amount)
        {
            galacticCredit -= amount;
            UIManager.instance.SetGalaxyCredit((int)galacticCredit);
            return true;
        }
        Debug.LogWarning("Not enough Galactic Credits. Required: " + amount + ", Available: " + galacticCredit);
        return false;
    }
}