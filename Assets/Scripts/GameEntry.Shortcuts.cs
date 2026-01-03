
public static partial class GameEntry
{
    /// <summary>
    /// 当前关卡
    /// TODO LevelData 改成 Level
    /// </summary>
    public static Level.LevelData CurrentLevel
    {
        get
        {
            Level.LevelManager levelManager = GetModule<Level.LevelManager>();
            return levelManager.CurrentLevelData;
        }
    }
}