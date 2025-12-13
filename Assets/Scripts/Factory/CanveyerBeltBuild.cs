
using Game.UI;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Factroy
{

    /// <summary>
    /// 完成传送带建造过程逻辑
    /// TODO: 点击设置起点和终点
    /// </summary>
    public class CanveyerBeltBuild 
    {


        GameObject beltMeshDirect;
        GameObject beltMeshTurnLeft;
        GameObject beltMeshTurnRight;
        enum BeltDir
        {
            up, down, left, right
        }
        enum PreviewDirection
        {
            Horizontal, //先水平（x轴）
            Vertical    //先垂直 (z轴)
        }

        GameObject defaltBelt;
        BeltDir curBeltInputDir = BeltDir.down;
        bool startBuild = false;
        Grid startGrid, endGrid;
        List<Grid> markPointGrids = new();
        List<GameObject> previewBeltList = new();
        int firstIdx;
        PreviewDirection direction;

        public void Awake()
        {
            //beltMeshDirect = Resources.Load<GameObject>("Factory/ConveyerBelt/Belt_Direct");
            //beltMeshTurnRight = Resources.Load<GameObject>("Factory/ConveyerBelt/Belt_Turn_Right");
            //beltMeshTurnLeft = Resources.Load<GameObject>("Factory/ConveyerBelt/Belt_Turn_Left");
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
            markPointGrids.Clear();
            previewBeltList.Clear();
            startGrid = PointAt.Instance.gridHit;
            GameObject belt = BuildBeltUnit(startGrid.pos, Quaternion.Euler(0, 0, 0));
            firstIdx = 0;
        }

        public void Update()
        {
            if(!startBuild)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    StartBuild();
                    GameObject.Destroy(defaltBelt);
                    startBuild = true;
                }
                Grid grid = PointAt.Instance.gridHit;
                if (grid != null) 
                {
                    defaltBelt.transform.position = grid.pos;
                }
                if (Input.GetKey(KeyCode.R))
                {
                    defaltBelt.transform.rotation *= Quaternion.Euler(0, 90, 0);
                    curBeltInputDir = (BeltDir)(((int)curBeltInputDir + 1) % 4);
                }
                return;
            }
            AddMarkPoint();
            RefreshBuildPreview();
            if(Input.GetMouseButtonDown(0))
            {
                ConfirmBuild();
                startBuild = false;
            }
        }


        // 当鼠标移动时刷新预览
        // TODO: 优化性能，只刷新变化的部分
        // TODO: 检测跨过其他传送带/障碍物的情况
        private void RefreshBuildPreview()
        {
            Grid currGrid = PointAt.Instance.gridHit;
            if (currGrid == null || currGrid == startGrid)
            {
                return;
            }
            if (currGrid.pos.x == startGrid.pos.x)
            {
                direction = PreviewDirection.Vertical;
            }
            else if (currGrid.pos.z == startGrid.pos.z)
            {
                direction = PreviewDirection.Horizontal;
            }

            for (int i = previewBeltList.Count - 1; i >= firstIdx; i--)
            {
                GameObject.Destroy(previewBeltList[i]);
                previewBeltList.RemoveAt(i);
            }

            GenerateBuildPreview();
        }


        private void GenerateBuildPreview()
        {
            endGrid = PointAt.Instance.gridHit;
            if (direction == PreviewDirection.Horizontal)
            {
                //建造上一个标记点位置的传送带
                if (endGrid.pos.x - startGrid.pos.x < 0) { previewBeltList.Add(BuildBeltUnit(startGrid.pos, curBeltInputDir, BeltDir.right)); }
                else { previewBeltList.Add(BuildBeltUnit(startGrid.pos, curBeltInputDir, BeltDir.left)); }

                int count = (int)Mathf.Abs(endGrid.pos.x - startGrid.pos.x);
                for (int i = 1; i < count; i++)
                    {
                        Vector3 pos = new Vector3(startGrid.pos.x + i * Mathf.Sign(endGrid.pos.x - startGrid.pos.x), startGrid.pos.y, startGrid.pos.z);
                        if (Mathf.Sign(endGrid.pos.x - startGrid.pos.x) > 0)
                        {
                            GameObject previewBelt = BuildBeltUnit(pos, BeltDir.left, BeltDir.right);
                            previewBeltList.Add(previewBelt);
                        }
                        else
                        {
                            GameObject previewBelt = BuildBeltUnit(pos, BeltDir.right, BeltDir.left);
                            previewBeltList.Add(previewBelt);
                        }
                    }


                if(endGrid.pos.z - startGrid.pos.z > 0) { previewBeltList.Add(BuildBeltUnit(new Vector3(endGrid.pos.x, startGrid.pos.y, startGrid.pos.z), Mathf.Sign(endGrid.pos.x - startGrid.pos.x) > 0 ? BeltDir.right : BeltDir.left, BeltDir.up)); }
                else { previewBeltList.Add(BuildBeltUnit(new Vector3(endGrid.pos.x, startGrid.pos.y, startGrid.pos.z), Mathf.Sign(endGrid.pos.x - startGrid.pos.x) > 0 ? BeltDir.right : BeltDir.left, BeltDir.down)); }


                count = (int)Mathf.Abs(endGrid.pos.z - startGrid.pos.z);
                for (int i = 1; i <= count; i++)
                {
                    Vector3 pos = new Vector3(endGrid.pos.x, startGrid.pos.y, startGrid.pos.z + i * Mathf.Sign(endGrid.pos.z - startGrid.pos.z));
                    if (Mathf.Sign(endGrid.pos.z - startGrid.pos.z) > 0)
                    {
                        GameObject previewBelt = BuildBeltUnit(pos, BeltDir.up, BeltDir.down);
                        previewBeltList.Add(previewBelt);
                    }
                    else
                    {
                        GameObject previewBelt = BuildBeltUnit(pos, BeltDir.down, BeltDir.up);
                        previewBeltList.Add(previewBelt);
                    }
                }
            }
            else
            {
                if(endGrid.pos.z - startGrid.pos.z < 0) { previewBeltList.Add(BuildBeltUnit(startGrid.pos, curBeltInputDir, BeltDir.up)); }
                else { previewBeltList.Add(BuildBeltUnit(startGrid.pos, curBeltInputDir, BeltDir.down)); }


                int count = (int)Mathf.Abs(endGrid.pos.z - startGrid.pos.z);
                for (int i = 1; i < count; i++)
                {
                    Vector3 pos = new Vector3(startGrid.pos.x, startGrid.pos.y, startGrid.pos.z + i * Mathf.Sign(endGrid.pos.z - startGrid.pos.z));
                    if (Mathf.Sign(endGrid.pos.z - startGrid.pos.z) > 0)
                    {
                        GameObject previewBelt = BuildBeltUnit(pos, BeltDir.up, BeltDir.down);
                        previewBeltList.Add(previewBelt);
                    }
                    else
                    {
                        GameObject previewBelt = BuildBeltUnit(pos, BeltDir.down, BeltDir.up);
                        previewBeltList.Add(previewBelt);
                    }
                }

                if(endGrid.pos.x - startGrid.pos.x > 0) { previewBeltList.Add(BuildBeltUnit(new Vector3(startGrid.pos.x, startGrid.pos.y, endGrid.pos.z), Mathf.Sign(endGrid.pos.z - startGrid.pos.z) > 0 ? BeltDir.up : BeltDir.down, BeltDir.right)); }
                else { previewBeltList.Add(BuildBeltUnit(new Vector3(startGrid.pos.x, startGrid.pos.y, endGrid.pos.z), Mathf.Sign(endGrid.pos.z - startGrid.pos.z) > 0 ? BeltDir.up :BeltDir.down, BeltDir.left)); }

                count = (int)Mathf.Abs(endGrid.pos.x - startGrid.pos.x);
                for (int i = 1; i <= count; i++)
                {
                    Vector3 pos = new Vector3(startGrid.pos.x + i * Mathf.Sign(endGrid.pos.x - startGrid.pos.x), startGrid.pos.y, endGrid.pos.z);
                    if(Mathf.Sign(endGrid.pos.x - startGrid.pos.x) > 0)
                    {
                        GameObject previewBelt = BuildBeltUnit(pos, BeltDir.left, BeltDir.right);
                        previewBeltList.Add(previewBelt);
                    }
                    else
                    {
                        GameObject previewBelt = BuildBeltUnit(pos, BeltDir.right, BeltDir.left);
                        previewBeltList.Add(previewBelt);
                    }
                }

            }
        }



        private void AddMarkPoint()
        {
            //TODO: 可修改按键映射
            //TODO: 似乎有越界bug，快速移动鼠标会出现空位
            if (Input.GetKeyDown(KeyCode.C))
            {
                startGrid = endGrid;
                firstIdx = previewBeltList.Count - 1;
            }
        }

        private GameObject BuildBeltUnit(Vector3 pos, Quaternion quat)
        {
            return GameObject.Instantiate(beltMeshDirect, pos, quat);
        }

        private GameObject BuildBeltUnit(Vector3 pos, BeltDir inputDir, BeltDir outputDir)
        {
            // TODO: 拐角方向需要调整
            pos += new Vector3(0, 0.5f, 0);
            Quaternion quat = Quaternion.identity;
            GameObject beltUnitPrefab = beltMeshDirect;
            switch (inputDir)
            {
                //默认从下面上来
                case BeltDir.left:
                    //顺时针旋转90°
                    quat = Quaternion.Euler(0, 90, 0);
                    if (outputDir == BeltDir.right)
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
                case BeltDir.right:
                    quat = Quaternion.Euler(0, -90, 0);
                    if (outputDir == BeltDir.left)
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
                case BeltDir.up:
                    quat = Quaternion.Euler(0, 180, 0);
                    if (outputDir == BeltDir.left)
                    {
                        beltUnitPrefab = beltMeshTurnRight;
                    }
                    else if (outputDir == BeltDir.right)
                    {
                        beltUnitPrefab = beltMeshTurnLeft;
                    }
                    else if (outputDir == BeltDir.down)
                    {
                        beltUnitPrefab = beltMeshDirect;
                    }
                    break;
                case BeltDir.down:
                    if (outputDir == BeltDir.left)
                    {
                        beltUnitPrefab = beltMeshTurnLeft;
                    }
                    else if (outputDir == BeltDir.right)
                    {
                        beltUnitPrefab = beltMeshTurnRight;
                    }
                    else if (outputDir == BeltDir.up)
                    {
                        beltUnitPrefab = beltMeshDirect;
                    }
                    break;
            }
            return GameObject.Instantiate(beltUnitPrefab, pos, quat);
        }

        /// <summary>
        /// 确认建造传送带
        /// </summary> 
        public void ConfirmBuild()
        {
            BuildManager.instance.ClearObjectToBuild();
        }
    }
}
