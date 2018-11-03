using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GridShapeType
{
    /// <summary>
    /// 正方形
    /// </summary>
    Square,
    /// <summary>
    /// 正六边形
    /// </summary>
    RegularHexagon,
}

public class MapEditorManager : MonoBehaviour
{

    public static MapEditorManager mIns;

    /// <summary>
    /// 地图宽
    /// </summary>
    public float mapWidth = 0;

    /// <summary>
    /// 地图长
    /// </summary>
    public float mapHeight = 0;

    /// <summary>
    /// 网格线是否显示
    /// </summary>
    public bool showGrid = true;

    /// <summary>
    /// 网格线宽度
    /// </summary>
    public float lineWidth = 0.1f;

    /// <summary>
    /// 网格线长度
    /// </summary>
    public float lineLength = 10f;

    /// <summary>
    /// 网格线颜色
    /// </summary>
    public Color lineColor = Color.red;

    /// <summary>
    /// 网格线的精度(一个边分为几段)
    /// </summary>
    public int linePix = 4;

    /// <summary>
    /// 网片精度（一个网片分为多少个子网片）
    /// </summary>
    public int meshPix = 2;

    /// <summary>
    /// 当前高度判断，如果大于这个高度就是限制行走渔区
    /// </summary>
    public int reachHeight = 30;

    /// <summary>
    /// 地图数据输出路径
    /// </summary>
    public string exportPath;

    /// <summary>
    /// 当前地图地形
    /// </summary>
    public Terrain m_terrian;

    /// <summary>
    /// 当前地图vector3数据
    /// </summary>
    private Vector3[,] m_array;
    public Vector3[,] MArray
    {
        get
        {
            return m_array;
        }
    }
    /// <summary>
    /// m_array行数
    /// </summary>
    private int m_arrayRow;
    public int ArrayRow
    {
        get
        {
            return m_arrayRow;
        }
    }
    /// <summary>
    /// m_array列数
    /// </summary>
    private int m_arrayCol;
    public int ArrayCol
    {
        get
        {
            return m_arrayCol;
        }
    }

    /// <summary>
    /// 存储的网格线
    /// </summary>
    private GameObject[,] m_lines;

    /// <summary>
    /// 存储的网格线
    /// </summary>
    private Dictionary<string, GameObject> m_meshs = new Dictionary<string, GameObject>();

    /// <summary>
    /// 存储的地图信息
    /// </summary>
    private GridInfo[,] m_gridInfos;

    /// <summary>
    /// 当前地图块类型
    /// </summary>
    private int currentType = 0;
    private GameObject MeshParent;

    private void Awake()
    {
        mIns = this;
        MeshParent = new GameObject("MeshParents");
        exportPath = $"{ Application.dataPath}/MapDataExport/{ SceneManager.GetActiveScene().name}.txt";
    }

