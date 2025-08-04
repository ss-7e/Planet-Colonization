using System.Collections.Generic;
using UnityEngine;
using Game.Ammo;

[CreateAssetMenu(fileName = "NewPackedAmmo", menuName = "Ammo/PackedAmmo", order = 1)]
public class PackedAmmo : ScriptableObject
{
    [Header("Ammo Storage")]
    public List<ShellData> ammoStorageList;
    public List<int> ammoCount;
    [Header("Ammo Pack Rarity")]
    public ItemRarity rarity = ItemRarity.Common;
    public Sprite rarityIcon;
    

    public void InitTurretStorage(TurretAmmoStorage turretAmmoStorage)
    {
        if (turretAmmoStorage == null)
        {
            Debug.LogError("TurretAmmoStorage is null. Cannot initialize PackedAmmo.");
            return;
        }
        for (int i = 0; i < ammoStorageList.Count; i++)
        {
            ShellData shellData = ammoStorageList[i];
            int count = ammoCount[i];
            if (shellData != null && count > 0)
            {
                turretAmmoStorage.AddAmmo(shellData, count);
            }
            else
            {
                Debug.LogWarning($"Invalid ShellData or count for index {i}: {shellData}, {count}");
            }
        }
    }
}