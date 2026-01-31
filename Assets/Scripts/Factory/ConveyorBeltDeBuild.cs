
using Factory;
using Game.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Factory
{

    /// <summary>
    /// 传送带拆除逻辑
    /// 
    /// </summary>
    public class CanveyerBeltDebuild
    {

        private void GetGridByDir(Grid grid, PortDir dir, out Grid outGrid)
        {
            GridManager manager = GridManager.Instance;
            Vector2Int index = manager.GetGridXYValue(grid.Pos);
            switch (dir)
            {
                case PortDir.up:
                    outGrid = manager.GetGridXY(index.x, index.y + 1);
                    break;
                case PortDir.down:
                    outGrid = manager.GetGridXY(index.x, index.y - 1);
                    break;
                case PortDir.left:
                    outGrid = manager.GetGridXY(index.x - 1, index.y);
                    break;
                case PortDir.right:
                    outGrid = manager.GetGridXY(index.x + 1, index.y);
                    break;
                default:
                    outGrid = null;
                    break;
            }
        }


        //调用的时候需要确定grid上的确有一个传送带单元
        public void CanveyerBeltUnitOnGridDebuild(Grid grid)
        {
            GameObject unitObject = grid.BuildingOnGrid;
            ConveyorBeltUnit unit = grid.ItemOutputFromBuilding as ConveyorBeltUnit;
            if (unit == null) return;
            if(unit.Connection.From != null)
            {
                
            }
            else
            {
                GetGridByDir(grid, unit.InputDir, out Grid fromGrid);
                if (fromGrid != null)
                {

                }
            }


            Object.Destroy(unitObject);
        }
    }
}