    private void Start()
    {
        LoadMap();
        GetConnectedState();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                DrawMesh(hit.point, currentType);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Export();
        }
    }
    #region 外部函数
    private const int INTERNET_CONNECTION_MODEM = 1;

    private const int INTERNET_CONNECTION_LAN = 2;

    private const int INTERNET_CONNECTION_PROXY = 4;

    private const int INTERNET_CONNECTION_MODEM_BUSY = 8;

    [DllImport("winInet.dll ")]

    //声明外部的函数： 

    private static extern bool InternetGetConnectedState(
        ref int Flag,
        int dwReserved
    );
    private void GetConnectedState()
    {
        int Flag = 0;

        string netStates = "";

        if (!InternetGetConnectedState(ref Flag, 0))
        {
            Debug.Log("no！");
        }
        else
        {

            if ((Flag & INTERNET_CONNECTION_MODEM) != 0)

                netStates += " Connect by MODEM /n";

            if ((Flag & INTERNET_CONNECTION_LAN) != 0)

                netStates += "Connect by LAN  /n";

            if ((Flag & INTERNET_CONNECTION_PROXY) != 0)

                netStates += "Connect by PROXY /n";

            if ((Flag & INTERNET_CONNECTION_MODEM_BUSY) != 0)

                netStates += " MODEM is busy  /n";
        }

        Debug.Log(netStates);
    }
    #endregion

    private void LoadMap()
    {
        if (m_terrian == null)
        {
            Debug.LogError("Terrian is null!");
            return;
        }
        if (this.linePix < 1)
        {
            Debug.LogError("linePix must be bigger than 1!");
            return;
        }

        TerrainData data = m_terrian.terrainData;

        mapWidth = data.size.x;
        mapHeight = data.size.z;

        m_arrayRow = (int)(data.size.x / lineLength) * linePix + 1;
        m_arrayCol = (int)(data.size.z / lineLength) * linePix + 1;

        m_array = new Vector3[m_arrayRow, m_arrayCol];

        for (int i = 0; i < m_arrayRow; i++)
        {
            for (int j = 0; j < m_arrayCol; j++)
            {
                float x = lineLength / linePix * j;
                float z = lineLength / linePix * i;
                m_array[i, j] = new Vector3(x, m_terrian.SampleHeight(new Vector3(x, 0, z)), z);
            }
        }

        m_terrian.GetComponent<Terrain>().enabled = false;

        ShowSquareGird();
    }

    private void ShowSquareGird()
    {
        Vector3[] pos;

        if (LoadMapData())
        {
            int rn = (m_arrayRow - 1) / linePix;
            int cn = (m_arrayCol - 1) / linePix;

            m_lines = new GameObject[rn, cn];

            for (int i = 0; i < rn; i++)
            {
                for (int j = 0; j < cn; j++)
                {
                    pos = new Vector3[linePix * 4 + 1];
                    for (int k = 0; k < linePix; k++)
                    {
                        pos[0 * linePix + k] = m_array[i * linePix, j * linePix + k];
                        pos[1 * linePix + k] = m_array[i * linePix + k, (j + 1) * linePix];
                        pos[2 * linePix + k] = m_array[(i + 1) * linePix, (j + 1) * linePix - k];
                        pos[3 * linePix + k] = m_array[(i + 1) * linePix - k, j * linePix];
                    }
                    pos[linePix * 4] = pos[0];
                    CreateLine(i, j, pos);

                    DrawMesh(m_gridInfos[i, j].centerPos, m_gridInfos[i, j].gridType);
                }
            }
        }
        else
        {
            int rn = (m_arrayRow - 1) / linePix;
            int cn = (m_arrayCol - 1) / linePix;
            m_lines = new GameObject[rn, cn];
            m_gridInfos = new GridInfo[rn, cn];

            for (int i = 0; i < rn; i++)
            {
                for (int j = 0; j < cn; j++)
                {
                    pos = new Vector3[linePix * 4 + 1];
                    for (int k = 0; k < linePix; k++)
                    {
                        pos[0 * linePix + k] = m_array[i * linePix, j * linePix + k];
                        pos[1 * linePix + k] = m_array[i * linePix + k, (j + 1) * linePix];
                        pos[2 * linePix + k] = m_array[(i + 1) * linePix, (j + 1) * linePix - k];
                        pos[3 * linePix + k] = m_array[(i + 1) * linePix - k, j * linePix];
                    }
                    pos[linePix * 4] = pos[0];
                    CreateLine(i, j, pos);
                    Vector3 center = m_array[i * linePix + Mathf.CeilToInt(linePix / 2), j * linePix + Mathf.CeilToInt(linePix / 2)];
                    DrawMesh(center, center.y > reachHeight ? 1 : 0);
                }
            }
        }

    }
    private void CreateLine(int row, int col, Vector3[] pos)
    {
        if (this.m_lines[row, col] != null)
        {
            GameObject.Destroy(m_lines[row, col]);
        }
        this.m_lines[row, col] = new GameObject();

        LineRenderer _lineRenderer = this.m_lines[row, col].AddComponent<LineRenderer>();
        _lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        _lineRenderer.startColor = lineColor;
        _lineRenderer.endColor = lineColor;
        _lineRenderer.startWidth = lineWidth;
        _lineRenderer.endWidth = lineWidth;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.positionCount = pos.Length;
        for (int i = 0; i < pos.Length; ++i)
        {
            _lineRenderer.SetPosition(i, pos[i]);
        }

        m_lines[row, col].name = "Line " + row + "_" + col;
    }
    /// <summary>
    /// 渲染网格
    /// </summary>
    /// <param name="pos"></param>
    private void DrawMesh(Vector3 pos, int gridType)
    {
        int row = Mathf.FloorToInt(pos.z / lineLength);
        int col = Mathf.FloorToInt(pos.x / lineLength);

        string key = row + "_" + col;

        if (m_meshs.ContainsKey(key))
        {
            m_meshs[key].SetActive(true);
            m_gridInfos[row, col].gridType = gridType;
            ShowGridColor(m_meshs[key], gridType);
        }
        else
        {
            Material material = new Material(Shader.Find("Standard"));
            GameObject m_mesh = new GameObject("m_mesh");
            m_mesh.AddComponent<MeshFilter>();

            MapGrid grid = new MapGrid(linePix, pos);
            Mesh mesh = grid.CreateMesh();
            //存格子数据
            GridInfo info = new GridInfo(gridType, pos);
            m_gridInfos[row, col] = info;
            //存储网片数据
            m_meshs.Add(key, m_mesh);
            //将绘制好的Mesh赋值
            m_mesh.GetComponent<MeshFilter>().sharedMesh = mesh;
            //设置自发光
            material.EnableKeyword("_EMISSION");
            m_mesh.AddComponent<MeshRenderer>().sharedMaterial = material;

            ShowGridColor(m_mesh, gridType);
        }
    }
    /// <summary>
    /// 显示网格颜色
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="gridType"></param>
    private void ShowGridColor(GameObject obj, int gridType)
    {
        if (!obj.activeSelf) obj.SetActive(true);
        switch (gridType)
        {
            case 0:
                obj.GetComponent<Renderer>().material.SetColor("_EmissionColor", ColorTool.Walkable_Color);
                break;
            case 1:
                obj.GetComponent<Renderer>().material.SetColor("_EmissionColor", ColorTool.Walkable_Non_Color);
                break;
            case 2:
                obj.GetComponent<Renderer>().material.SetColor("_EmissionColor", ColorTool.Player_Birth);
                break;
            case 3:
                obj.GetComponent<Renderer>().material.SetColor("_EmissionColor", ColorTool.Player_Samsara);
                break;
            case 4:
                obj.GetComponent<Renderer>().material.SetColor("_EmissionColor", ColorTool.NPC_Birth);
                break;
        }
    }
    #region UI按钮事件
    public void Export()
    {
        try
        {

            if (File.Exists(exportPath)) File.Delete(exportPath);

            int rn = (m_arrayRow - 1) / linePix;
            int cn = (m_arrayCol - 1) / linePix;

            FileStream fs = new FileStream(exportPath, FileMode.Create);
            fs.Seek(0, SeekOrigin.Current);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(rn);
            bw.Write(cn);
            Debug.Log(rn + ":" + cn);
            for (int i = 0; i < rn; i++)
            {
                for (int j = 0; j < cn; j++)
                {
                    // gridType 0-4  centerPos=Vector3
                    bw.Write((byte)m_gridInfos[i, j].gridType);
                    bw.Write((int)m_gridInfos[i, j].centerPos.x * 100);
                    bw.Write((int)m_gridInfos[i, j].centerPos.y * 100);
                    bw.Write((int)m_gridInfos[i, j].centerPos.z * 100);
                }
            }
            fs.Flush();
            fs.Close();
            fs.Dispose();
            fs = null;

            Debug.Log("导出地形数据成功");
        }
        catch (Exception e)
        {
            Debug.Log("导出地形数据成功");
            Debug.LogError(e);
        }
    }
    public bool LoadMapData()
    {
        if (File.Exists(exportPath))
        {
            try
            {
                FileStream fs = new FileStream(exportPath, FileMode.OpenOrCreate);
                BinaryReader br = new BinaryReader(fs);
                int row_count = br.ReadInt32();
                int col_count = br.ReadInt32();
                Debug.Log(row_count + ":" + col_count);
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
    public void OnWalkable(bool value)
    {
        if (value) ChangeCurrentGridType(0);
    }
    public void OnWalkableNone(bool value)
    {
        if (value) ChangeCurrentGridType(1);
    }
    public void OnPlayerBirth(bool value)
    {
        if (value) ChangeCurrentGridType(2);
    }
    public void OnPlayerSamsara(bool value)
    {
        if (value) ChangeCurrentGridType(3);
    }
    public void OnNPCBirth(bool value)
    {
        if (value) ChangeCurrentGridType(4);
    }
    private void ChangeCurrentGridType(int gridType)
    {
        currentType = gridType;
        currentType = Mathf.Clamp(currentType, 0, 4);
    }
    #endregion
}
