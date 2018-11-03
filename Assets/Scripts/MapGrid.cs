using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid{
   
    /// <summary>
    /// 顶点数
    /// </summary>
    private Vector3[] _vertexes;

    /// <summary>
    /// 三角形索引  
    /// </summary>
    private int[] _triangles;

    /// <summary>
    /// mesh 长度的段数和宽度的段数 
    /// </summary>
    private Vector2 _segment;

    /// <summary>
    /// 一个网格的基数
    /// </summary>
    private int _coefficient;

    /// <summary>
    /// 行数
    /// </summary>
    private int _row;

    /// <summary>
    /// 列数
    /// </summary>
    private int _col;

    /// <summary>
    /// 面片
    /// </summary>
    private Mesh _mesh;


    public MapGrid(int linePix, Vector3 pos)
    {
        this._coefficient = linePix + 1;
        this._segment = new Vector2(linePix, linePix);
        this._row = Mathf.FloorToInt(pos.z / MapEditorManager.mIns.lineLength);
        this._col = Mathf.FloorToInt(pos.x / MapEditorManager.mIns.lineLength);
    }

    public Mesh CreateMesh()
    {
        CaculateVertexes();
        CaculateTriangles();

        if (_vertexes == null || _triangles == null)
        {
            _mesh = null;
            return null;
        }

        if (_mesh == null)
        {
            _mesh = new Mesh();
        }
        _mesh.vertices = _vertexes;
        _mesh.triangles = _triangles;
        return _mesh;
    }

    private void CaculateVertexes()
    {
        KeyValuePair<int, int> info = new KeyValuePair<int, int>(_row, _col);

        if ((info.Key + this._coefficient - 1 >= MapEditorManager.mIns.ArrayRow) || (info.Value + this._coefficient - 1 >= MapEditorManager.mIns.ArrayCol))
        {
            this._vertexes = null;
            return;
        }

        int index = 0;
        this._vertexes = new Vector3[this._coefficient * this._coefficient];

        for (int i = 0; i < this._coefficient; ++i)
        {
            for (int j = 0; j < this._coefficient; ++j)
            {
                this._vertexes[index++] = MapEditorManager.mIns.MArray[info.Key * (this._coefficient - 1) + i, info.Value * (this._coefficient - 1) + j] - new Vector3(0, 0.1f, 0);
            }
        }
    }

    private void CaculateTriangles()
    {
        int sum = Mathf.FloorToInt(this._segment.x * this._segment.y * 6);
        this._triangles = new int[sum];

        uint index = 0;
        for (int i = 0; i < this._segment.y; i++)
        {
            for (int j = 0; j < this._segment.x; j++)
            {
                int role = Mathf.FloorToInt(this._segment.x) + 1;
                int self = j + (i * role);
                int next = j + ((i + 1) * role);
                //顺时针  
                this._triangles[index] = self;
                this._triangles[index + 1] = next + 1;
                this._triangles[index + 2] = self + 1;
                this._triangles[index + 3] = self;
                this._triangles[index + 4] = next;
                this._triangles[index + 5] = next + 1;
                index += 6;
            }
        }
    }
}
