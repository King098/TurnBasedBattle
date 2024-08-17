# TurnBasedBattle
回合制战斗逻辑

# 版本更新 2024/8/17
### 1.升级到Unity2022 LTS版本兼容
### 2.增加Addressables系统索引加载资源
### 3.增加阵营概念，同阵营为友方单位，不同阵营各自为敌的玩法。
### 4.优化调整技能目标的设计，原先的设计将技能目标数量也加入进去了，不好扩展和使用。现在优化为自己队伍的目标，同阵营的目标（包含自己队伍），敌方阵营目标（所有不和自己同阵营的所有阵营目标），技能的目标数量改为在编辑器设计技能的时候配置。
### 5.优化了友方和敌方的概念。现在同阵营都为友方，其他阵营都为敌方，敌方可能存在多个阵营，他们也可能互为敌方。比如：现在demo中的四个队伍，第一个是自己的队伍，第四个是友方队伍，他们为同阵营的单位，而第二个和第三个队伍分别为两个不同阵营，则他们互为敌对势力同时也是玩家的敌对势力。
### 6.调整场上队伍数量，最多可以同时存在四个队伍，可以设置每个队伍在场上第几个位置，索引格子分别从0-3对应左上，右上，右下，左下四个队伍所在区域。
### 7.增加玩家控制英雄的技能释放为可控目标选择，其他队伍为自动释放技能，自动追踪目标对象。
### 8.优化了战斗场景的设计，原先都在UI上实现的效果。现在改为战斗场景和UI分离，战斗场景为一个单独的设计，由StageCamera渲染，操作UI使用UI部分单独渲染。方便后续的场景更换等功能的实现。
### 9.增加了HeroTeam（数据对象），HeroTeamMono（运行的实例对象）类的实现，HeroTeam是一个队伍的概念，现在的开始战斗的接口增加重载方法如下，每个HeroTeam包含上阵的英雄Hero数据，TeamIndex上阵在场景第几个格子，TeamGroup所属阵营名字，TeamType由自己控制的队伍还是NPC控制的队伍。HeroTeamMono则是包含生成的HeroMono对象和其他的参数缓存，也就是运行时的实例对象。
```C Sharp
public async void StartBattle(List<HeroTeam> teams)
{
    ......
}
```
### 10.引入了TimerManager,EventManager重构了部分逻辑的实现代码，采用了广播的方式进行各个模块之间的调用。
### 11.HeroMono可以继承实现自己的自定义英雄表现效果了。下面是HeroMono基类实现的默认表现效果，可以继承之后实现自定义在各个实际的英雄表现效果。
```C Sharp
/// <summary>
        /// 当这个英雄被选择的时候
        /// </summary>
        protected virtual void OnHeroChoose()
        {
            this.SpriteRender.color = Color.red;
        }

        /// <summary>
        /// 当这个英雄可以被选择的时候
        /// </summary>
        protected virtual void OnHeroCanChoose()
        {
            this.SpriteRender.color = Color.green;
        }

        /// <summary>
        /// 当这个英雄设置为不可选择的状态的时候
        /// </summary>
        protected virtual void OnHeroCannotChoose()
        {
            this.SpriteRender.color = Color.white;
        }

        /// <summary>
        /// 当这个英雄是当前执行的英雄的时候
        /// </summary>
        protected virtual void OnHeroActioning()
        {

        }

        /// <summary>
        /// 当这个英雄被攻击的时候
        /// </summary>
        protected virtual void OnHeroAttacked(HeroMono attacker,BaseSkill skill)
        {
            //防御的时候是判断技能属于物理还是防御，然后叠加，然后检测是否有技能对此攻击技能有影响，还有buff的影响
            SkillController.Instance.DefenseSkill(attacker, this, skill);
            if(skill.SkillType == Global.SkillType.Respwan)
            {
                this.SpriteRender.enabled = true;
            }
            else
            {
                //添加被击高亮
                this.SpriteRender.color = Color.red;
                this.regScheduleOnce((t,p)=>{
                    this.SpriteRender.color = Color.white;
                },0.2f);
            }
        }

        /// <summary>
        /// 当这个英雄添加了新的buff的时候
        /// </summary>
        protected virtual void OnHeroAddBuff(Buff buff)
        {
            this.SpriteRender.color = buff.IsBuff ? Color.green : Color.gray;
            this.regScheduleOnce((t,p)=>{
                this.SpriteRender.color = Color.white;
            },0.2f);
        }

        /// <summary>
        /// 当这个英雄移除buff的时候
        /// </summary>
        protected virtual void OnHeroRemoveBuff(Buff buff)
        {
            this.SpriteRender.color = buff.IsBuff ? Color.yellow : Color.gray;
            this.regScheduleOnce((t,p)=>{
                this.SpriteRender.color = Color.white;
            },0.2f);
        }

        /// <summary>
        /// 当英雄的buff执行效果的时候
        /// </summary>
        /// <param name="buff"></param>
        protected virtual void OnHeroBuffExcute(Buff buff)
        {
            this.SpriteRender.color = buff.IsBuff ? Color.green : Color.red;
            this.regScheduleOnce((t,p)=>{
                this.SpriteRender.color = Color.white;
            },0.2f);
        }

        /// <summary>
        /// 当这个英雄死亡的时候
        /// </summary>
        protected virtual void OnHeroDead()
        {
            this.SpriteRender.enabled = false;
        }
```

