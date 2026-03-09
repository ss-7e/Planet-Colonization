# Agent Guidelines

---

## 项目概述
此项目为基于 Unity 的工厂与塔防结合的游戏。

---

## 文件索引
游戏脚本：Assets/Scripts/
开发工具：Tools/

---

## C# 代码编写指南
### 代码风格
- 类名：PascalCase
- 接口名：I+PascalCase
- 枚举名：E+PascalCase
- 方法名：PascalCase，必须为“动词”或“动词+名词”的形式
- 公共成员变量名：PascalCase
- 私有成员变量名：_+camelCase
- 局部变量名：camelCase

使用4个空格缩进，大括号应当换行。

### 命名空间
- namespace Game：C# 游戏逻辑
- namespace Game.Editor：C# 编辑器逻辑