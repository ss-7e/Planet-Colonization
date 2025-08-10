using System.Collections.Generic;
using UnityEngine;
namespace Game.Towers.Factory
{
    public class AssembleLine : ScriptableObject
    {
        [SerializeField]protected float assembleTime;
        [SerializeField]IStorable[] inputItem;
        [SerializeField]List<IStorable> outputItem;

    }
}
