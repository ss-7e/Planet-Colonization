using System;
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
public enum ItemIDPrev : uint
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
    public static ItemIDPrev GetCategory(ItemIDPrev type)
    {
        return (ItemIDPrev)((uint)type & (uint)ItemIDPrev.CategoryMask);
    }

    // 提取生产阶段
    public static ItemIDPrev GetProductionStage(ItemIDPrev type)
    {
        return (ItemIDPrev)((uint)type & (uint)ItemIDPrev.StageMask);
    }

    // 提取品质等级
    public static ItemIDPrev GetGrade(ItemIDPrev type)
    {
        return (ItemIDPrev)((uint)type & (uint)ItemIDPrev.GradeMask);
    }

    // 提取具体型号ID
    public static byte GetModelId(ItemIDPrev type)
    {
        return (byte)((uint)type & (uint)ItemIDPrev.ModelMask);
    }

    // 判断是否为原材料
    public static bool IsRawMaterial(ItemIDPrev type)
    {
        return GetCategory(type) == ItemIDPrev.Category_RawMaterial;
    }

    // 判断是否为成品
    public static bool IsFinishedProduct(ItemIDPrev type)
    {
        return GetCategory(type) == ItemIDPrev.Category_Product;
    }

    // 判断是否需要质量检测
    public static bool RequiresQualityTest(ItemIDPrev type)
    {
        var stage = GetProductionStage(type);
        return stage == ItemIDPrev.Stage_Tested ||
               stage == ItemIDPrev.Stage_Calibrated ||
               stage == ItemIDPrev.Stage_Certified;
    }

    // 获取物品的生产流水线路径
    public static string GetProductionPath(ItemIDPrev type)
    {
        var category = GetCategory(type);
        var stage = GetProductionStage(type);

        return $"{GetCategoryName(category)} -> {GetStageName(stage)}";
    }

    private static string GetCategoryName(ItemIDPrev category)
    {
        return category switch
        {
            ItemIDPrev.Category_RawMaterial => "原材料采集",
            ItemIDPrev.Category_RefinedMaterial => "材料精炼",
            ItemIDPrev.Category_Component => "组件制造",
            ItemIDPrev.Category_Module => "模块组装",
            ItemIDPrev.Category_Product => "成品生产",
            ItemIDPrev.Category_Blueprint => "设计研发",
            ItemIDPrev.Category_Upgrade => "系统升级",
            ItemIDPrev.Category_Special => "特殊物品",
            _ => "未知分类"
        };
    }

    private static string GetStageName(ItemIDPrev stage)
    {
        return stage switch
        {
            ItemIDPrev.Stage_Mined => "采矿作业",
            ItemIDPrev.Stage_Harvested => "采集作业",
            ItemIDPrev.Stage_Synthesized => "化学合成",
            ItemIDPrev.Stage_Smelted => "高温熔炼",
            ItemIDPrev.Stage_Refined => "精密提纯",
            ItemIDPrev.Stage_Compressed => "高压压缩",
            ItemIDPrev.Stage_Fabricated => "量子制造",
            ItemIDPrev.Stage_Assembled => "纳米组装",
            ItemIDPrev.Stage_Integrated => "系统集成",
            ItemIDPrev.Stage_Tested => "性能测试",
            ItemIDPrev.Stage_Calibrated => "精密校准",
            ItemIDPrev.Stage_Certified => "质量认证",
            _ => "生产阶段"
        };
    }
}

public interface IItem
{
    int Id { get; } 
    ItemTypeA ItemType { get; }
}

/// <summary>
/// 物品ID类型 - 提供类型安全性和实用功能
/// </summary>
[Serializable]
public struct ItemID 
{
    #region 核心数据
    
    [SerializeField]
    private int _value;
    
    /// <summary>
    /// 原始ID值
    /// </summary>
    public readonly int Value => _value;
    
    // 特殊ID定义
    public static readonly ItemID None = new ItemID(0);
    public static readonly ItemID Invalid = new ItemID(-1);
    
    #endregion
    
    #region 构造函数和转换
    
    public ItemID(int id)
    {
        _value = id;
    }
    
    // 隐式转换（方便使用）
    public static implicit operator ItemID(int id) => new ItemID(id);
    public static implicit operator int(ItemID itemId) => itemId._value;
    
    // 显式转换（需要时使用）
    public static explicit operator ItemID(uint id) => new ItemID((int)id);

    #endregion

    #region 类型判断和分类 TODO:这些不对

    /// <summary>
    /// 是否有效ID
    /// </summary>
    public bool IsValid => _value > 0;
    
    /// <summary>
    /// 是否是货币类物品（假设1000-1999是货币）
    /// </summary>
    public bool IsCurrency => _value >= 1000 && _value < 2000;
    
    /// <summary>
    /// 是否是装备（假设2000-4999是装备）
    /// </summary>
    public bool IsEquipment => _value >= 2000 && _value < 5000;
    
