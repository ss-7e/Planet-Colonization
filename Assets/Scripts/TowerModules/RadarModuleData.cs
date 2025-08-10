using UnityEngine;
using Game.Towers.Turrets;
using NUnit.Framework;
using System.Collections.Generic;

namespace Game.Modules
{
    public class RadarModuleData : ModuleData
    {
        public RadarModuleData(RadarModule module)
        {
            this.module = module;

        }
    }
}
