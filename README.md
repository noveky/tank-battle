# TankBattle

#### 介绍

Unity作业，制作一个坦克大战游戏

#### 游戏模式

1.  单人闯关模式：单个玩家操控玩家1的坦克，全歼敌方坦克则关卡通过并进入下一关，直至通关；关卡通过前基地被破坏则游戏结束；玩家阵亡后一段时间在出生点重生。
2.  多人闯关模式：玩家1和玩家2共同游戏，敌方坦克生命值翻倍，其余与单人闯关模式相同。
3.  双人对战模式：玩家1和玩家2对战，游戏在一个对称的地图下进行，率先拆毁另一方基地的玩家获胜；玩家阵亡后一段时间在出生点重生。

#### 游玩方法

1.  玩家1使用WASD来移动，空格键或鼠标左键来射击。
2.  玩家2使用方向键来移动，空格键或鼠标右键来射击。
3.  坦克被敌对方炮弹打中时，生命值会减少，直至生命值为零时该坦克被击杀。
4.  草丛允许坦克和炮弹穿过，躲在草丛中的敌方坦克会隐藏血条。
5.  炮弹可破坏除金属围墙外的任何场景中物体，并且可以同时击中相邻的两块。

#### 敌方坦克简介

1.  灰色坦克：平庸的坦克，生命值少，炮弹伤害低，移动速度及进攻能力中等。
2.  绿色坦克：重型坦克，生命值较多，炮弹伤害较高、减速作用较明显，但炮弹初速慢，射击间隔长，擅长拆墙。
3.  红色坦克：道具坦克，玩家将其击杀时立即获得生命值回复道具并回复一定生命值，生命值、移动速度等属性均属中等。
4.  蓝色坦克：稀有但危险的特种坦克，发射极高伤害的高速炮弹，移动非常迅速，擅长迅速推进并拆毁玩家基地。