# demo的表现效果以现在工程中的demo为准，现在的demo和早期demo的实现和表现效果已经完全不同了。


回合制战斗系统功能说明和一些注意事项

# 1.基础介绍
###  a.目前系统中对于英雄的设定有：最大生命值，当前生命值，最大魔力值，当前魔力值，当前物理攻击力，当前物理防御力，当前魔法攻击力，当前魔法防御力，速度，回合等概念
###  b.目前系统中使用的参数设置有两种常见的设定方式，一种是设定具体的值，一种是设定百分比（非具体，会根据实际运行中的参数进行计算）
###  c.目前系统中buff，debuff实际上是一种对象，只是对于执行数值做了正负值区分
###  d.目前系统中主动技能，被动技能实际上是一种对象。主动技能实际上只是在回合执行中的阶段使用的主动使用的技能，被动技能则是在任意回合阶段中自动执行的技能
###  e.目前系统中支持的队伍列表上限为6，超过将会丢弃数据。
###  f.目前系统技能能设定目标对象是什么，从自身，自身队友，到敌人一个目标到多个目标。

# 2.有什么功能
###  a.核心的玩法不需要重新构建，支持扩展新技能（需要进行基类继承，目前有基础技能，基础单人目标物理技能，基础单人目标魔法技能，基础多人目标物理技能，基础多人目标魔法技能。这些类型的技能支持被动技能设置）
###  b.回合制游戏中可有可无的技能蓄力，技能冷却回合的设定，现有设定在设定每次战斗回合总数设定上限的情况下，能实现一次战斗中永久只能发动一次的技能冷却设定，目前不支持真正的战斗中限定一次的技能设置。技能支持暴击效果，支持MISS
###  c.buff,debuff的快速扩展，不需要代码，直接设置参数即可实现不同的效果，支持对英雄的各种属性做出改变，甚至是对方的回合也能改变，以此实现跳过回合或者将回合交给特定角色的功能实现
###  d.目前为了增加同样数值情况下战斗的不可预测性，和高可玩性增加了“乱敏”设定，可在Global脚本中进行设置调节，默认是英雄在开局时候将会在自身速度的70%-100%之间随机一个速度值作为当前战斗使用的速度。如果速度相同，遵循以下原则：敌人的英雄为第一回合，自己队伍中按照排队顺序进行战斗。乱敏的好处：减少同数值战斗结果单一现象，让战斗更加不可预料。如果要关闭乱敏功能，可以将Global中HeroSpeedMix设定为1
###  e.目前有编辑器可以支持英雄，技能，buff这些的界面化配置，除了ID之外，其他参数基本都为可自定义参数。
###  f.BattleController支持DebugMode
###  g.BattleController支持自动战斗
###  h.使用接口非常简单，只有一个BattleController.Instance.StartBattle方法，两个参数，第一个是自己队伍的英雄数据集合，第二个敌人的队伍的英雄数据集合，具体使用参考TestCombat脚本
###  i.支持自定义界面，自定义用户操作等，BaseBattleHeroHub，BaseBattleHeroUI，BaseBattleUI，BaseInputController，这些类都支持继承来扩展。

