using Game.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject healthBarPrefab;
    public TurretUI turretUI;
    private void Start()
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
}
