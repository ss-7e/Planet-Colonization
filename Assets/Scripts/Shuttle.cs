using UnityEngine;
using UnityEngine.EventSystems;

public class Shuttle : MonoBehaviour, IDamageable, IPointerClickHandler
{
    public HealthComponent healthComp { get; private set; }
    public Vector2Int OccupyGrids;
    public float defaltHealth;
    public HealthBarControl healthBarControl;
    public float wearResistance { get; private set; }
    [SerializeField] protected ParticleSystem explosion;
    [SerializeField] private GameObject shuttleUI;

    private void Awake()
    {
        healthComp = new HealthComponent(defaltHealth, defaltHealth);
        if (healthComp == null)
        {
            Debug.LogError("HealthComponent is missing on Shuttle.");
        }
    }

    public void SetShipGrids()
    {
        Vector2Int basicGrid = GridManager.instance.GetGridXY(transform.position);
        for (int i = -OccupyGrids.x; i < OccupyGrids.x; i++)
        {
            for (int j = -OccupyGrids.y; j < OccupyGrids.y; j++)
            {
                Vector2Int gridPos = new Vector2Int(basicGrid.x + i, basicGrid.y + j);
                GridManager.instance.GetGridXY(gridPos.x, gridPos.y).isShipyard = true;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        healthComp.TakeDamage(damage);
        healthBarControl.UpdateHealthBar(healthComp.health, healthComp.maxHealth);
        if (healthComp.health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Shuttle destroyed.");
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
        PauseFailManager.instance.TriggerFail();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (shuttleUI != null)
        {
            shuttleUI.SetActive(!shuttleUI.activeSelf);
        }
    }
}
