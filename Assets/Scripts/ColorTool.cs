using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTool
{
    /// <summary>
    /// 可行走区域颜色
    /// </summary>
    public static Color Walkable_Color = new Color(0.363484f, 0.5955882f, 0.363484f, 1);
    /// <summary>
    /// 不可行走区域
    /// </summary>
    public static Color Walkable_Non_Color = new Color(0.5f, 0.2655172f, 0.25f, 1);
    /// <summary>
    /// 玩家出生点
    /// </summary>
    public static Color Player_Birth = new Color(1f, 0.7929713f, 0f, 1);
    /// <summary>
    /// 玩家玩家复活
    /// </summary>
    public static Color Player_Samsara = new Color(0.2239403f, 0.7297772f, 0.8014706f, 1);
    /// <summary>
    /// NPC出生点
    /// </summary>
    public static Color NPC_Birth = new Color(0.6859191f, 0.2394572f, 0.7573529f, 1);
}
/// <summary>
/// 格子类型
/// </summary>
//public enum GridType
//{
//    /// <summary>
//    /// 可行走区域
//    /// </summary>
//    Walkable=0,
//    /// <summary>
//    /// 阻隔区域
//    /// </summary>
//    Walkble_Non = 1,
//    /// <summary>
//    /// 出生点
//    /// </summary>
//    Birth=2,
//    /// <summary>
//    /// 复活点
//    /// </summary>
//    Samsara=3,
//    /// <summary>
//    /// NPC出生点
//    /// </summary>
//    NPC_Birth = 4,
//}
