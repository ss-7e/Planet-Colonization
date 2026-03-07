using UnityEngine;

namespace Game
{
    public class GameEntry : MonoBehaviour
    {
        public static GameEntry Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("GameEntry 已经存在！");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // 加载数据表
            SheetDatabase.LoadData();

            // TODO 逐步将 Manager 初始化搬迁到这里
            // 注意区分全局 Manager 和局内 Manager，局内的 Manager 应该由 GameMode 管理，
            // 这里只管理全局 Manager。
        }

        private void Update()
        {
            // TODO 逐步将 Manager 的 Update 搬迁到这里
        }
    }
}