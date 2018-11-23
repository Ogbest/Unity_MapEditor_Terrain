# Unity_MapEditor_Terrain
动态渲染Unity地形网格，记录网格是否是玩家的行走范围，然后导出编辑好的网格用于服务器导航、验证使用

1:点击右上角按钮导出当前地形的编辑数据

2:MapDataTest脚本里面Vertify方法验证当前物体所在的位置对应网格的什么类型(验证是否是可行走区域等)

3:原点以地形左上角为原点，提供了对应的转换API

4:在编辑的时候会把地形移动到原点，记录之前地形的位置，在验证的时候做插值转换

5:地形的位置必须是能整除2的整数


![](https://github.com/Ogbest/Unity_MapEditor_Terrain/blob/master/texure.png)
