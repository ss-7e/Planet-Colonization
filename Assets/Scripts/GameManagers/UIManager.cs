using Game.UI;
using UnityEngine;
using UnityEngine.UI;
using Game.Towers;
using Game.Towers.Turrets;
using Game.Towers.Factory;
using TMPro;


public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject healthBarPrefab;
    public TurretUI turretUI;
    public FactoryUI factoryUI;
    [SerializeField] TextMeshProUGUI GalaxyCreditUI;
    public Image downSelectionBarFrame;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetGalaxyCredit(int amount)
    {
        if (GalaxyCreditUI != null)
        {
            GalaxyCreditUI.text = amount.ToString();
        }
        else
        {
            Debug.LogWarning("GalaxyCreditUI is not assigned in the UIManager.");
        }
    }

    public void setHealthBar(GameObject entity)
    {
        if (entity == null)
        {
            return;
        }
        GameObject healthBar = Instantiate(healthBarPrefab, entity.transform);
        healthBar.transform.localPosition = new Vector3(0, 2f, 0);
        healthBar.transform.localScale = new Vector3(1, 1, 1);
    }

    public void SetLevelTime(float time)
    {
        // Assuming you have a UI element to display the level time
        // Update the UI element with the level time
    }


    public void SetTowerUI(Tower tower, bool isLeft)
    {
        switch(tower)
        {
            case TurretBase turret:
                turretUI.SetUI(turret, isLeft);
                break;
            case FactroyTowerBase factory:
                factoryUI.SetUI(factory, isLeft);
                break;
            default:
                Debug.LogWarning("Unsupported tower type for UI: " + tower.GetType());
                break;
        }
    }

    public void HideTowerUI(Tower tower)
    {
        switch(tower)
        {
            case TurretBase turret:
                turretUI.HideUI(turret);
                break;
            case FactroyTowerBase factory:
                factoryUI.HideUI(factory);
                break;
            default:
                Debug.LogWarning("Unsupported tower type for hiding UI: " + tower.GetType());
                break;
        }
    }
}
