using UnityEngine;

/// <summary>
/// 挂载碰撞体上作为可建筑位置
/// </summary>
public class BuildSpot : MonoBehaviour
{
    public bool building { get; private set; } = false;
    public bool builded { get; private set; } = false;
    private void Start()
    {
        if (this.gameObject.GetComponent<Collider>() == null)
        {
            this.gameObject.AddComponent<BoxCollider>();
        }
        Collider collider = this.gameObject.GetComponent<Collider>();
        collider.isTrigger = true;
    }


    /// <summary>
    /// 鼠标移入调用，生成预览建筑
    /// </summary>
    public void OnBuild(out Vector3 pos)
    {
        //gameObject.GetComponent<Collider>().enabled = false;
        building = true;
        pos = this.transform.position;
        pos.y += 0.5f;
    }


    /// <summary>
    /// 
    /// 鼠标移出调用，移除预览建筑，重启碰撞体
    /// 
    /// </summary>
    public void OffBuild()
    {
        building = false;
        gameObject.GetComponent<Collider>().enabled = true;
    }

    /// <summary>
    /// 确认建造
    /// </summary>
    public void ConfirmBuild()
    {
        building = false;
        gameObject.GetComponent<Collider>().enabled = false;
        builded = true;
    }

}