
using Factory;
using Game.UI;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Factory
{

    /// <summary>
    /// 完成传送带建造过程逻辑
    /// 预览传送带路径（第一次点击）
    /// 确定建造逻辑（第二次点击）：添加到grid访问，连接前后传送带单元
    /// </summary>
    public class ConveyorBeltBuild 
    {
        
        GameObject _beltMeshDirect;
        GameObject _beltMeshTurnLeft;
        GameObject _beltMeshTurnRight;

        enum PreviewDirection
        {
            Horizontal, //先水平（x轴）
            Vertical    //先垂直 (z轴)
        }

        PortDir _curBeltInputDir = PortDir.down;
        bool _startBuild = false;
        bool _buildable = true;
        Grid _startGrid, _endGrid;
        List<Grid> _markPointGrids = new();
        List<GameObject> _previewBeltList = new();
        List<PortDir> _inputDirs = new();
        List<PortDir> _outputDirs = new();
        int _firstIdx;
        PreviewDirection _direction;
        IItemInput _connectTo;
        IConnection _connectFrom = null;

        CanveyerBeltDebuild _debuild = new();

        public void Awake()
        {
            _beltMeshDirect = BuildManager.Instance.CanveyerBelts[0];
            _beltMeshTurnLeft = BuildManager.Instance.CanveyerBelts[1];
            _beltMeshTurnRight = BuildManager.Instance.CanveyerBelts[2];
        }

        /// <summary>
        /// 鼠标第一次点击时调用，初始化建造过程
        /// </summary>
        private void StartBuild()
        {
            _inputDirs.Clear();
            _outputDirs.Clear();
            _markPointGrids.Clear();
            _previewBeltList.Clear();
            _firstIdx = 0;


            _startGrid = PointAt.Instance.gridHit;
            if(_startGrid == null) return;
            _connectFrom = _startGrid.ItemOutputFromBuilding;
            _curBeltInputDir = _startGrid.ItemOutputFromBuildingDir.Count > 0 ? _startGrid.ItemOutputFromBuildingDir[0].Item2 : PortDir.down;
        }

        public void Update()
        {
            if(!_startBuild)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    StartBuild();
                    _startBuild = true;
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    Grid grid = PointAt.Instance.gridHit;
                    if(grid != null && grid.ItemOutputFromBuilding is ConveyorBeltUnit)
                    {
                        _debuild.CanveyerBeltUnitOnGridDebuild(grid);
                    }
                }

                return;
            }
            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    curBeltInputDir = (PortDir)(((int)curBeltInputDir + 1) % 4);
            //    Debug.Log(curBeltInputDir);
            //}
            AddMarkPoint();
            RefreshBuildPreview(); 
            if (Input.GetMouseButtonUp(0))
            {
                ConfirmBuild();
                _startBuild = false;
            }
        }


        // 当鼠标移动时刷新预览
        // TODO: 检测跨过其他传送带的情况（是否要禁止建造？）
        private void RefreshBuildPreview()
        {
            Grid currGrid = PointAt.Instance.gridHit;
            if (currGrid == null || _endGrid == currGrid)
            {
                return;
            }
            _endGrid = currGrid;
            if (_connectFrom != null)
            {
                
            }

            for (int i = _previewBeltList.Count - 1; i >= _firstIdx; i--)
            {
                GameObject.Destroy(_previewBeltList[i]);
                _inputDirs.RemoveAt(i);
                _outputDirs.RemoveAt(i);
                _previewBeltList.RemoveAt(i);
            }

            if (currGrid == _startGrid)
            {
                _previewBeltList.Add(BuildBeltUnitPreview(_startGrid.Pos, _curBeltInputDir, _curBeltInputDir.Opposite()));
                return;
            }
            else if (currGrid.Pos.x == _startGrid.Pos.x)
            {
                _direction = PreviewDirection.Vertical;
                if(_firstIdx == 0 && _connectFrom == null)
                {
                    _curBeltInputDir = currGrid.Pos.z - _startGrid.Pos.z < 0 ? PortDir.up : PortDir.down;
                }
            }
            else if (currGrid.Pos.z == _startGrid.Pos.z)
            {
                _direction = PreviewDirection.Horizontal;
                if (_firstIdx == 0 && _connectFrom == null)
                {
                    _curBeltInputDir = currGrid.Pos.x - _startGrid.Pos.x < 0 ? PortDir.right : PortDir.left;
                }
            }

            GenerateBuildPreview();
        }


        private void GenerateBuildPreview()
        {
            if (_firstIdx > 0) { _curBeltInputDir = _outputDirs[_firstIdx - 1].Opposite(); }
            if (_direction == PreviewDirection.Horizontal) // x轴对齐
            {
                //建造上一个标记点位置的传送带
                if (_endGrid.Pos.x - _startGrid.Pos.x < 0) 
                {
                    _previewBeltList.Add(BuildBeltUnitPreview(_startGrid.Pos, _curBeltInputDir, PortDir.left)); 
                }
                else
                {
                    _previewBeltList.Add(BuildBeltUnitPreview(_startGrid.Pos, _curBeltInputDir, PortDir.right)); 
                }

                int count = (int)Mathf.Abs(_endGrid.Pos.x - _startGrid.Pos.x);
                for (int i = 1; i < count; i++)
                    {
                        Vector3 pos = new Vector3(_startGrid.Pos.x + i * Mathf.Sign(_endGrid.Pos.x - _startGrid.Pos.x), _startGrid.Pos.y, _startGrid.Pos.z);
                        if (Mathf.Sign(_endGrid.Pos.x - _startGrid.Pos.x) > 0)
                        {
                            GameObject previewBelt = BuildBeltUnitPreview(pos, PortDir.left, PortDir.right);
                            _previewBeltList.Add(previewBelt);
                        }
                        else
                        {
                            GameObject previewBelt = BuildBeltUnitPreview(pos, PortDir.right, PortDir.left);
                            _previewBeltList.Add(previewBelt);
                        }
                    }

                //拐角点---------------------------------
                if(_endGrid.Pos.z - _startGrid.Pos.z > 0) 
                { _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_endGrid.Pos.x, _startGrid.Pos.y, _startGrid.Pos.z), Mathf.Sign(_endGrid.Pos.x - _startGrid.Pos.x) > 0 ? PortDir.left : PortDir.right, PortDir.up)); }
                else if(_endGrid.Pos.z - _startGrid.Pos.z < 0)
                { _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_endGrid.Pos.x, _startGrid.Pos.y, _startGrid.Pos.z), Mathf.Sign(_endGrid.Pos.x - _startGrid.Pos.x) > 0 ? PortDir.left : PortDir.right, PortDir.down)); }
                else 
                { 
                    PortDir dir = Mathf.Sign(_endGrid.Pos.x - _startGrid.Pos.x) > 0 ? PortDir.left : PortDir.right;
                    _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_endGrid.Pos.x, _startGrid.Pos.y, _startGrid.Pos.z), dir, dir.Opposite()));
                }


                count = (int)Mathf.Abs(_endGrid.Pos.z - _startGrid.Pos.z);
                count -= _endGrid.CanBuild() ? 0 : 1;
                for (int i = 1; i <= count; i++)
                {
                    Vector3 pos = new Vector3(_endGrid.Pos.x, _startGrid.Pos.y, _startGrid.Pos.z + i * Mathf.Sign(_endGrid.Pos.z - _startGrid.Pos.z));
                    if (Mathf.Sign(_endGrid.Pos.z - _startGrid.Pos.z) < 0)
                    {
                        GameObject previewBelt = BuildBeltUnitPreview(pos, PortDir.up, PortDir.down);
                        _previewBeltList.Add(previewBelt);
                    }
                    else
                    {
                        GameObject previewBelt = BuildBeltUnitPreview(pos, PortDir.down, PortDir.up);
                        _previewBeltList.Add(previewBelt);
                    }
                }
            }
            else
            {
                if(_endGrid.Pos.z - _startGrid.Pos.z < 0) 
                {
                    _previewBeltList.Add(BuildBeltUnitPreview(_startGrid.Pos, _curBeltInputDir, PortDir.down));
                }
                else
                { 
                    _previewBeltList.Add(BuildBeltUnitPreview(_startGrid.Pos, _curBeltInputDir, PortDir.up)); 
                }


                int count = (int)Mathf.Abs(_endGrid.Pos.z - _startGrid.Pos.z);
                for (int i = 1; i < count; i++)
                {
                    Vector3 pos = new Vector3(_startGrid.Pos.x, _startGrid.Pos.y, _startGrid.Pos.z + i * Mathf.Sign(_endGrid.Pos.z - _startGrid.Pos.z));
                    if (Mathf.Sign(_endGrid.Pos.z - _startGrid.Pos.z) < 0)
                    {
                        GameObject previewBelt = BuildBeltUnitPreview(pos, PortDir.up, PortDir.down);
                        _previewBeltList.Add(previewBelt);
                    }
                    else
                    {
                        GameObject previewBelt = BuildBeltUnitPreview(pos, PortDir.down, PortDir.up);
                        _previewBeltList.Add(previewBelt);
                    }
                }

                //拐角点---------------------------------
                if (_endGrid.Pos.x - _startGrid.Pos.x > 0) 
                { _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_startGrid.Pos.x, _startGrid.Pos.y, _endGrid.Pos.z), Mathf.Sign(_endGrid.Pos.z - _startGrid.Pos.z) < 0 ? PortDir.up : PortDir.down, PortDir.right)); }
                else if(_endGrid.Pos.x - _startGrid.Pos.x < 0)
                { _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_startGrid.Pos.x, _startGrid.Pos.y, _endGrid.Pos.z), Mathf.Sign(_endGrid.Pos.z - _startGrid.Pos.z) < 0 ? PortDir.up :PortDir.down, PortDir.left)); }
                else 
                { 
                    PortDir dir = Mathf.Sign(_endGrid.Pos.z - _startGrid.Pos.z) < 0 ? PortDir.up : PortDir.down;
                    _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_startGrid.Pos.x, _startGrid.Pos.y, _endGrid.Pos.z), dir, dir.Opposite()));
                }

                count = (int)Mathf.Abs(_endGrid.Pos.x - _startGrid.Pos.x);
                count -= _endGrid.CanBuild() ? 0 : 1;
                for (int i = 1; i <= count; i++)
                {
                    Vector3 pos = new Vector3(_startGrid.Pos.x + i * Mathf.Sign(_endGrid.Pos.x - _startGrid.Pos.x), _startGrid.Pos.y, _endGrid.Pos.z);
                    if(Mathf.Sign(_endGrid.Pos.x - _startGrid.Pos.x) > 0)
                    {
                        GameObject previewBelt = BuildBeltUnitPreview(pos, PortDir.left, PortDir.right);
                        _previewBeltList.Add(previewBelt);
                    }
                    else
                    {
                        GameObject previewBelt = BuildBeltUnitPreview(pos, PortDir.right, PortDir.left);
                        _previewBeltList.Add(previewBelt);
                    }
                }

            }
        }



        private void AddMarkPoint()
        {
            //TODO: 可修改按键映射
            if (Input.GetKeyDown(KeyCode.C) && _startGrid != _endGrid)
            {
                _startGrid = _endGrid;
                _firstIdx = _previewBeltList.Count - 1;
            }
        }

        //TODO: 预览建筑材质？
        private GameObject BuildBeltUnitPreview(Vector3 pos, PortDir inputDir, PortDir outputDir)
        {
            CheckPosBuildable(pos, ref inputDir, ref outputDir);
            pos += new Vector3(0, 0.5f, 0);
            Quaternion quat = Quaternion.identity;
            GameObject beltUnitPrefab = _beltMeshDirect;
            switch (inputDir)
            {
                //默认从下面上来
                case PortDir.right:
                    //顺时针旋转90°
                    quat = Quaternion.Euler(0, 90, 0);
                    if (outputDir == PortDir.left)
                    {
                        beltUnitPrefab = _beltMeshDirect;
                    }
                    else if (outputDir == PortDir.up)
                    {
                        beltUnitPrefab = _beltMeshTurnLeft;
                    }
                    else if(outputDir == PortDir.down)
                    {
                        beltUnitPrefab = _beltMeshTurnRight;
                    }
                        break;
                case PortDir.left:
                    quat = Quaternion.Euler(0, -90, 0);
                    if (outputDir == PortDir.right)
                    {
                        beltUnitPrefab = _beltMeshDirect;
                    }
                    else if (outputDir == PortDir.up)
                    {
                        beltUnitPrefab = _beltMeshTurnRight;
                    }
                    else if (outputDir == PortDir.down)
                    {
                        beltUnitPrefab = _beltMeshTurnLeft;
                    }
                    break;
                case PortDir.down:
                    quat = Quaternion.Euler(0, 180, 0);
                    if (outputDir == PortDir.left)
                    {
                        beltUnitPrefab = _beltMeshTurnRight;
                    }
                    else if (outputDir == PortDir.right)
                    {
                        beltUnitPrefab = _beltMeshTurnLeft;
                    }
                    else if (outputDir == PortDir.up)
                    {
                        beltUnitPrefab = _beltMeshDirect;
                    }
                    break;
                case PortDir.up:
                    if (outputDir == PortDir.left)
                    {
                        beltUnitPrefab = _beltMeshTurnLeft;
                    }
                    else if (outputDir == PortDir.right)
                    {
                        beltUnitPrefab = _beltMeshTurnRight;
                    }
                    else if (outputDir == PortDir.down)
                    {
                        beltUnitPrefab = _beltMeshDirect;
                    }
                    break;
            }
            GameObject bletUnit = GameObject.Instantiate(beltUnitPrefab, pos, quat);
            _inputDirs.Add(inputDir);
            _outputDirs.Add(outputDir);
            return bletUnit;
        }


        private void CheckPosBuildable(Vector3 pos, ref PortDir input, ref PortDir output)
        {
            Grid grid = GridManager.Instance.GetGridByPos(pos, out Vector2Int _);
            if (!grid.CanBuild())
            {
                _buildable = false;
                return;
            }
            if (grid != _endGrid)
            {
                return;
            }
            _connectTo = grid.ItemInputToBuilding;
            output = grid.ItemInputToBuildingDir.Count > 0 ? grid.ItemInputToBuildingDir[0].Item2 : output;

            //TODO: 需要抽象
        }

        //找到dir方向上的下一个格子
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



        /// <summary>
        /// 确认建造传送带
        /// 绑定传送带单元逻辑脚本，连接前后传送带单元
        /// 
        /// </summary> 
        public void ConfirmBuild()
        {
            BuildManager.Instance.ClearObjectToBuild();
            if (!_buildable)
            {
                _previewBeltList.ForEach(belt => GameObject.Destroy(belt));
                _previewBeltList.Clear();
                _inputDirs.Clear();
                _outputDirs.Clear();
                _buildable = true;
                return;
            }
            List<ConveyorBeltUnit> canveyerBeltUnits = new();
            Grid firstGrid = null;
            Grid lastGrid = null;
            for (int i = 0; i < _previewBeltList.Count; i++)
            {
                GameObject beltUnit = _previewBeltList[i];
                ConveyorBeltUnit canveyerBeltUnit = beltUnit.AddComponent<ConveyorBeltUnit>();
                canveyerBeltUnits.Add(canveyerBeltUnit);
                canveyerBeltUnit.ConveyorBeltUnitInit(_inputDirs[i], _outputDirs[i]);       //TODO: 如果有多输入/输出？
                Grid grid = GridManager.Instance.GetGridByPos(beltUnit.transform.position, out Vector2Int _);
                canveyerBeltUnit.SetOutputOnGrid(grid);
                canveyerBeltUnit.SetInputOnGrid(grid);

                grid.AddFactoryToGrid(beltUnit);
                grid.AssignBuildingToGrid(beltUnit);
                //grid.ItemOutputFromBuilding = canveyerBeltUnit;
                if (i == 0) firstGrid = grid;
                if (i == _previewBeltList.Count - 1)lastGrid = grid;
            }
            //强制连接建造的所有传送带单元（不考虑地面）
            for (int i = 0; i < canveyerBeltUnits.Count; i++)  
            {
                canveyerBeltUnits[i].Connection.SetTarget(
                    i == canveyerBeltUnits.Count - 1 ? _connectTo : canveyerBeltUnits[i + 1]
                    );
                canveyerBeltUnits[i].Connection.SetSource(
                    i == 0 ? _connectFrom : canveyerBeltUnits[i - 1]
                    );
            }
            _connectFrom?.Connection.SetTarget(canveyerBeltUnits[0]);
            _connectTo?.Connection.SetSource(canveyerBeltUnits[^1]);
        }
    }
}
