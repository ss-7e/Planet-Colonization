## Context

当前流场系统使用 Fast Sweeping Method (FSM) 求解 Eikonal 方程，仅支持单一目标点。`NavFlowField.SetGoal()` 和 `FastSweepingSolver.Solve(goalX, goalY)` 只接受一个目标坐标。

需求变更：敌人应攻击最近的工厂，而非固定位置。需要修改为多目标流场。

## Goals / Non-Goals

**Goals:**
- 支持多个目标点的流场计算
- 每个格子存储到最近目标的距离，梯度指向最近目标
- 保持单张流场图供所有敌人使用

**Non-Goals:**
- 不考虑工厂被摧毁后的重路由
- 不考虑敌人分配算法的优化（如负载均衡）

## Decisions

### Decision 1: 修改 Solve 方法接受多个目标点

**选择**: 将 `Solve(goalX, goalY)` 改为 `Solve(List<Vector2Int> goals)`

**替代方案**:
- 方案 A: 多次调用 Solve 并取最小值 → 多次迭代，开销大
- 方案 B: 修改 Solve 接受多个种子点 → 一次迭代，开销可控 ✓

**实现方式**:
初始化时将所有目标点设置为 `cost_field = 0`，其余为 `float.MaxValue`，然后执行标准扫描。

### Decision 2: 工厂位置获取方式

**选择**: 通过 `BuildManager._factoryTowers` 获取工厂列表

**理由**: BuildManager 已维护所有工厂引用，无需额外系统。

### Decision 3: 流场更新策略

**选择**: 工厂建造/销毁时触发流场刷新

**理由**: 与现有 `OnBuildEvent` 机制保持一致，改动最小。

## Risks / Trade-offs

| Risk | Mitigation |
|------|------------|
| 工厂列表为空时流场无意义 | 添加检查，工厂为空时使用默认行为或跳过求解 |
| 大量工厂时求解迭代次数增加 | 8 次迭代通常足够，可根据需求调整 maxIterations |

## Open Questions

1. 是否需要在运行时频繁更新流场？（工厂建造频率）
2. 是否需要为不同敌人类型设置不同目标优先级？
