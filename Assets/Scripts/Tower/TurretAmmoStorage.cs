using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Ammo;
public class TurretAmmoStorage
{
    Dictionary<ShellData, int> ammoStorage = new Dictionary<ShellData, int>();
    List<ShellData> ammoOrder = new List<ShellData>();
    public void AddAmmo(ShellData ShellData, int count)
    {
        if (ammoStorage.ContainsKey(ShellData))
        {
            ammoStorage[ShellData] += count;
        }
        else
        {
            ammoStorage.Add(ShellData, count);
            ammoOrder.Add(ShellData);
        }
    }
    public ShellData GetAmmo()
    {
        if (ammoOrder.Count == 0) return null;
        ShellData ShellData = ammoOrder[0];
        if (ammoStorage.ContainsKey(ShellData) && ammoStorage[ShellData] > 0)
        {
            ammoStorage[ShellData]--;
            if (ammoStorage[ShellData] == 0)
            {
                ammoOrder.RemoveAt(0);
                ammoStorage.Remove(ShellData);
            }
            return ShellData;
        }
        else
        {
            ammoOrder.RemoveAt(0);
            return GetAmmo(); // Try to get the next available ShellData
        }
    }
    public IReadOnlyList<ShellData> GetAmmoOrder()
    {
        return ammoOrder.AsReadOnly();
    }
    public int GetAmmoCount(ShellData ShellData)
    {
        if (ammoStorage.TryGetValue(ShellData, out int count))
        {
            return count;
        }
        return 0; // ShellData not found
    }
    public void SwapAmmoOrder(ShellData ShellData1, ShellData ShellData2)
    {
        int index1 = ammoOrder.IndexOf(ShellData1);
        int index2 = ammoOrder.IndexOf(ShellData2);
        if (index1 >= 0 && index2 >= 0)
        {
            ammoOrder[index1] = ShellData2;
            ammoOrder[index2] = ShellData1;
        }
        else if (index1 < 0 && index2 >= 0)
        {
            Debug.LogWarning($"ShellData {ShellData1.shellName} not found in ammo order.");
        }
        else if (index1 >= 0 && index2 < 0)
        {
            Debug.LogWarning($"ShellData {ShellData2.shellName} not found in ammo order.");
        }
        else
        {
            Debug.LogWarning("Both shells not found in ammo order.");
        }
    }
}
