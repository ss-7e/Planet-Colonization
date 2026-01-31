using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Towers
{
    public class Tower : MonoBehaviour
    {
        protected List<StoragePrev> storageList = new List<StoragePrev>(); //nearby storages 
        protected Dictionary<StoragePrev, Tower> storageTowerList = new Dictionary<StoragePrev, Tower>(); //nearby storage towers
        protected Dictionary<StoragePrev, float> storageDistance = new Dictionary<StoragePrev, float>();
        public Grid onGrid {  get; protected set; }
        public virtual void BuildOnGrid(Grid grid)
        {
            onGrid = grid;
        }

        public void addStorage(StoragePrev storage, float distance)
        {
            if (storageList.Contains(storage))
            {
                return;
            }
            storageList.Add(storage);
            storageDistance[storage] = distance;
            storageList.Sort((a, b) => storageDistance[a].CompareTo(storageDistance[b]));
        }

        public void addStorage(Tower tower, StoragePrev storage, float distance)
        {
            if (storageTowerList.ContainsKey(storage))
            {
                return;
            }
            if (storage == null)
            {
                Debug.LogError("Storage is null when adding to tower storage list.");
            }
            storageTowerList[storage] = tower;
            storageDistance[storage] = distance;
            storageList.Add(storage);
            storageList.Sort((a, b) => storageDistance[a].CompareTo(storageDistance[b]));
        }

        public void addStorage(StoragePrev storage)
        {
            storageList.Add(storage);
        }

        public List<StoragePrev> GetStorageList()
        {
            return storageList;
        }

        public Dictionary<StoragePrev, Tower> GetStorageTowerList()
        {
            return storageTowerList;
        }
    }
}