using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ShopButtomSet : MonoBehaviour
{
    public TextMeshProUGUI goodsName;
    public TextMeshProUGUI goodsAmount;
    public TextMeshProUGUI goodsPrice;
    public Image icon;
    public float price;
    public void SetText(string name, int amount, float price, Image icon)
    {
        goodsName.text = name;
        goodsAmount.text = amount.ToString();
        goodsPrice.text = price.ToString("F2");
        this.icon.sprite = icon.sprite;
    }
    public void BuyTowerItem(TowerPackedItem item)
    {
        if (GameManager.instance.CostGalacticCredit(price))
        {
            Cargo.instance.AddItem(item);
        }
    }

    public void Buy()
    {
        GameManager.instance.CostGalacticCredit(price);
    }
}
