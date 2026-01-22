using System.Collections.Generic;
using UnityEngine;

public enum ItemTypeA
{
    NaturalResource,
    Module,
    ProcessingResource,
    Component,
    Ammo,
    Tower
}
[System.Flags]
public enum ItemType : uint
{
    // =============================================================== 位掩码定义 ===============================================================
    // 第一层：物品大类
    CategoryMask = 0xFF00_0000,

    // 第二层：生产阶段子类
    StageMask = 0x00FF_0000,

    // 第三层：具体类型 （无品级Item）
    TypeMask = 0x0000_FFFF,

    //---------------------------------------------------------- 品级 + 型号（成品） ----------------------------------------------------------
    // 第四层：品质等级
    GradeMask = 0x0000_FF00,   

    // 第五层：具体型号
    ModelMask = 0x0000_00FF,

    // =============================================================== 物品大类 ===============================================================
    // 基础材料大类
    Category_RawMaterial = 0x0100_0000,         // 原始原材料
    Category_RefinedMaterial = 0x0200_0000,     // 精炼材料
    Category_Component = 0x0300_0000,           // 基础组件
    Category_Module = 0x0400_0000,              // 功能模块
    Category_Product = 0x0500_0000,             // 成品产品
    Category_Blueprint = 0x0600_0000,           // 设计蓝图
    Category_Upgrade = 0x0700_0000,             // 升级核心
    Category_Special = 0x0800_0000,             // 特殊物品

    // =============================================================== 生产阶段子类 ===============================================================
    // 原材料阶段
    Stage_Mined         = 0x0001_0000,              // 采矿获得
    Stage_Harvested     = 0x0002_0000,              // 采集获得
    Stage_Synthesized   = 0x0003_0000,              // 合成获得

    // 加工阶段
    Stage_Smelted    = 0x0009_0000,                 // 熔炼
    Stage_Refined    = 0x000A_0000,                 // 精炼
    Stage_Compressed = 0x000B_0000,                 // 压缩

    // 制造阶段
    Stage_Fabricated = 0x0011_0000,                 // 基础制造
    Stage_Assembled  = 0x0012_0000,                 // 组装
    Stage_Integrated = 0x0013_0000,                 // 集成

    // 完成阶段
    Stage_Tested        = 0x0019_0000,              // 测试通过
    Stage_Calibrated    = 0x001A_0000,              // 校准完成
    Stage_Certified     = 0x001B_0000,              // 质量认证

    // =============================================================== 具体物品类型 ===============================================================

    // 原始原材料 (Category_RawMaterial)
    Raw_IronOre = Category_RawMaterial | Stage_Mined | 0x0001,              // 铁矿石  0x0101 0001
    Raw_Neutronium = Category_RawMaterial | Stage_Mined | 0x0002,           // 中子矿石
    Raw_PlasmaCrystal = Category_RawMaterial | Stage_Mined | 0x0003,        // 等离子晶体
    Raw_NanoFiber = Category_RawMaterial | Stage_Harvested | 0x0004,        // 纳米纤维
    Raw_QuantumDust = Category_RawMaterial | Stage_Synthesized | 0x0005,    // 量子尘埃

    // 精炼材料 (Category_RefinedMaterial)
    Refined_IronIngot   = Category_RefinedMaterial | Stage_Smelted | 0x0001,   // 铁锭
    Refined_Polymer     = Category_RefinedMaterial | Stage_Smelted | 0x0002,   // 聚合材料

    Refined_Alloy = Category_RefinedMaterial | Stage_Refined | 0x0002,   // 特种合金
    Refined_Superconductor = Category_RefinedMaterial | Stage_Compressed | 0x0003, // 超导材料

    // 基础组件 (Category_Component)
    Component_CircuitBoard = Category_Component | Stage_Fabricated | 0x0001,      // 量子电路板
    Component_PowerCell = Category_Component | Stage_Fabricated | 0x0002,      // 聚变电池
    Component_HydraulicArm = Category_Component | Stage_Assembled | 0x0003,       // 液压机械臂
    Component_SensorArray = Category_Component | Stage_Integrated | 0x0004,      // 传感器阵列

    // 功能模块 (Category_Module)
    Module_AIProcessor = Category_Module | Stage_Assembled | 0x0001,          // AI处理器
    Module_ShieldGen = Category_Module | Stage_Assembled | 0x0002,          // 护盾发生器
    Module_WarpDrive = Category_Module | Stage_Integrated | 0x0003,         // 曲率引擎
    Module_Teleporter = Category_Module | Stage_Integrated | 0x0004,         // 传送模块

    // 成品产品 (Category_Product)
    Product_IronBullet = Category_Product | Stage_Tested | 0x0101,              // 铁制子弹 0x0519_0101
    Product_RobotWorker = Category_Product | Stage_Tested | 0x0001,            // 工业机器人
    Product_HoverVehicle = Category_Product | Stage_Calibrated | 0x0002,        // 悬浮运输车
    Product_DefenseTurret = Category_Product | Stage_Certified | 0x0003,         // 自动防御炮塔
    Product_Replicator = Category_Product | Stage_Certified | 0x0004,         // 物质复制机

    // 设计蓝图 (Category_Blueprint)
    Blueprint_Standard = Category_Blueprint | 0x0000_0001,                     // 标准蓝图
    Blueprint_Advanced = Category_Blueprint | 0x0000_0002,                     // 高级蓝图
    Blueprint_Prototype = Category_Blueprint | 0x0000_0003,                     // 原型蓝图

