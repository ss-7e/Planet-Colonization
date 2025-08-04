using UnityEngine;
using UnityEngine.UI;

public class HealthBarControl : MonoBehaviour
{
    [Range(0f, 1f)]
    public float fillAmount = 1f;

    private Material runtimeMat;

    void Start()
    {
        Image image = GetComponent<Image>();

        runtimeMat = Instantiate(image.material);
        image.material = runtimeMat;

        runtimeMat.SetFloat("_FillAmount", fillAmount);
    }
    public void UpdateHealthBar(float health, float maxHealth)
    {
        if (maxHealth <= 0) return; 
        fillAmount = Mathf.Clamp01(health / maxHealth);
        SetFillAmount(fillAmount);
    }
    public void SetFillAmount(float amount)
    {
        fillAmount = Mathf.Clamp01(amount);
        if (runtimeMat != null)
        {
            runtimeMat.SetFloat("_FillAmount", fillAmount);
        }
    }
}
