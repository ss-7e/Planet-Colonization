using UnityEngine;

public class BuildingProcess : MonoBehaviour
{
    [SerializeField] Material transparentMaterial;
    [SerializeField] Material setMaterial;

    private void Start()
    {
        this.gameObject.GetComponent<Renderer>().material = transparentMaterial;
    }
    public void Building(GameObject buildOn)
    {
       Factory.FactorySquare factorySquare = buildOn.GetComponent<Factory.FactorySquare>();
       this.gameObject.GetComponent<Renderer>().material = transparentMaterial;
    }

    /// <summary>
    /// 单击确认建造
    /// </summary>
    public void ConfirmBuild()
    {
        this.gameObject.GetComponent<Renderer>().material = setMaterial;

    }
}