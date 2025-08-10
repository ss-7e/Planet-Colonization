using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


namespace Game.Towers
{
    public class StorageTower : Tower
    {
        [SerializeField] protected int storageLength = 8;
        [SerializeField] protected int storageWidth = 4;
        [SerializeField] protected Storage storage = null;

        public void Start()
        {
            storage = new Storage(storageLength * storageWidth);
        }

        public Storage GetStorage()
        {
            return storage;
        }
    }
}
