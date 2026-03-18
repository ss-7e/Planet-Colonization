## 1. 修改 FastSweepingSolver

- [x] 1.1 将 `Solve(int goalX, int goalY, ...)` 方法签名改为 `Solve(List<Vector2Int> goals, ...)`
- [x] 1.2 修改初始化逻辑，将所有 goals 中的坐标设置为 cost_field = 0
- [x] 1.3 保留原有单目标兼容接口或统一使用新接口

## 2. 修改 NavFlowField

- [x] 2.1 修改 `Update()` 方法，从 BuildManager.Instance._factoryTowers 获取工厂列表
- [x] 2.2 将每个工厂的世界坐标转换为网格坐标
- [x] 2.3 调用 `Solve(List<Vector2Int> goals)` 传入所有工厂坐标
- [x] 2.4 添加工厂列表为空时的处理逻辑

## 3. 验证与测试

- [x] 3.1 运行游戏验证敌人向最近工厂移动
- [x] 3.2 建造多个工厂验证敌人分散攻击
