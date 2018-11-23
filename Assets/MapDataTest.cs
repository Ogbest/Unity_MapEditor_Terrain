using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapDataTest : MonoBehaviour
{
    public GameObject go;

    private string exportPath;
    private Vector3 offset;
    private GridInfo[,] m_gridInfos;
    private GridInfo originPoint;
    private int row_count;
    private int col_count;
    void Start()
    {
        exportPath = $"{ Application.dataPath}/MapDataExport/{ SceneManager.GetActiveScene().name}.txt";
        LoadMapData();
        originPoint = m_gridInfos[0, 0];
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vertify(go.transform.position);
        }
    }
    /// <summary>
    /// 验证在什么区域内
    /// </summary>
    /// <param name="position"></param>
    public int Vertify(Vector3 position)
    {
        position -= (originPoint.centerPos + offset);
        //计算物体对应地形数据的行列
        int row = Mathf.Abs((int)(position.x));
        int col = Mathf.Abs((int)(position.z - 0.5f));
        Debug.Log(row + ":" + col + ":" + position);

        return m_gridInfos[row, col].gridType;
    }
    public bool LoadMapData()
    {
        if (File.Exists(exportPath))
        {
            try
            {
                FileStream fs = new FileStream(exportPath, FileMode.OpenOrCreate);
                BinaryReader br = new BinaryReader(fs);
                //读取行列
                row_count = br.ReadInt32();
                col_count = br.ReadInt32();
                //读取中心点做插值
                if (br.ReadByte() != 10) Debug.LogError("读取地图错误");
                offset.x = br.ReadInt32();
                offset.y = br.ReadInt32();
                offset.z = br.ReadInt32();
                m_gridInfos = new GridInfo[row_count, col_count];
                for (int r = 0; r < row_count; r++)
                {
                    for (int c = 0; c < col_count; c++)
                    {
                        m_gridInfos[r, c] = new GridInfo();
                        m_gridInfos[r, c].gridType = br.ReadByte();
                        m_gridInfos[r, c].centerPos = new Vector3();
                        m_gridInfos[r, c].centerPos.x = (float)br.ReadInt32() / (float)100;
                        m_gridInfos[r, c].centerPos.y = (float)br.ReadInt32() / (float)100;
                        m_gridInfos[r, c].centerPos.z = (float)br.ReadInt32() / (float)100;
                    }
                }
                fs.Dispose();
                fs.Close();
                fs = null;
                Debug.Log("加载地形数据成功");

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("加载地形数据失败" + e);
                return false;
            }
        }
        return false;
    }
}
