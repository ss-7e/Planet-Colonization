using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Towers
{
    public class Tower : MonoBehaviour
    {
        protected List<Storage> storageList = new List<Storage>(); //nearby storages 
        protected Dictionary<Storage, Tower> storageTowerList = new Dictionary<Storage, Tower>(); //nearby storage towers
        protected Dictionary<Storage, float> storageDistance = new Dictionary<Storage, float>();
        public Grid onGrid {  get; protected set; }
        public virtual void BuildOnGrid(Grid grid)
        {
            onGrid = grid;
        }

        public void addStorage(Storage storage, float distance)
        {
            if (storageList.Contains(storage))
            {
                return;
            }
            storageList.Add(storage);
            storageDistance[storage] = distance;
            storageList.Sort((a, b) => storageDistance[a].CompareTo(storageDistance[b]));
        }

        public void addStorage(Tower tower, Storage storage, float distance)
        {
            if (storageTowerList.ContainsKey(storage))
            {
                Debug.LogError("Storage tower already added to this tower!");
                return;
            }
            storageTowerList[storage] = tower;
            storageDistance[storage] = distance;
            storageList.Add(storage);
            storageList.Sort((a, b) => storageDistance[a].CompareTo(storageDistance[b]));
        }

        public void addStorage(Storage storage)
        {
            storageList.Add(storage);
        }

        public List<Storage> GetStorageList()
        {
            return storageList;
        }

        public Dictionary<Storage, Tower> GetStorageTowerList()
        {
            return storageTowerList;
        }
    }
}