    // 升级核心 (Category_Upgrade)
    Upgrade_Efficiency = Category_Upgrade | 0x0000_0001,                       // 效率核心
    Upgrade_Capacity = Category_Upgrade | 0x0000_0002,                       // 容量核心
    Upgrade_Speed = Category_Upgrade | 0x0000_0003,                         // 速度核心

    // 特殊物品 (Category_Special)
    Special_BlackHoleCore = Category_Special | 0x0000_0001,                       // 黑洞核心
    Special_TimeCrystal = Category_Special | 0x0000_0002,                       // 时间晶体
    Special_AntimatterCell = Category_Special | 0x0000_0003,                       // 反物质单元

    // =============================================================== 品质等级定义 ===============================================================
    // 使用低16位中的高8位表示品质
    Grade_Standard  = 0x0000_0100,      // 标准级
    Grade_Enhanced  = 0x0000_0200,      // 增强级
    Grade_Superior  = 0x0000_0300,      // 优等级
    Grade_Elite     = 0x0000_0400,      // 精英级
    Grade_Legendary = 0x0000_0500,      // 传奇级

    // =============================================================== 预定义复合物品 ===============================================================
    // 带有品质的具体物品（示例）
    Standard_RobotWorker = Product_RobotWorker | Grade_Standard,
    Enhanced_RobotWorker = Product_RobotWorker | Grade_Enhanced,
    Superior_AIProcessor = Module_AIProcessor | Grade_Superior,
    Elite_WarpDrive = Module_WarpDrive | Grade_Elite,

    // 具体型号示例（使用低8位）
    RobotWorker_MK1 = Product_RobotWorker  | 0x0000_0001,
    RobotWorker_MK2 = Product_RobotWorker  | 0x0000_0002,
    HoverVehicle_V1 = Product_HoverVehicle | 0x0000_0001,
    HoverVehicle_V2 = Product_HoverVehicle | 0x0000_0002,
}

public static class FactoryItemTypeParser
{
    // 提取大类
    public static ItemType GetCategory(ItemType type)
    {
        return (ItemType)((uint)type & (uint)ItemType.CategoryMask);
    }

    // 提取生产阶段
    public static ItemType GetProductionStage(ItemType type)
    {
        return (ItemType)((uint)type & (uint)ItemType.StageMask);
    }

    // 提取品质等级
    public static ItemType GetGrade(ItemType type)
    {
        return (ItemType)((uint)type & (uint)ItemType.GradeMask);
    }

    // 提取具体型号ID
    public static byte GetModelId(ItemType type)
    {
        return (byte)((uint)type & (uint)ItemType.ModelMask);
    }

    // 判断是否为原材料
    public static bool IsRawMaterial(ItemType type)
    {
        return GetCategory(type) == ItemType.Category_RawMaterial;
    }

    // 判断是否为成品
    public static bool IsFinishedProduct(ItemType type)
    {
        return GetCategory(type) == ItemType.Category_Product;
    }

    // 判断是否需要质量检测
    public static bool RequiresQualityTest(ItemType type)
    {
        var stage = GetProductionStage(type);
        return stage == ItemType.Stage_Tested ||
               stage == ItemType.Stage_Calibrated ||
               stage == ItemType.Stage_Certified;
    }

    // 获取物品的生产流水线路径
    public static string GetProductionPath(ItemType type)
    {
        var category = GetCategory(type);
        var stage = GetProductionStage(type);

        return $"{GetCategoryName(category)} -> {GetStageName(stage)}";
    }

    private static string GetCategoryName(ItemType category)
    {
        return category switch
        {
            ItemType.Category_RawMaterial => "原材料采集",
            ItemType.Category_RefinedMaterial => "材料精炼",
            ItemType.Category_Component => "组件制造",
            ItemType.Category_Module => "模块组装",
            ItemType.Category_Product => "成品生产",
            ItemType.Category_Blueprint => "设计研发",
            ItemType.Category_Upgrade => "系统升级",
            ItemType.Category_Special => "特殊物品",
            _ => "未知分类"
        };
    }

    private static string GetStageName(ItemType stage)
    {
        return stage switch
        {
            ItemType.Stage_Mined => "采矿作业",
            ItemType.Stage_Harvested => "采集作业",
            ItemType.Stage_Synthesized => "化学合成",
            ItemType.Stage_Smelted => "高温熔炼",
            ItemType.Stage_Refined => "精密提纯",
            ItemType.Stage_Compressed => "高压压缩",
            ItemType.Stage_Fabricated => "量子制造",
            ItemType.Stage_Assembled => "纳米组装",
            ItemType.Stage_Integrated => "系统集成",
            ItemType.Stage_Tested => "性能测试",
            ItemType.Stage_Calibrated => "精密校准",
            ItemType.Stage_Certified => "质量认证",
            _ => "生产阶段"
        };
    }
}

public interface IItem
{
    int Id { get; } 
    ItemTypeA ItemType { get; }
}

public interface IStorable : IItem // 能够在仓储界面看到的物品
{
    int MaxCount { get; }
    int CurrentCount { get; set; }
    Sprite Icon { get; set; }

    bool SameItem(IStorable other);
}