    /// <summary>
    /// 是否是消耗品（假设5000-7999是消耗品）
    /// </summary>
    public bool IsConsumable => _value >= 5000 && _value < 8000;
    
    /// <summary>
    /// 是否是材料（假设8000-9999是材料）
    /// </summary>
    public bool IsMaterial => _value >= 8000 && _value < 10000;
    
    /// <summary>
    /// 获取物品大类
    /// </summary>
    public ItemCategory GetCategory()
    {
        if (!IsValid) return ItemCategory.None;
        if (IsCurrency) return ItemCategory.Currency;
        if (IsEquipment) return ItemCategory.Equipment;
        if (IsConsumable) return ItemCategory.Consumable;
        if (IsMaterial) return ItemCategory.Material;
        return ItemCategory.Other;
    }
    
    /// <summary>
    /// 获取物品子类型（根据具体项目规则）
    /// </summary>
    public int GetSubType()
    {
        if (!IsValid) return 0;
        
        // 示例：后三位是子类型
        return _value % 1000;
    }
    
    #endregion
    
    #region 配置表相关方法
    
    /// <summary>
    /// 从配置表获取物品配置（需要根据实际项目调整）
    /// </summary>

    
    /// <summary>
    /// 获取物品名称
    /// </summary>
    
    /// <summary>
    /// 获取物品图标
    /// </summary>

    
    /// <summary>
    /// 获取物品品质
    /// </summary>

    
    /// <summary>
    /// 是否可以堆叠
    /// </summary>

    /// <summary>
    /// 最大堆叠数量
    /// </summary>
    
    #endregion
    
    #region 实用方法
    
    /// <summary>
    /// 创建包含数量的物品实例
    /// </summary>
    public ItemInstance WithCount(int count = 1)
    {
        return new ItemInstance(this, count);
    }
    
    /// <summary>
    /// 判断两个ID是否属于同一物品类型（忽略ID中的特殊标记）
    /// </summary>
    public bool IsSameItemType(ItemID other)
    {
        // 示例：前4位相同即为同一物品类型
        int typeMask = 0xFFFFF00; // 掩码需要根据实际ID规则调整
        return (_value & typeMask) == (other._value & typeMask);
    }
    
    /// <summary>
    /// 是否为绑定物品（根据ID规则判断）
    /// </summary>
    public bool IsBound()
    {
        // 示例：最高位为1表示绑定
        return (_value & 0x80000000) != 0;
    }
    
    /// <summary>
    /// 获取解绑后的ID
    /// </summary>
    public ItemID GetUnboundID()
    {
        // 清除绑定标记位
        return new ItemID(_value & 0x7FFFFFFF);
    }
    
    #endregion
    
    #region 重写方法
    
    public override bool Equals(object obj)
    {
        return obj is ItemID other && Equals(other);
    }
    
    public bool Equals(ItemID other)
    {
        return _value == other._value;
    }
    
    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }
    
    
    public int CompareTo(ItemID other)
    {
        return _value.CompareTo(other._value);
    }
    
    #endregion
    
    #region 运算符重载
    
    public static bool operator ==(ItemID left, ItemID right) => left._value == right._value;
    public static bool operator !=(ItemID left, ItemID right) => left._value != right._value;
    public static bool operator <(ItemID left, ItemID right) => left._value < right._value;
    public static bool operator >(ItemID left, ItemID right) => left._value > right._value;
    public static bool operator <=(ItemID left, ItemID right) => left._value <= right._value;
    public static bool operator >=(ItemID left, ItemID right) => left._value >= right._value;
    
    #endregion
}

/// <summary>
/// 物品分类枚举
/// </summary>
public enum ItemCategory
{
    None = 0,
    Currency = 1,
    Equipment = 2,
    Consumable = 3,
    Material = 4,
    Other = 99
}

/// <summary>
/// 物品品质枚举
/// </summary>
public enum ItemQuality
{
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Epic = 3,
    Legendary = 4
}

/// <summary>
/// 物品配置类（示例）
/// </summary>
public class ItemConfig
{
    public ItemID ID;
    public string Name;
    public string Description;
    public Sprite Icon;
    public ItemQuality Quality;
    public ItemCategory Category;
    public int MaxStack = 1;
    // 其他配置字段...
}

/// <summary>
/// 物品实例（包含ID和数量）
/// </summary>
[Serializable]
public struct ItemInstance
{
    public ItemID ID;
    public int Count;
    
    public ItemInstance(ItemID id, int count = 1)
    {
        ID = id;
        Count = count;
    }
    
    public bool IsValid => ID.IsValid && Count > 0;
    
    public static ItemInstance None => new ItemInstance(ItemID.None, 0);
}

public interface IStorable : IItem // 能够在仓储界面看到的物品(似乎不用？
{
    int MaxCount { get; }
    int CurrentCount { get; set; }
    Sprite Icon { get; set; }

    bool SameItem(IStorable other);
}

