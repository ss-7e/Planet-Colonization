using UnityEngine;

namespace Game.UI
{

    /// <summary>
    ///  射线检测鼠标指向，外部使用检测结果
    /// </summary>
    public class PointAt : MonoBehaviour
    {
        public static PointAt Instance;
        private void Awake()
        {
            Instance = this;
        }

        public RaycastHit[] hits { get; private set; } = new RaycastHit[10];
        public RaycastHit buildHit { get; private set; }
        public RaycastHit defaltHit { get; private set; }
        public Grid gridHit { get; private set; }
        public Vector2Int gridPos { get; private set; }
        private void Update()
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, LayerMask.GetMask("Default"));
            defaltHit = hit;
            gridHit = GridManager.Instance.GetGridByPos(defaltHit.point, out Vector2Int gridPos);
            this.gridPos = gridPos;
            hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 100f);
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, LayerMask.GetMask("Build"));
            buildHit = hit;

        }
    }
}
