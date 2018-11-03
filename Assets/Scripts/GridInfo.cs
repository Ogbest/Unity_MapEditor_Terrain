using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInfo
{
    //0-可行走  1-禁止行走  2-玩家出生点  3-玩家重生点  4-NPC出生点
    public int gridType;
    public Vector3 centerPos;

    public GridInfo(int type, Vector3 pos)
    {
        this.gridType = type;
        this.centerPos = pos;
    }
    public GridInfo()
    {
        this.gridType = 0;
        this.centerPos =Vector3.zero;
    }
}
