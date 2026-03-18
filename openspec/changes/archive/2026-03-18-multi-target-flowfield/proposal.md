## Why

当前流场系统仅支持单一目标点（centerSpacecraft），敌人只能向同一个位置移动。需要让敌人能够自动寻路攻击最近的工厂，实现更真实的分散攻击行为。

## What Changes

- 修改 `FastSweepingSolver.Solve()` 支持多个目标点（goal seeds）
- 修改 `NavFlowField.Update()` 遍历所有工厂设置多个目标
- 敌人保持现有逻辑，沿流场梯度移动即可

## Capabilities

### New Capabilities

- `multi-target-flowfield`: 多目标流场能力，支持同时设置多个目标点，计算每个格子到最近目标的距离场，梯度自动指向最近目标

## Impact

- **涉及文件**: 
  - `Assets/Scripts/AI/HeatMap/NavFlowField.cs`
  - `Assets/Scripts/AI/HeatMap/FastSweepingSolver.cs`（位于 NavFlowField.cs 内）
- **依赖系统**: `BuildManager._factoryTowers`、`Grid.FactoryTowers`
- **性能**: 一张流场图供所有敌人使用，开销可控