# 3.编辑器功能介绍
###  a.英雄编辑器：![编辑器界面英雄的编辑效果图](https://github.com/King098/TurnBasedBattle/blob/master/CapturePhoto/编辑器_英雄.png)
目前有一些测试用参数在项目中
###  b.技能编辑器![技能编辑器界面](https://github.com/King098/TurnBasedBattle/blob/master/CapturePhoto/编辑器_技能.png)
技能设置完毕之后，可以在英雄编辑界面对指定英雄使用技能修改器，添加修改英雄的技能
###  c.buff,debuff编辑器![buff编辑器](https://github.com/King098/TurnBasedBattle/blob/master/CapturePhoto/编辑器_BUFF.png)
buff编辑好之后可以在技能编辑界面对指定技能使用buff修改器，添加修改技能的附带buff效果
### d.编辑好的数据怎么看效果？做好的buff当然要关联到技能上，做好的技能当然要关联到英雄上，而战斗当然需要的是英雄数据，所以把要测试的数据都堆到一个英雄数据身上，然后在测试用的TestCombat组件上选中UseTableData，将PlayerIds，EnemyIds里面填入要测试的英雄ID，然后运行即可。


# 4.未来将会加入的功能
###  a.技能升级，英雄升级，英雄升星，英雄等级之类将会计入属性关联的计算功能
###  b.编辑器完善，支持自由度更高
###  c.增加资源加载的多种方式，目前资源只支持Resources目录下固定路径加载
###  d.增加新的3D Demo，加强现在2D Demo
###  e.bug修复，bug修复，bug修复（目前最关键的）

# 5.注意事项
###  a.不要试图制造互相冲突或者明显不合理的数值去测试，这样百分之一百是不可能正常的
###  b.目前有个Demo在工程中，只有一个测试用场景，是个2D的，设置了两队分别为两个人，自己的称为P1，P2，敌人的称为E1，E2；P1血厚，纯物理攻击，两个技能，一个普通单体攻击，一个蓄力单体攻击；P2血少远攻，两个主动技能，一个被动技能；主动1，远程单体魔法攻击；主动2，远程物理攻击附带一个流血的buff（2回合持续掉血）；被动1，死亡时候恢复己方随机两名角色的血量上限的一半血，对自身也有效果。E1，E2是一样的，两个主动技能，一个被动技能；主动1，对敌方随机两名角色使用物理攻击，主动2，对敌方随即两名角色使用远程魔法攻击；被动1，自身死亡可以立即复活并恢复一半血量，仅对自身有效，一次战斗仅能使用一次。多人目标的技能在目标数量上限少于可针对数量时候按照目标人数上限设置。
###  c.目前项目中的资源只支持Resources固定路径下的设置，如果需要使用其他路径，可以自行扩展


一些Demo效果图，项目中所用美术素材均来自网络
![Demo1](https://github.com/King098/TurnBasedBattle/blob/master/CapturePhoto/Demo1.png)
![Demo2](https://github.com/King098/TurnBasedBattle/blob/master/CapturePhoto/Demo2.png)
![Demo3](https://github.com/King098/TurnBasedBattle/blob/master/CapturePhoto/Demo3.png)
![Demo4](https://github.com/King098/TurnBasedBattle/blob/master/CapturePhoto/Demo4.png)
![Demo5](https://github.com/King098/TurnBasedBattle/blob/master/CapturePhoto/Demo5.png)
