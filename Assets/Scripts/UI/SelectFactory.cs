using Game.UI;
using UnityEngine;

/// <summary>
/// 专门做建造在工厂塔上面的塔
/// </summary>
public class SelectFactory : MonoBehaviour
{
    public static SelectFactory instance;

    public GameObject factoryPreview;

    BuildSpot buildSpot = null;


    /// <summary>
    /// 直接生成预览建筑
    /// 外部添加预览建筑，外部判断能否建造
    /// </summary>
    /// <param name="factoryPrefab"></param>
    public void OnSelectFactory(GameObject factoryPrefab)
    {
        if (factoryPreview != null)
        {
            Destroy(factoryPreview);
        }
        factoryPreview = Instantiate(factoryPrefab);
        factoryPreview.SetActive(false);
    }

    /// <summary>
    /// 取消选择物体
    /// </summary>
    public void OnCancelSelect()
    {
        if (factoryPreview != null)
        {
            Destroy(factoryPreview);
            factoryPreview = null;
        }
    }


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
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (factoryPreview)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 100, LayerMask.GetMask("Build")))
            {

                // 可能会有多个BuildSpot？
                buildSpot = hit.collider.GetComponent<BuildSpot>();
                if (buildSpot)
                {
                    if(!buildSpot.building)
                    {
                        buildSpot.OnBuild(out Vector3 pos);
                        SpawnFactoryPreview(pos);
                    }
                }


                if(Input.GetMouseButtonDown(0))
                {
                    // Confirm build
                    factoryPreview.GetComponent<Factory.FactorySquare>().ConfirmBuild();
                    factoryPreview.GetComponent<BuildingProcess>().ConfirmBuild();
                    BuildManager.instance.ClearObjectToBuild();
                    factoryPreview = null;
                    buildSpot.ConfirmBuild();
                    buildSpot = null;
                    //TODO: 添加资源消耗等逻辑

                }
            }
            else
            {
                if (buildSpot)
                {
                    buildSpot.OffBuild();
                    buildSpot = null;
                    factoryPreview.SetActive(false);
                }
            }
        }
    }


    
    private void SpawnFactoryPreview(Vector3 pos)
    {
        factoryPreview.SetActive(true);
        factoryPreview.transform.position = pos;
    }

    
}