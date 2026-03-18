## ADDED Requirements

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

### Requirement: NavFlowField 与 BuildManager 集成
NavFlowField SHALL 从 BuildManager 获取所有工厂位置并设置为流场目标。

#### Scenario: 获取工厂位置
- **WHEN** NavFlowField.Update 被调用
- **THEN** 遍历 BuildManager.Instance._factoryTowers
- **AND** 将每个工厂的世界坐标转换为网格坐标
- **AND** 将所有工厂网格坐标传递给求解器作为目标

#### Scenario: 工厂列表为空处理
- **WHEN** BuildManager._factoryTowers 为空
- **THEN** 流场求解器跳过计算或使用默认目标
- **AND** 不抛出异常
