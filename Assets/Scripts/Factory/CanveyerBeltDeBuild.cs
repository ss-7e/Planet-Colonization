
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

        private void GetGridByDir(Grid grid, BeltDir dir, out Grid outGrid)
        {
            GridManager manager = GridManager.Instance;
            Vector2Int index = manager.GetGridXY(grid.Pos);
            switch (dir)
            {
                case BeltDir.up:
                    outGrid = manager.GetGridXY(index.x, index.y + 1);
                    break;
                case BeltDir.down:
                    outGrid = manager.GetGridXY(index.x, index.y - 1);
                    break;
                case BeltDir.left:
                    outGrid = manager.GetGridXY(index.x - 1, index.y);
                    break;
                case BeltDir.right:
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
            CanveyerBeltUnit unit = grid.ItemOutputFromBuilding as CanveyerBeltUnit;
            grid.ItemOutputFromBuilding = null;
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
