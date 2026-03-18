## Purpose

将流场系统从单一目标点扩展为多目标点，使敌人能够自动追踪并攻击最近的工厂建筑。系统通过 Fast Sweeping Method 求解 Eikonal 方程，计算每个格子到最近工厂的距离和梯度方向。

## Requirements

### Requirement: Multi-target flow field
系统 SHALL 支持流场中的多个目标点，每个格子存储到最近目标的距离，梯度指向最近目标。

#### Scenario: 单目标流场
- **WHEN** 游戏中只有一个工厂
- **THEN** 流场梯度指向该工厂

#### Scenario: 多目标流场
- **WHEN** 游戏中存在多个工厂
- **THEN** 每个格子存储到最近工厂的距离
- **AND** 每个格子的梯度指向其最近的工厂

#### Scenario: 工厂建造时流场更新
- **WHEN** 新工厂被建造
- **THEN** 流场重新计算，将新工厂作为目标之一
- **AND** 敌人移动方向相应更新

### Requirement: Fast Sweeping Solver 多种子支持
FastSweepingSolver SHALL 接受目标坐标列表，并将所有目标初始化为 cost 值 0。

#### Scenario: 多种子初始化
- **WHEN** Solve 方法以目标坐标列表 [(x1,y1), (x2,y2)] 调用
- **THEN** cost_field[x1,y1] = 0 且 cost_field[x2,y2] = 0
- **AND** 其他所有格子初始化为 float.MaxValue

#### Scenario: 多种子收敛
- **WHEN** 多个目标点被设置
- **THEN** 当 maxChange < tolerance 时算法收敛
- **AND** 每个格子包含到任意目标的最短距离

### Requirement: NavFlowField 与 GridManager 集成
NavFlowField SHALL 从 GridManager 获取所有工厂位置并设置为流场目标。

#### Scenario: 获取工厂位置
- **WHEN** NavFlowField.Update 被调用
- **THEN** 遍历 GridManager.Instance.Grids
- **AND** 访问每个 Grid.FactoryTowers 获取工厂 GameObject
- **AND** 将每个工厂的世界坐标转换为网格坐标
- **AND** 将所有工厂网格坐标传递给求解器作为目标

#### Scenario: 工厂列表为空处理
- **WHEN** 所有 Grid.FactoryTowers 为空
- **THEN** 流场求解器跳过计算
- **AND** 不抛出异常

#### Scenario: 工厂建造时流场刷新
- **WHEN** BuildManager.OnBuildEvent 被触发
- **THEN** AIModule.OnBuild 设置 _needRefreshHeatMap = true
- **AND** 下一次 LateUpdate 时调用 HeatMapSet.Refresh()
- **AND** NavFlowField.Update 重新计算流场
