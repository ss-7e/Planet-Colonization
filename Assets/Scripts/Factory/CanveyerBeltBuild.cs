
using Factory;
using Game.UI;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Factory
{

    /// <summary>
    /// 完成传送带建造过程逻辑
    /// 预览传送带路径（第一次点击）
    /// 确定建造逻辑（第二次点击）：添加到grid访问，连接前后传送带单元
    /// </summary>
    public class CanveyerBeltBuild 
    {
        
        GameObject _beltMeshDirect;
        GameObject _beltMeshTurnLeft;
        GameObject _beltMeshTurnRight;

        enum PreviewDirection
        {
            Horizontal, //先水平（x轴）
            Vertical    //先垂直 (z轴)
        }

        BeltDir _curBeltInputDir = BeltDir.down;
        bool _startBuild = false;
        bool _buildable = true;
        Grid _startGrid, _endGrid;
        List<Grid> _markPointGrids = new();
        List<GameObject> _previewBeltList = new();
        List<BeltDir> _inputDirs = new();
        List<BeltDir> _outputDirs = new();
        int _firstIdx;
        PreviewDirection _direction;
        IItemInput _connectTo;
        IConnectTo _connectFrom = null;

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
            if (_startGrid.ConnectableBuilding != null)
            {
                if (_startGrid.ConnectableBuilding is FactoryStorager)
                {
                    FactoryStorager producer = _startGrid.ConnectableBuilding as FactoryStorager;
                    _curBeltInputDir = producer.OutputDir.Opposite();
                    _connectFrom = producer;
                }
                else if (_startGrid.ConnectableBuilding is CanveyerBeltUnit)
                {
                    CanveyerBeltUnit beltUnit = _startGrid.ConnectableBuilding as CanveyerBeltUnit;
                    _curBeltInputDir = beltUnit.InputDir;
                    _connectFrom = beltUnit.ItemFrom;
                }
            }
            else if (_startGrid.ProducerFrom != null)
            {
                _connectFrom = _startGrid.ProducerFrom;
                _curBeltInputDir = _startGrid.ProducerFrom.OutputDir.Opposite();
            }
            else if (_startGrid.CanveyerBeltUnitFrom != null)
            {
                _connectFrom = _startGrid.CanveyerBeltUnitFrom;
                _curBeltInputDir = _startGrid.CanveyerBeltUnitFrom.OutputDir.Opposite();
            }
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
                    if(grid != null && grid.ConnectableBuilding is CanveyerBeltUnit)
                    {
                        _debuild.CanveyerBeltUnitOnGridDebuild(grid);
                    }
                }

                return;
            }
            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    curBeltInputDir = (BeltDir)(((int)curBeltInputDir + 1) % 4);
            //    Debug.Log(curBeltInputDir);
            //}
            AddMarkPoint();
            RefreshBuildPreview(); //TODO: 只在鼠标移动到新格子时调用
            if (Input.GetMouseButtonUp(0))
            {
                ConfirmBuild();
                _startBuild = false;
            }
        }


        // 当鼠标移动时刷新预览
        // TODO: 检测跨过其他传送带的情况
        private void RefreshBuildPreview()
        {
            Grid currGrid = PointAt.Instance.gridHit;
            if (currGrid == null)
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
                    _curBeltInputDir = currGrid.Pos.z - _startGrid.Pos.z < 0 ? BeltDir.up : BeltDir.down;
                }
            }
            else if (currGrid.Pos.z == _startGrid.Pos.z)
            {
                _direction = PreviewDirection.Horizontal;
                if (_firstIdx == 0 && _connectFrom == null)
                {
                    _curBeltInputDir = currGrid.Pos.x - _startGrid.Pos.x < 0 ? BeltDir.right : BeltDir.left;
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
                    _previewBeltList.Add(BuildBeltUnitPreview(_startGrid.Pos, _curBeltInputDir, BeltDir.left)); 
                }
                else
                {
                    _previewBeltList.Add(BuildBeltUnitPreview(_startGrid.Pos, _curBeltInputDir, BeltDir.right)); 
                }

                int count = (int)Mathf.Abs(_endGrid.Pos.x - _startGrid.Pos.x);
                for (int i = 1; i < count; i++)
                    {
                        Vector3 pos = new Vector3(_startGrid.Pos.x + i * Mathf.Sign(_endGrid.Pos.x - _startGrid.Pos.x), _startGrid.Pos.y, _startGrid.Pos.z);
                        if (Mathf.Sign(_endGrid.Pos.x - _startGrid.Pos.x) > 0)
                        {
                            GameObject previewBelt = BuildBeltUnitPreview(pos, BeltDir.left, BeltDir.right);
                            _previewBeltList.Add(previewBelt);
                        }
                        else
                        {
                            GameObject previewBelt = BuildBeltUnitPreview(pos, BeltDir.right, BeltDir.left);
                            _previewBeltList.Add(previewBelt);
                        }
                    }

                //拐角点---------------------------------
                if(_endGrid.Pos.z - _startGrid.Pos.z > 0) 
                { _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_endGrid.Pos.x, _startGrid.Pos.y, _startGrid.Pos.z), Mathf.Sign(_endGrid.Pos.x - _startGrid.Pos.x) > 0 ? BeltDir.left : BeltDir.right, BeltDir.up)); }
                else if(_endGrid.Pos.z - _startGrid.Pos.z < 0)
                { _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_endGrid.Pos.x, _startGrid.Pos.y, _startGrid.Pos.z), Mathf.Sign(_endGrid.Pos.x - _startGrid.Pos.x) > 0 ? BeltDir.left : BeltDir.right, BeltDir.down)); }
                else 
                { 
                    BeltDir dir = Mathf.Sign(_endGrid.Pos.x - _startGrid.Pos.x) > 0 ? BeltDir.left : BeltDir.right;
                    _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_endGrid.Pos.x, _startGrid.Pos.y, _startGrid.Pos.z), dir, dir.Opposite()));
                }


                count = (int)Mathf.Abs(_endGrid.Pos.z - _startGrid.Pos.z);
                count -= _endGrid.canBuild() ? 0 : 1;
                for (int i = 1; i <= count; i++)
                {
                    Vector3 pos = new Vector3(_endGrid.Pos.x, _startGrid.Pos.y, _startGrid.Pos.z + i * Mathf.Sign(_endGrid.Pos.z - _startGrid.Pos.z));
                    if (Mathf.Sign(_endGrid.Pos.z - _startGrid.Pos.z) < 0)
                    {
                        GameObject previewBelt = BuildBeltUnitPreview(pos, BeltDir.up, BeltDir.down);
                        _previewBeltList.Add(previewBelt);
                    }
                    else
                    {
                        GameObject previewBelt = BuildBeltUnitPreview(pos, BeltDir.down, BeltDir.up);
                        _previewBeltList.Add(previewBelt);
                    }
                }
            }
            else
            {
                if(_endGrid.Pos.z - _startGrid.Pos.z < 0) 
                {
                    _previewBeltList.Add(BuildBeltUnitPreview(_startGrid.Pos, _curBeltInputDir, BeltDir.down));
                }
                else
                { 
                    _previewBeltList.Add(BuildBeltUnitPreview(_startGrid.Pos, _curBeltInputDir, BeltDir.up)); 
                }


                int count = (int)Mathf.Abs(_endGrid.Pos.z - _startGrid.Pos.z);
                for (int i = 1; i < count; i++)
                {
                    Vector3 pos = new Vector3(_startGrid.Pos.x, _startGrid.Pos.y, _startGrid.Pos.z + i * Mathf.Sign(_endGrid.Pos.z - _startGrid.Pos.z));
                    if (Mathf.Sign(_endGrid.Pos.z - _startGrid.Pos.z) < 0)
                    {
                        GameObject previewBelt = BuildBeltUnitPreview(pos, BeltDir.up, BeltDir.down);
                        _previewBeltList.Add(previewBelt);
                    }
                    else
                    {
                        GameObject previewBelt = BuildBeltUnitPreview(pos, BeltDir.down, BeltDir.up);
                        _previewBeltList.Add(previewBelt);
                    }
                }

                //拐角点---------------------------------
                if (_endGrid.Pos.x - _startGrid.Pos.x > 0) 
                { _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_startGrid.Pos.x, _startGrid.Pos.y, _endGrid.Pos.z), Mathf.Sign(_endGrid.Pos.z - _startGrid.Pos.z) < 0 ? BeltDir.up : BeltDir.down, BeltDir.right)); }
                else if(_endGrid.Pos.x - _startGrid.Pos.x < 0)
                { _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_startGrid.Pos.x, _startGrid.Pos.y, _endGrid.Pos.z), Mathf.Sign(_endGrid.Pos.z - _startGrid.Pos.z) < 0 ? BeltDir.up :BeltDir.down, BeltDir.left)); }
                else 
                { 
                    BeltDir dir = Mathf.Sign(_endGrid.Pos.z - _startGrid.Pos.z) < 0 ? BeltDir.up : BeltDir.down;
                    _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_startGrid.Pos.x, _startGrid.Pos.y, _endGrid.Pos.z), dir, dir.Opposite()));
                }

                count = (int)Mathf.Abs(_endGrid.Pos.x - _startGrid.Pos.x);
                count -= _endGrid.canBuild() ? 0 : 1;
                for (int i = 1; i <= count; i++)
                {
                    Vector3 pos = new Vector3(_startGrid.Pos.x + i * Mathf.Sign(_endGrid.Pos.x - _startGrid.Pos.x), _startGrid.Pos.y, _endGrid.Pos.z);
                    if(Mathf.Sign(_endGrid.Pos.x - _startGrid.Pos.x) > 0)
                    {
                        GameObject previewBelt = BuildBeltUnitPreview(pos, BeltDir.left, BeltDir.right);
                        _previewBeltList.Add(previewBelt);
                    }
                    else
                    {
                        GameObject previewBelt = BuildBeltUnitPreview(pos, BeltDir.right, BeltDir.left);
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


        private GameObject BuildBeltUnitPreview(Vector3 pos, BeltDir inputDir, BeltDir outputDir)
        {
            CheckPosBuildable(pos, ref inputDir, ref outputDir);
            pos += new Vector3(0, 0.5f, 0);
            Quaternion quat = Quaternion.identity;
            GameObject beltUnitPrefab = _beltMeshDirect;
            switch (inputDir)
            {
                //默认从下面上来
                case BeltDir.right:
                    //顺时针旋转90°
                    quat = Quaternion.Euler(0, 90, 0);
                    if (outputDir == BeltDir.left)
                    {
                        beltUnitPrefab = _beltMeshDirect;
                    }
                    else if (outputDir == BeltDir.up)
                    {
                        beltUnitPrefab = _beltMeshTurnLeft;
                    }
                    else if(outputDir == BeltDir.down)
                    {
                        beltUnitPrefab = _beltMeshTurnRight;
                    }
                        break;
                case BeltDir.left:
                    quat = Quaternion.Euler(0, -90, 0);
                    if (outputDir == BeltDir.right)
                    {
                        beltUnitPrefab = _beltMeshDirect;
                    }
                    else if (outputDir == BeltDir.up)
                    {
                        beltUnitPrefab = _beltMeshTurnRight;
                    }
                    else if (outputDir == BeltDir.down)
                    {
                        beltUnitPrefab = _beltMeshTurnLeft;
                    }
                    break;
                case BeltDir.down:
                    quat = Quaternion.Euler(0, 180, 0);
                    if (outputDir == BeltDir.left)
                    {
                        beltUnitPrefab = _beltMeshTurnRight;
                    }
                    else if (outputDir == BeltDir.right)
                    {
                        beltUnitPrefab = _beltMeshTurnLeft;
                    }
                    else if (outputDir == BeltDir.up)
                    {
                        beltUnitPrefab = _beltMeshDirect;
                    }
                    break;
                case BeltDir.up:
                    if (outputDir == BeltDir.left)
                    {
                        beltUnitPrefab = _beltMeshTurnLeft;
                    }
                    else if (outputDir == BeltDir.right)
                    {
                        beltUnitPrefab = _beltMeshTurnRight;
                    }
                    else if (outputDir == BeltDir.down)
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


        private void CheckPosBuildable(Vector3 pos, ref BeltDir input, ref BeltDir output)
        {
            Grid grid = GridManager.instance.GetGridXY(pos, out Vector2Int _);
            if (!grid.canBuild() || grid.ConnectableBuilding != null)
            {
                _buildable = false;
                return;
            }
            if (grid != _endGrid)
            {
                return;
            }
            FactoryStorager producer = grid.ProducerTo;
            CanveyerBeltUnit beltUnit = grid.CanveyerBeltUnitTo;
            if (producer != null)
            {
            }
            else if(beltUnit != null)
            {
                _connectTo = beltUnit;
                output = beltUnit.InputDir.Opposite();
            }

        }


        private void GetGridByDir(Grid grid, BeltDir dir, out Grid outGrid)
        {
            GridManager manager = GridManager.instance;
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
            List<CanveyerBeltUnit> canveyerBeltUnits = new();
            Grid firstGrid = null;
            Grid lastGrid = null;
            for (int i = 0; i < _previewBeltList.Count; i++)
            {
                GameObject beltUnit = _previewBeltList[i];
                CanveyerBeltUnit canveyerBeltUnit = beltUnit.AddComponent<CanveyerBeltUnit>();
                canveyerBeltUnits.Add(canveyerBeltUnit);
                canveyerBeltUnit.CanveyerBeltUnitInit(_inputDirs[i], _outputDirs[i]);

                Grid grid = GridManager.instance.GetGridXY(beltUnit.transform.position, out Vector2Int _);
                grid.AddFactoryToGrid(beltUnit);
                grid.AssignBuildingToGrid(beltUnit);
                grid.ConnectableBuilding = canveyerBeltUnit;
                if (i == 0) firstGrid = grid;
                if (i == _previewBeltList.Count - 1)lastGrid = grid;
            }
            for (int i = 0; i < canveyerBeltUnits.Count; i++)
            {
                canveyerBeltUnits[i].SetItemDeliverTarget(
                    i == canveyerBeltUnits.Count - 1 ? _connectTo : canveyerBeltUnits[i + 1]
                    );
                canveyerBeltUnits[i].SetItemDeliverFrom(
                    i == 0 ? _connectFrom : canveyerBeltUnits[i - 1]
                    );
            }
            if (_connectFrom != null)
            {
                if (_connectFrom is FactoryStorager producer)
                    producer.SetItemTarget(canveyerBeltUnits[0]);
                else if (_connectFrom is CanveyerBeltUnit fromUnit)
                    fromUnit.SetItemDeliverTarget(canveyerBeltUnits[0]);
            }
            else
            {
                GetGridByDir(firstGrid, canveyerBeltUnits[0].InputDir, out Grid grid);
                grid.CanveyerBeltUnitTo = canveyerBeltUnits[0];

                // Debug visual effect
                //GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //debugSphere.transform.position = grid.Pos + Vector3.up;
                //debugSphere.transform.localScale = Vector3.one * 0.5f;
                //debugSphere.GetComponent<Renderer>().material.color = Color.magenta;
                //GameObject.Destroy(debugSphere, 5f);
            }
            if (_connectTo == null)
            {
                GetGridByDir(lastGrid, canveyerBeltUnits[^1].OutputDir, out Grid grid);
                grid.CanveyerBeltUnitFrom = canveyerBeltUnits[^1];
            }
            else
            {
                if (_connectTo is FactoryStorager toProducer)
                {
                    //lastGrid.ProducerTo = toProducer;
                }
                else if (_connectTo is CanveyerBeltUnit toUnit)
                {
                    toUnit.SetItemDeliverFrom(canveyerBeltUnits[^1]);
                }
            }
        }
    }
}
