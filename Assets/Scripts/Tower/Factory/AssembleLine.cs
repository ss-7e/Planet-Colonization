using UnityEngine;
namespace Game.Towers.Factory
{
    public abstract class AssembleLine : ScriptableObject
    {
        public float produceTime;
        public int produceCountPerCycle;
        public ItemType productType;
    }
}
