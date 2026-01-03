using System.Collections.Generic;
using UnityEngine;

public static partial class GameEntry
{
    private static Dictionary<System.Type, GameModuleBase> s_gameModuleMap = new();

    public static void RegisterModule(GameModuleBase gameModule)
    {
        if (s_gameModuleMap.ContainsKey(gameModule.GetType()))
        {
            Debug.LogError($"Module of type {gameModule.GetType()} is already registered.");
            return;
        }
        s_gameModuleMap[gameModule.GetType()] = gameModule;
    }

    public static GameModuleBase GetModule(System.Type type)
    {
        return s_gameModuleMap.GetValueOrDefault(type);
    }

    public static T GetModule<T>() where T : GameModuleBase
    {
        return GetModule(typeof(T)) as T;
    }
}