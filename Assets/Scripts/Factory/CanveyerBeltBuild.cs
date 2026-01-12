
using Factory;
using Game.UI;
using System.Collections.Generic;
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

        GameObject beltMeshDirect;
        GameObject beltMeshTurnLeft;
        GameObject beltMeshTurnRight;
        //enum BeltDir
        //{
        //    up, down, left, right
        //}
        enum PreviewDirection
        {
            Horizontal, //先水平（x轴）
            Vertical    //先垂直 (z轴)
        }

        GameObject defaltBelt;
        BeltDir curBeltInputDir = BeltDir.down;
        bool _startBuild = false;
        bool _buildable = true;
        Grid _startGrid, _endGrid;
        List<Grid> _markPointGrids = new();
        List<GameObject> _previewBeltList = new();
        List<BeltDir> _inputDirs = new();
        List<BeltDir> _outputDirs = new();
        int _firstIdx;
        PreviewDirection direction;
        IItemInput _connectTo;
        IConnectTo _connectFrom = null;


        public void Awake()
        {
            beltMeshDirect = BuildManager.instance.canveyerBelts[0];
            beltMeshTurnLeft = BuildManager.instance.canveyerBelts[1];
            beltMeshTurnRight = BuildManager.instance.canveyerBelts[2];
            defaltBelt = GameObject.Instantiate(beltMeshDirect);
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
            _startGrid = PointAt.Instance.gridHit;
            if( _startGrid.ProducerFrom != null)
            {
                _connectFrom = _startGrid.ProducerFrom;
            }
            BuildBeltUnit(_startGrid.pos, Quaternion.Euler(0, 0, 0));
            _firstIdx = 0;
        }

        public void Update()
        {
            if(!_startBuild)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    StartBuild();
                    GameObject.Destroy(defaltBelt);
                    _startBuild = true;
                }
                Grid grid = PointAt.Instance.gridHit;
                if (grid != null) 
                {
                    defaltBelt.transform.position = grid.pos;
                }
                return;
            }
            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    curBeltInputDir = (BeltDir)(((int)curBeltInputDir + 1) % 4);
            //    Debug.Log(curBeltInputDir);
            //}
            AddMarkPoint();
            RefreshBuildPreview();
            if(Input.GetMouseButtonDown(0))
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

            for (int i = _previewBeltList.Count - 1; i >= _firstIdx; i--)
            {
                GameObject.Destroy(_previewBeltList[i]);
                _inputDirs.RemoveAt(i);
                _outputDirs.RemoveAt(i);
                _previewBeltList.RemoveAt(i);
            }

            if(currGrid == _startGrid)
            {
                _previewBeltList.Add(BuildBeltUnitPreview(_startGrid.pos, curBeltInputDir, curBeltInputDir.Opposite()));
                return;
            }
            else if (currGrid.pos.x == _startGrid.pos.x)
            {
                direction = PreviewDirection.Vertical;
                if(_firstIdx == 0 && _connectFrom == null)
                {
                    curBeltInputDir = currGrid.pos.z - _startGrid.pos.z < 0 ? BeltDir.up : BeltDir.down;
                }
            }
            else if (currGrid.pos.z == _startGrid.pos.z)
            {
                direction = PreviewDirection.Horizontal;
                if (_firstIdx == 0 && _connectFrom == null)
                {
                    curBeltInputDir = currGrid.pos.x - _startGrid.pos.x < 0 ? BeltDir.right : BeltDir.left;
                }
            }

            GenerateBuildPreview();
        }


        private void GenerateBuildPreview()
        {
            _endGrid = PointAt.Instance.gridHit;
            if (_firstIdx > 0) { curBeltInputDir = _outputDirs[_firstIdx - 1].Opposite(); }
            if (direction == PreviewDirection.Horizontal) // x轴对齐
            {
                //建造上一个标记点位置的传送带
                if (_endGrid.pos.x - _startGrid.pos.x < 0) 
                {
                    
                    _previewBeltList.Add(BuildBeltUnitPreview(_startGrid.pos, curBeltInputDir, BeltDir.left)); 
                }
                else
                {
                    _previewBeltList.Add(BuildBeltUnitPreview(_startGrid.pos, curBeltInputDir, BeltDir.right)); 
                }

                int count = (int)Mathf.Abs(_endGrid.pos.x - _startGrid.pos.x);
                for (int i = 1; i < count; i++)
                    {
                        Vector3 pos = new Vector3(_startGrid.pos.x + i * Mathf.Sign(_endGrid.pos.x - _startGrid.pos.x), _startGrid.pos.y, _startGrid.pos.z);
                        if (Mathf.Sign(_endGrid.pos.x - _startGrid.pos.x) > 0)
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
                if(_endGrid.pos.z - _startGrid.pos.z > 0) 
                { _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_endGrid.pos.x, _startGrid.pos.y, _startGrid.pos.z), Mathf.Sign(_endGrid.pos.x - _startGrid.pos.x) > 0 ? BeltDir.left : BeltDir.right, BeltDir.up)); }
                else if(_endGrid.pos.z - _startGrid.pos.z < 0)
                { _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_endGrid.pos.x, _startGrid.pos.y, _startGrid.pos.z), Mathf.Sign(_endGrid.pos.x - _startGrid.pos.x) > 0 ? BeltDir.left : BeltDir.right, BeltDir.down)); }
                else 
                { 
                    BeltDir dir = Mathf.Sign(_endGrid.pos.x - _startGrid.pos.x) > 0 ? BeltDir.left : BeltDir.right;
                    _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_endGrid.pos.x, _startGrid.pos.y, _startGrid.pos.z), dir, dir.Opposite()));
                }


                count = (int)Mathf.Abs(_endGrid.pos.z - _startGrid.pos.z);
                for (int i = 1; i <= count; i++)
                {
                    Vector3 pos = new Vector3(_endGrid.pos.x, _startGrid.pos.y, _startGrid.pos.z + i * Mathf.Sign(_endGrid.pos.z - _startGrid.pos.z));
                    if (Mathf.Sign(_endGrid.pos.z - _startGrid.pos.z) < 0)
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
                if(_endGrid.pos.z - _startGrid.pos.z < 0) 
                {
                    _previewBeltList.Add(BuildBeltUnitPreview(_startGrid.pos, curBeltInputDir, BeltDir.down));
                }
                else
                { 
                    _previewBeltList.Add(BuildBeltUnitPreview(_startGrid.pos, curBeltInputDir, BeltDir.up)); 
                }


                int count = (int)Mathf.Abs(_endGrid.pos.z - _startGrid.pos.z);
                for (int i = 1; i < count; i++)
                {
                    Vector3 pos = new Vector3(_startGrid.pos.x, _startGrid.pos.y, _startGrid.pos.z + i * Mathf.Sign(_endGrid.pos.z - _startGrid.pos.z));
                    if (Mathf.Sign(_endGrid.pos.z - _startGrid.pos.z) < 0)
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
                if (_endGrid.pos.x - _startGrid.pos.x > 0) 
                { _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_startGrid.pos.x, _startGrid.pos.y, _endGrid.pos.z), Mathf.Sign(_endGrid.pos.z - _startGrid.pos.z) < 0 ? BeltDir.up : BeltDir.down, BeltDir.right)); }
                else if(_endGrid.pos.x - _startGrid.pos.x < 0)
                { _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_startGrid.pos.x, _startGrid.pos.y, _endGrid.pos.z), Mathf.Sign(_endGrid.pos.z - _startGrid.pos.z) < 0 ? BeltDir.up :BeltDir.down, BeltDir.left)); }
                else 
                { 
                    BeltDir dir = Mathf.Sign(_endGrid.pos.z - _startGrid.pos.z) < 0 ? BeltDir.up : BeltDir.down;
                    _previewBeltList.Add(BuildBeltUnitPreview(new Vector3(_startGrid.pos.x, _startGrid.pos.y, _endGrid.pos.z), dir, dir.Opposite()));
                }

                count = (int)Mathf.Abs(_endGrid.pos.x - _startGrid.pos.x);
                for (int i = 1; i <= count; i++)
                {
                    Vector3 pos = new Vector3(_startGrid.pos.x + i * Mathf.Sign(_endGrid.pos.x - _startGrid.pos.x), _startGrid.pos.y, _endGrid.pos.z);
                    if(Mathf.Sign(_endGrid.pos.x - _startGrid.pos.x) > 0)
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

        private GameObject BuildBeltUnit(Vector3 pos, Quaternion quat)
        {
            return GameObject.Instantiate(beltMeshDirect, pos, quat);
        }

        private GameObject BuildBeltUnitPreview(Vector3 pos, BeltDir inputDir, BeltDir outputDir)
        {
            CheckPosBuildable(pos);
            pos += new Vector3(0, 0.5f, 0);
            Quaternion quat = Quaternion.identity;
            GameObject beltUnitPrefab = beltMeshDirect;
            switch (inputDir)
            {
                //默认从下面上来
                case BeltDir.right:
                    //顺时针旋转90°
                    quat = Quaternion.Euler(0, 90, 0);
                    if (outputDir == BeltDir.left)
                    {
                        beltUnitPrefab = beltMeshDirect;
                    }
                    else if (outputDir == BeltDir.up)
                    {
                        beltUnitPrefab = beltMeshTurnLeft;
                    }
                    else if(outputDir == BeltDir.down)
                    {
                        beltUnitPrefab = beltMeshTurnRight;
                    }
                        break;
                case BeltDir.left:
                    quat = Quaternion.Euler(0, -90, 0);
                    if (outputDir == BeltDir.right)
                    {
                        beltUnitPrefab = beltMeshDirect;
                    }
                    else if (outputDir == BeltDir.up)
                    {
                        beltUnitPrefab = beltMeshTurnRight;
                    }
                    else if (outputDir == BeltDir.down)
                    {
                        beltUnitPrefab = beltMeshTurnLeft;
                    }
                    break;
                case BeltDir.down:
                    quat = Quaternion.Euler(0, 180, 0);
                    if (outputDir == BeltDir.left)
                    {
                        beltUnitPrefab = beltMeshTurnRight;
                    }
                    else if (outputDir == BeltDir.right)
                    {
                        beltUnitPrefab = beltMeshTurnLeft;
                    }
                    else if (outputDir == BeltDir.up)
                    {
                        beltUnitPrefab = beltMeshDirect;
                    }
                    break;
                case BeltDir.up:
                    if (outputDir == BeltDir.left)
                    {
                        beltUnitPrefab = beltMeshTurnLeft;
                    }
                    else if (outputDir == BeltDir.right)
                    {
                        beltUnitPrefab = beltMeshTurnRight;
                    }
                    else if (outputDir == BeltDir.down)
                    {
                        beltUnitPrefab = beltMeshDirect;
                    }
                    break;
            }
            GameObject bletUnit = GameObject.Instantiate(beltUnitPrefab, pos, quat);
            _inputDirs.Add(inputDir);
            _outputDirs.Add(outputDir);
            return bletUnit;
        }


        private void CheckPosBuildable(Vector3 pos)
        {
            Grid grid = GridManager.instance.GetGridXY(pos, out Vector2Int _);
            if (!grid.canBuild())
            {
                _buildable = false;
                return;
            }
        }

        /// <summary>
        /// 确认建造传送带
        /// 绑定传送带单元逻辑脚本，连接前后传送带单元
        /// 
        /// </summary> 
        public void ConfirmBuild()
        {
            BuildManager.instance.ClearObjectToBuild();
            if (!_buildable)
            {
                _previewBeltList.ForEach(belt => GameObject.Destroy(belt));
                _previewBeltList.Clear();
                _inputDirs.Clear();
                _outputDirs.Clear();
                _buildable = true;
                return;
            }
            List<CanveyerBeltUnit> canveyerBeltUnits = new List<CanveyerBeltUnit>();
            for (int i = 0; i < _previewBeltList.Count; i++)
            {
                GameObject beltUnit = _previewBeltList[i];
                CanveyerBeltUnit canveyerBeltUnit = beltUnit.AddComponent<CanveyerBeltUnit>();
                canveyerBeltUnits.Add(canveyerBeltUnit);
                canveyerBeltUnit.CanveyerBeltUnitInit(_inputDirs[i], _outputDirs[i]);
                Grid grid = GridManager.instance.GetGridXY(beltUnit.transform.position, out Vector2Int _);
                grid.AddFactoryToGrid(beltUnit);
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
                FactoryProducer producer = _connectFrom as FactoryProducer;
                producer.SetItemTarget(canveyerBeltUnits[0]);
            }
        }
    }
}
