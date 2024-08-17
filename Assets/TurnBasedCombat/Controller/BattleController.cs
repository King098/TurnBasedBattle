using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AEventManager;
using static King.TurnBasedCombat.EventsConst;

namespace King.TurnBasedCombat
{
    public enum LogType
    {
        INFO = 1,
        WARNING = 2,
        ERROR = 3,
    }
    /// <summary>
    /// 回合制战斗控制器（单例模式）
    /// </summary>
    public class BattleController : MonoBehaviour
    {
        #region 单例设计
        private static BattleController m_self = null;
        public static BattleController Instance { get { return m_self; } }

        void Awake()
        {
            m_self = this;
            Init();
        }
        #endregion
        /// <summary>
        /// 战斗场景的控制对象
        /// </summary>
        public BattleStage BattleStage;
        /// <summary>
        /// 是否自动战斗
        /// </summary>
        public bool IsAutoBattle;
        /// <summary>
        /// 是否是调试模式
        /// </summary>
        public bool DebugMode;

        #region 战斗系统数值变量定义及初始化系统
        /// <summary>
        /// 当前战斗系统的状态
        /// </summary>
        private Global.CombatSystemType _SystemState;
        /// <summary>
        /// 所有参与到战斗中的队伍信息
        /// </summary>
        private List<HeroTeamMono> _AllTeams;
        /// <summary>
        /// 当前参与战斗的所有阵营组数据
        /// </summary>
        private List<string> _AllTeamGroups;
        /// <summary>
        /// 当前玩家所在的阵营组
        /// </summary>
        private string _MineTeamGroup;
        /// <summary>
        /// 当前剩余的可进行攻击的英雄的队列
        /// </summary>
        private List<HeroMono> _AttackList;
        /// <summary>
        /// 当前回合的英雄对象
        /// </summary>
        private HeroMono _CurTurnHero;
        /// <summary>
        /// 是否正在进行一场战斗
        /// </summary>
        private bool _IsBattling;
        /// <summary>
        /// 是否执行了一个系统状态
        /// </summary>
        private bool _IsExcuteAction;
        /// <summary>
        /// 回合数
        /// </summary>
        private int _TurnNumber;
        public int TurnNumber
        {
            get
            {
                return _TurnNumber;
            }
        }
        public HeroMono CurTurnHero
        {
            get
            {
                return _CurTurnHero;
            }
        }

        /// <summary>
        /// 系统中输出使用这个接口统一管理
        /// </summary>
        public void DebugLog(LogType type,string msg)
        {
            if(!DebugMode)
                return;
            if(type == LogType.WARNING)
            {
                Debug.LogWarning(msg);
            }
            else if(type == LogType.ERROR)
            {
                Debug.LogError(msg);
            }
            else
            {
                Debug.Log(msg);
            }
        }

        /// <summary>
        /// 初始化系统数据
        /// </summary>
        void Init()
        {
            _SystemState = Global.CombatSystemType.ExitSystem;
            _AllTeams = new List<HeroTeamMono>();
            _AllTeamGroups = new List<string>();
            _AttackList = new List<HeroMono>();
            _CurTurnHero = null;
            _IsBattling = false;
            _IsExcuteAction = true;
        }
        #endregion

        #region 战斗状态逻辑
        /// <summary>
        /// 开始一场战斗初始化战斗系统
        /// </summary>
        void InitSystem()
        {
            //广播系统初始化
            EventManager.Instance.TriggerEvent(EventsConst.OnInitSystem);
            //回合数重置
            _TurnNumber = 0;
            //播放一些开场动画之类的，或者需要穿插剧情，就在这里进行判断
            StartCoroutine(WaitForNext(SystemSetting.BattlePerTurnStartGapTime, () =>
            {
                DebugLog(LogType.INFO,"InitSystem");
                //初始化完毕进入下一状态
                ToActionState();
            }));
        }

        /// <summary>
        /// 每个回合行动前调用
        /// </summary>
        void BeforeAction()
        {
            //对行动列表进行处理(这里会处理多回合和跳过回合的情况)
            CaculateActiveHeroList();
            //接着从行动列表中选出速度最快的英雄作为当前回合的行动者
            GetMaxSpeedHero();
            //增加一个回合数
            _TurnNumber += 1;
            //广播回合开始之前
            EventManager.Instance.TriggerEvent(EventsConst.OnBeforeAction,new CommonHeroMonoEventArgs(_CurTurnHero));
            //显示当前回合英雄高亮
            // CurrentBattleUI.SelectHero(_CurTurnHero);
            //接着判断一下当前回合的英雄buff或者debuff有没有起作用的，需要再次处理
            _CurTurnHero.ExcuteBuff(Global.BuffActiveState.BeforeAction);
            //如果执行完buff操作之后，英雄血量变空，则进入下一个准备开始阶段
            if(!_CurTurnHero.HasLife())
            {
                //这里先让buff执行，之后再让其他逻辑执行，让每次使用技能都有时间差
                StartCoroutine(WaitForNext(SystemSetting.BattlePerTurnEndGapTime,()=>
                {
                    if(_CurTurnHero.IsDead())
                    {
                        //TODO 被debuff杀死时候需要重新下一个回合，还有是否有恢复技能未使用
                        AbortAction(Global.CombatSystemType.AfterAction);
                    }
                    else
                    {
                        //这里是血量为空，但是又技能可以复活的时候执行
                        _CurTurnHero.ExcuteBuff(Global.BuffActiveState.IsDead);
                        _CurTurnHero.ExcuteSkill(Global.BuffActiveState.IsDead);
                        //这里做个延迟
                        StartCoroutine(WaitForNext(SystemSetting.BattlePerTurnEndGapTime,()=>
                        {
                            //接着判断一下当前回合的英雄有没有回合开始前发动的技能，然后处理一下
                            if (!_CurTurnHero.ExcuteSkill(Global.BuffActiveState.BeforeAction))
                            {
                                DebugLog(LogType.INFO,"BeforeAction");
                                //如果没有技能执行，则手动进入下一状态
                                ToActionState();
                            }
                        }));
                    } 
                }));
                return;
            }
            //接着判断一下当前回合的英雄有没有回合开始前发动的技能，然后处理一下
            if (!_CurTurnHero.ExcuteSkill(Global.BuffActiveState.BeforeAction))
            {
                DebugLog(LogType.INFO,"BeforeAction");
                //如果没有技能执行，则手动进入下一状态
                ToActionState();
            }
        }

        /// <summary>
        /// 每个回合行动中调用
        /// </summary>
        void Actioning()
        {
            //取消所有英雄的可选
            this.DisableChooseHeroes();
            //英雄开始执行回合
            _CurTurnHero.HeroActioning();
            //广播回合执行的时候
            EventManager.Instance.TriggerEvent(EventsConst.OnActioning);
            //冷却一回合技能CD或者蓄力冷却一回合
            _CurTurnHero.CoolDownSkillCD(1);
            BaseSkill skill = _CurTurnHero.CoolDownSkillDelay(1);
            if (skill != null)
            {
                if (skill.CurrentDelayTurn > 0)
                {
                    //蓄力或者使用这个技能
                    _CurTurnHero.Attack(skill.SkillType);
                }
                else
                {
                    //直接使用这个技能
                    _CurTurnHero.ExcuteSkill(skill.SkillType);
                }
            }
            else
            {
                //如果当前没有蓄力技能释放
                //如果是玩家的回合,并且不是自动战斗，则将操作交给玩家
                if (_CurTurnHero.IsControlledByMine() && !IsAutoBattle)
                {
                    //等待玩家的输入
                    // CurrentInputController.WaitForInput(_CurTurnHero);
                    EventManager.Instance.TriggerEvent(EventsConst.OnWaitingPlayerInput,new CommonHeroMonoEventArgs(_CurTurnHero));
                }
                else
                {
                    //如果可以施放技能，就施放技能
                    _CurTurnHero.AutoExcuteSkill();
                }
            }
        }

        /// <summary>
        /// 每个回合行动之后调用
        /// </summary>
        void AfterAction()
        {
            
            StartCoroutine(WaitForNext(SystemSetting.BattlePerTurnEndGapTime, () =>
            {
                //首先判断一下当前回合的英雄buff或者debuff有没有起作用的，需要再次处理
                _CurTurnHero.ExcuteBuff(Global.BuffActiveState.AfterAction);
                //接着判断当前英雄被动技能是否有发动的
                if (!_CurTurnHero.ExcuteSkill(Global.BuffActiveState.AfterAction))
                {
                    //关闭英雄高亮
                    // CurrentBattleUI.DeselectHero(_CurTurnHero);
                    DebugLog(LogType.INFO,"AfterAction");
                    //广播回合执行完毕的时候
                    EventManager.Instance.TriggerEvent(EventsConst.OnAfterAction,new CommonHeroMonoEventArgs(_CurTurnHero));
                    //切换状态
                    ToActionState();
                }
            }));
        }

        /// <summary>
        /// 一场战斗结束时候调用
        /// </summary>
        void CombatEnd()
        {
            string winTeamGroup = CheckWinTeamGroup();
            if(!string.IsNullOrEmpty(winTeamGroup))
            {
                DebugLog(LogType.INFO, $"小队{winTeamGroup}胜利");
            }
            //广播战斗结束的时候
            EventManager.Instance.TriggerEvent(EventsConst.OnCombatBattleEnd,new CommonCombatEndEventArgs(winTeamGroup));
            //战斗结束，根据胜利失败显示结算画面
            ToActionState();
        }

        /// <summary>
        /// 退出战斗系统的时候调用
        /// </summary>
        void ExitSystem()
        {
            //退出战斗系统的时候清空数据
            Clear();
            //并重置战斗标记
            _IsBattling = false;
            //广播退出战斗系统的时候
            EventManager.Instance.TriggerEvent(EventsConst.OnExitSystem);
        }

        void Update()
        {
            if (_IsBattling && !_IsExcuteAction)
            {
                _IsExcuteAction = true;
                switch (_SystemState)
                {
                    case Global.CombatSystemType.InitSystem:
                        InitSystem();
                        break;
                    case Global.CombatSystemType.BeforeAction:
                        BeforeAction();
                        break;
                    case Global.CombatSystemType.Actioning:
                        Actioning();
                        break;
                    case Global.CombatSystemType.AfterAction:
                        AfterAction();
                        break;
                    case Global.CombatSystemType.CombatEnd:
                        CombatEnd();
                        break;
                    case Global.CombatSystemType.ExitSystem:
                        ExitSystem();
                        break;
                }
            }
        }
        #endregion

        /// <summary>
        /// 开始一场战斗
        /// </summary>
        /// <param name="player">参与战斗的玩家英雄数据</param>
        /// <param name="enemy">参与战斗的敌人英雄数据</param>
        public async void StartBattle(List<Hero> player, List<Hero> enemy)
        {
            List<HeroTeam> heroTeams = new List<HeroTeam>
            {
                new HeroTeam(player, HeroTeamType.Mine, 0,HeroTeam.MineTeamGroup),
                new HeroTeam(enemy, HeroTeamType.NPC, 1,HeroTeam.EnemyTeamGroup)
            };
            _AllTeams.Clear();
            _AllTeamGroups.Clear();
            for(int i = 0;i<heroTeams.Count;i++)
            {
                HeroTeamMono teamMono = await this.BattleStage.LoadHeroes(heroTeams[i]);
                this._AllTeams.Add(teamMono);
                if(!_AllTeamGroups.Contains(teamMono.TeamGroup))
                {
                    _AllTeamGroups.Add(teamMono.TeamGroup);
                }
                if(teamMono.TeamType == HeroTeamType.Mine)
                {
                    _MineTeamGroup = teamMono.TeamGroup;
                }
            }
            //进入到战斗系统初始化阶段
            _SystemState = Global.CombatSystemType.InitSystem;
            _IsExcuteAction = false;
            _IsBattling = true;
            //广播一场战斗开始
            EventManager.Instance.TriggerEvent(EventsConst.OnCombatBattleStart,new CombatBattleStartEventArgs(this._AllTeams));
        }

        /// <summary>
        /// 开始一场战斗，最多配置四个阵营的数据
        /// </summary>
        /// <param name="teams"></param>
        public async void StartBattle(List<HeroTeam> teams)
        {
            _AllTeams.Clear();
            _AllTeamGroups.Clear();
            for(int i = 0;i<teams.Count;i++)
            {
                HeroTeamMono teamMono = await this.BattleStage.LoadHeroes(teams[i]);
                this._AllTeams.Add(teamMono);
                if(!_AllTeamGroups.Contains(teamMono.TeamGroup))
                {
                    _AllTeamGroups.Add(teamMono.TeamGroup);
                }
                if(teamMono.TeamType == HeroTeamType.Mine)
                {
                    _MineTeamGroup = teamMono.TeamGroup;
                }
            }
            //进入到战斗系统初始化阶段
            _SystemState = Global.CombatSystemType.InitSystem;
            _IsExcuteAction = false;
            _IsBattling = true;
            //广播一场战斗开始
            EventManager.Instance.TriggerEvent(EventsConst.OnCombatBattleStart,new CombatBattleStartEventArgs(this._AllTeams));
        }

        /// <summary>
        /// 获取传入英雄是被那个阵营在操控
        /// </summary>
        /// <param name="hero"></param>
        /// <returns></returns>
        public HeroTeamMono GetHeroControlledBy(HeroMono hero)
        {
            for(int i = 0;i<_AllTeams.Count;i++)
            {
                if(_AllTeams[i].IsControledHero(hero))
                {
                    return _AllTeams[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 获取指定阵营组的指定阵营分类的所有阵营组列表
        /// </summary>
        /// <param name="teamGroup">哪个阵营的</param>
        /// <param name="teamGroupType">什么阵营分类</param>
        /// <returns></returns>
        List<string> GetTeamGroups(string teamGroup,TeamGroupType teamGroupType)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < _AllTeams.Count; i++)
            {
                if((teamGroupType == TeamGroupType.SameTeamGroup && _AllTeams[i].TeamGroup == teamGroup) || teamGroupType == TeamGroupType.AllTeamGroup)
                {
                    result.Add(_AllTeams[i].TeamGroup);
                }
                else if((teamGroupType == TeamGroupType.NotSameTeamGroup && _AllTeams[i].TeamGroup != teamGroup) || teamGroupType == TeamGroupType.AllTeamGroup)
                {
                    result.Add(_AllTeams[i].TeamGroup);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取指定阵营中所有活着的英雄对象
        /// </summary>
        /// <param name="teamGroups">目标阵营类型</param>
        /// <param name="containOther">是否包含被控制的其他角色</param>
        /// <returns></returns>
        List<HeroMono> GetTeamsAlive(List<string> teamGroups,bool containOther = true)
        {
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < _AllTeams.Count; i++)
            {
                if(teamGroups.Contains(_AllTeams[i].TeamGroup))
                {
                    List<HeroMono> heroes = _AllTeams[i].GetTeamAlive(containOther);
                    for(int j = 0; j < heroes.Count; j++)
                    {
                        if(!result.Contains(heroes[j]))
                        {
                            result.Add(heroes[j]);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取所有友方包含自己队列中剩余的存活英雄对象
        /// </summary>
        /// <returns></returns>
        List<HeroMono> GetFriendlyTeamsAlive(string teamGroup,bool containOther = true)
        {
            List<string> groups = GetTeamGroups(teamGroup,TeamGroupType.SameTeamGroup);
            return GetTeamsAlive(groups,containOther);
        }

        /// <summary>
        /// 获取敌方队伍中诉讼有剩余的存货英雄
        /// </summary>
        /// <param name="containOther">是否包含被控的英雄</param>
        /// <returns></returns>
        List<HeroMono> GetEnemyTeamsAlive(string teamGroup,bool containOther = true)
        {
            List<string> groups = GetTeamGroups(teamGroup,TeamGroupType.NotSameTeamGroup);
            return GetTeamsAlive(groups,containOther);
        }

        /// <summary>
        /// 获取指定阵营死亡的英雄
        /// </summary>
        /// <param name="teamTypes"></param>
        /// <param name="containOther">是否包含其他阵营被控制的英雄</param>
        /// <returns></returns>
        List<HeroMono> GetTeamsDead(List<string> teamGroups,bool containOther = true)
        {
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < _AllTeams.Count; i++)
            {
                if(teamGroups.Contains(_AllTeams[i].TeamGroup))
                {
                    List<HeroMono> heroes = _AllTeams[i].GetTeamDead(containOther);
                    for(int j = 0; j < heroes.Count; j++)
                    {
                        if(!result.Contains(heroes[j]))
                        {
                            result.Add(heroes[j]);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取友方包含自己阵营的死亡英雄
        /// </summary>
        /// <param name="containOther"></param>
        /// <returns></returns>
        List<HeroMono> GetFriendlyTeamsDead(string teamGroup,bool containOther = true)
        {
            List<string> groups = GetTeamGroups(teamGroup,TeamGroupType.SameTeamGroup);
            return GetTeamsDead(groups,containOther);
        }

        /// <summary>
        /// 获取敌方阵营的死亡英雄
        /// </summary>
        /// <param name="containOther"></param>
        /// <returns></returns>
        List<HeroMono> GetEnemyTeamsDead(string teamGroup,bool containOther = true)
        {
            List<string> groups = GetTeamGroups(teamGroup,TeamGroupType.SameTeamGroup);
            return GetTeamsDead(groups,containOther);
        }

        /// <summary>
        /// 获取指定阵营的不论生死的英雄列表
        /// </summary>
        /// <param name="teamTypes"></param>
        /// <param name="containOther"></param>
        /// <returns></returns>
        List<HeroMono> GetTeamsHero(List<string> teamGroups, bool containOther = true)
        {
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < _AllTeams.Count; i++)
            {
                if(teamGroups.Contains(_AllTeams[i].TeamGroup))
                {
                    List<HeroMono> heroes = _AllTeams[i].GetTeamHero(containOther);
                    for(int j = 0; j < heroes.Count; j++)
                    {
                        if(!result.Contains(heroes[j]))
                        {
                            result.Add(heroes[j]);
                        }
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// 获取当前技能的目标对象列表
        /// </summary>
        /// <param name="attacker">技能发起者</param>
        /// <param name="skillTarget">技能目标类型</param>
        /// <returns>返回目标对象列表</returns>
        public List<HeroMono> SkillTarget(HeroMono attacker, Global.SkillTargetType skillTarget,int targetNumber,bool containsOther)
        {
            List<HeroMono> result = new List<HeroMono>();
            HeroTeamMono heroTeam = GetHeroControlledBy(attacker);
            switch (skillTarget)
            {
                case Global.SkillTargetType.None:
                    break;
                case Global.SkillTargetType.AliveEnemy:
                    result.AddRange(GetAliveTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.NotSameTeamGroup),targetNumber,containsOther));
                    break;
                case Global.SkillTargetType.DeadEnemy:
                    result.AddRange(GetDeadTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.NotSameTeamGroup),targetNumber,containsOther));
                    break;
                case Global.SkillTargetType.Enemy:
                    result.AddRange(GetTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.NotSameTeamGroup),targetNumber,containsOther));
                    break;
                case Global.SkillTargetType.AliveMine:
                    result.AddRange(GetAliveTargetByHeroTeam(heroTeam,targetNumber,containsOther));
                    break;
                case Global.SkillTargetType.DeadMine:
                    result.AddRange(GetDeadTargetByHeroTeam(heroTeam,targetNumber,containsOther));
                    break;
                case Global.SkillTargetType.Mine:
                    result.AddRange(GetTargetHerosByHeroTeam(heroTeam,targetNumber,containsOther));
                    break;
                case Global.SkillTargetType.AliveFriendly:
                    result.AddRange(GetAliveTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.SameTeamGroup),targetNumber,containsOther));
                    break;
                case Global.SkillTargetType.DeadFriendly:
                    result.AddRange(GetDeadTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.SameTeamGroup),targetNumber,containsOther));
                    break;
                case Global.SkillTargetType.Friendly:
                    result.AddRange(GetTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.SameTeamGroup),targetNumber,containsOther));
                    break;
                case Global.SkillTargetType.Self:
                    result.Add(attacker);
                    break;
                case Global.SkillTargetType.AllAlive:
                    result.AddRange(GetAliveTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.AllTeamGroup),targetNumber,containsOther));
                    break;
                case Global.SkillTargetType.AllDead:
                    result.AddRange(GetDeadTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.AllTeamGroup),targetNumber,containsOther));
                    break;
                case Global.SkillTargetType.All:
                    result.AddRange(GetTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.AllTeamGroup),targetNumber,containsOther));
                    break;
            }
            return result;
        }

        /// <summary>
        /// 高亮某个英雄使用技能的所有可选目标
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="skillTarget"></param>
        /// <param name="targetNumber"></param>
        /// <param name="containsOther"></param>
        public void HighlightTargets(HeroMono attacker, Global.SkillTargetType skillTarget,bool containsOther)
        {
            List<HeroMono> result = new List<HeroMono>();
            HeroTeamMono heroTeam = GetHeroControlledBy(attacker);
            switch (skillTarget)
            {
                case Global.SkillTargetType.None:
                    break;
                case Global.SkillTargetType.AliveEnemy:
                    result.AddRange(GetAliveTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.NotSameTeamGroup),99999,containsOther));
                    break;
                case Global.SkillTargetType.DeadEnemy:
                    result.AddRange(GetDeadTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.NotSameTeamGroup),99999,containsOther));
                    break;
                case Global.SkillTargetType.Enemy:
                    result.AddRange(GetTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.NotSameTeamGroup),99999,containsOther));
                    break;
                case Global.SkillTargetType.AliveMine:
                    result.AddRange(GetAliveTargetByHeroTeam(heroTeam,99999,containsOther));
                    break;
                case Global.SkillTargetType.DeadMine:
                    result.AddRange(GetDeadTargetByHeroTeam(heroTeam,99999,containsOther));
                    break;
                case Global.SkillTargetType.Mine:
                    result.AddRange(GetTargetHerosByHeroTeam(heroTeam,99999,containsOther));
                    break;
                case Global.SkillTargetType.AliveFriendly:
                    result.AddRange(GetAliveTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.SameTeamGroup),99999,containsOther));
                    break;
                case Global.SkillTargetType.DeadFriendly:
                    result.AddRange(GetDeadTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.SameTeamGroup),99999,containsOther));
                    break;
                case Global.SkillTargetType.Friendly:
                    result.AddRange(GetTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.SameTeamGroup),99999,containsOther));
                    break;
                case Global.SkillTargetType.Self:
                    result.Add(attacker);
                    break;
                case Global.SkillTargetType.AllAlive:
                    result.AddRange(GetAliveTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.AllTeamGroup),99999,containsOther));
                    break;
                case Global.SkillTargetType.AllDead:
                    result.AddRange(GetDeadTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.AllTeamGroup),99999,containsOther));
                    break;
                case Global.SkillTargetType.All:
                    result.AddRange(GetTarget(GetTeamGroups(heroTeam.TeamGroup,TeamGroupType.AllTeamGroup),99999,containsOther));
                    break;
            }
            for(int i = 0;i<result.Count;i++)
            {
                result[i].HeroCanChoose();
            }
        }

        /// <summary>
        /// 取消所有英雄的高亮效果
        /// </summary>
        public void DisableChooseHeroes()
        {
            for(int i = 0;i<_AllTeams.Count;i++)
            {
                for(int j = 0;j<_AllTeams[i].Heros.Count;j++)
                {
                    _AllTeams[i].Heros[j].HeroCannotChoose();
                }
            }
        }

        /// <summary>
        /// 进入下一个状态
        /// </summary>
        public void ToActionState()
        {
            //设置下一个阶段的状态
            switch (_SystemState)
            {
                case Global.CombatSystemType.InitSystem:
                    _SystemState = Global.CombatSystemType.BeforeAction;
                    break;
                case Global.CombatSystemType.BeforeAction:
                    _SystemState = Global.CombatSystemType.Actioning;
                    break;
                case Global.CombatSystemType.Actioning:
                    _SystemState = Global.CombatSystemType.AfterAction;
                    break;
                case Global.CombatSystemType.AfterAction:
                    if (!string.IsNullOrEmpty(CheckWinTeamGroup()))
                    {
                        _SystemState = Global.CombatSystemType.CombatEnd;
                    }
                    else
                    {
                        _SystemState = Global.CombatSystemType.BeforeAction;
                    }
                    break;
                case Global.CombatSystemType.CombatEnd:
                    _SystemState = Global.CombatSystemType.ExitSystem;
                    break;
            }
            //表示这个阶段执行完毕可以进行下一阶段了
            _IsExcuteAction = false;
        }

        /// <summary>
        /// 强制结束状态到什么状态
        /// </summary>
        public void AbortAction(Global.CombatSystemType state)
        {
            _SystemState = state;
            //表示这个阶段执行完毕可以进行下一阶段了
            _IsExcuteAction = false;
        }

        /// <summary>
        /// 计算当前剩余的可行动的英雄
        /// </summary>
        /// <returns></returns>
        void CaculateActiveHeroList()
        {
            //判断上一回合行动英雄
            if (_CurTurnHero != null)
            {
                if (_CurTurnHero.CurrentTurnLast <= 0)
                {
                    //表示被跳过回合了或者行动回合结束了，则移出行动列表
                    _AttackList.Remove(_CurTurnHero);
                }
                //然后将剩余回合数增加-1
                _CurTurnHero.CurrentTurnLast += -1;
            }

            //最后判断一下是否行动列表已经为空,为空则重置行动列表
            if (_AttackList.Count == 0)
            {
                List<HeroMono> beforeList = new List<HeroMono>();
                List<HeroMono> afterList = new List<HeroMono>();
                List<string> hasAddTeamGroup = new List<string>();
                for(int i = 0;i<_AllTeams.Count;i++)
                {
                    if(hasAddTeamGroup.Contains(_AllTeams[i].TeamGroup))
                    {
                        continue;
                    }
                    if(_AllTeams[i].TeamType == HeroTeamType.NPC)
                    {
                        beforeList.AddRange(GetFriendlyTeamsAlive(_AllTeams[i].TeamGroup,false));
                        hasAddTeamGroup.Add(_AllTeams[i].TeamGroup);
                    }
                    else
                    {
                        afterList.AddRange(GetFriendlyTeamsAlive(_AllTeams[i].TeamGroup,false));
                        hasAddTeamGroup.Add(_AllTeams[i].TeamGroup);
                    }
                }
                _AttackList.AddRange(beforeList);
                _AttackList.AddRange(afterList);
            }
        }

        /// <summary>
        /// 获取当前行动列表中速度最快的英雄对象
        /// </summary>
        void GetMaxSpeedHero()
        {
            _CurTurnHero = _AttackList[0];
            for (int i = 0; i < _AttackList.Count; i++)
            {
                if (_AttackList[i].CurrentSpeed > _CurTurnHero.CurrentSpeed && !_AttackList[i].IsDead())
                {
                    _CurTurnHero = _AttackList[i];
                }
            }
            if (_CurTurnHero.IsDead())
            {
                // Debug.Log("当前随机到了：" + _CurTurnHero.Name + " 剩余血量：" + _CurTurnHero.CurrentLife + " 死亡标志为：" + _CurTurnHero.IsDeath);
                // //表示当前列表中没有可以行动的英雄，重新计算行动列表,重新开始这个回合
                // Debug.Log("回合重新计算");
                //对行动列表进行处理(这里会处理多回合和跳过回合的情况)
                CaculateActiveHeroList();
                //接着从行动列表中选出速度最快的英雄作为当前回合的行动者
                GetMaxSpeedHero();
            }
        }

        /// <summary>
        /// 等待下一步操作的协成
        /// </summary>
        /// <param name="time">等待的时间</param>
        /// <param name="callback">下一步操作</param>
        /// <returns>返回协程的handler</returns>
        IEnumerator WaitForNext(float time, System.Action callback)
        {
            yield return new WaitForSeconds(time);
            if (callback != null)
            {
                callback();
            }
        }

        /// <summary>
        /// 获取一定数量的活着的目标
        /// </summary>
        /// <param name="teamTypes">从指定的阵营类型取英雄</param>
        /// <param name="number">获取的数量</param>
        /// <returns>返回目标列表</returns>
        List<HeroMono> GetAliveTarget(List<string> groupTyps, int number,bool containsOther = true)
        {
            List<HeroMono> alive = GetTeamsAlive(groupTyps,containsOther);
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < (alive.Count > number ? number : alive.Count); i++)
            {
                int r = UnityEngine.Random.Range(0, alive.Count);
                HeroMono hero = alive[r];
                while (result.Contains(hero))
                {
                    r = UnityEngine.Random.Range(0, alive.Count);
                    hero = alive[r];
                }
                result.Add(alive[r]);
            }
            return result;
        }

        /// <summary>
        /// 获取一定数量的死亡的目标
        /// </summary>
        /// <param name="teamTypes">指定的阵营类型</param>
        /// <param name="number">获取的数量</param>
        /// <returns>返回目标列表</returns>
        List<HeroMono> GetDeadTarget(List<string> teamGroups, int number,bool containsOther = true)
        {
            List<HeroMono> dead = GetTeamsDead(teamGroups,containsOther);
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < (dead.Count > number ? number : dead.Count); i++)
            {
                int r = UnityEngine.Random.Range(0, dead.Count);
                HeroMono hero = dead[r];
                while (result.Contains(hero))
                {
                    r = UnityEngine.Random.Range(0, dead.Count);
                    hero = dead[r];
                }
                result.Add(dead[r]);
            }
            return result;
        }

        /// <summary>
        /// 获取一定数量的目标
        /// </summary>
        /// <param name="teamTypes">指定的阵营类型</param>
        /// <param name="number">获取的数量</param>
        /// <returns>返回目标列表</returns>
        List<HeroMono> GetTarget(List<string> teamGroups, int number,bool containOther = true)
        {
            List<HeroMono> dead = GetTeamsHero(teamGroups,containOther);
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < (dead.Count > number ? number : dead.Count); i++)
            {
                int r = UnityEngine.Random.Range(0, dead.Count);
                HeroMono hero = dead[r];
                while (result.Contains(hero))
                {
                    r = UnityEngine.Random.Range(0, dead.Count);
                    hero = dead[r];
                }
                result.Add(dead[r]);
            }
            return result;
        }

        /// <summary>
        /// 从指定的阵营中获取活着的目标
        /// </summary>
        /// <param name="team"></param>
        /// <param name="number"></param>
        /// <param name="containOther"></param>
        /// <returns></returns>
        List<HeroMono> GetAliveTargetByHeroTeam(HeroTeamMono team,int number,bool containOther = true)
        {
            List<HeroMono> dead = team.GetTeamAlive(containOther);
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < (dead.Count > number ? number : dead.Count); i++)
            {
                int r = UnityEngine.Random.Range(0, dead.Count);
                HeroMono hero = dead[r];
                while (result.Contains(hero))
                {
                    r = UnityEngine.Random.Range(0, dead.Count);
                    hero = dead[r];
                }
                result.Add(dead[r]);
            }
            return result;
        }

        /// <summary>
        /// 从指定的阵营中获取死亡的目标
        /// </summary>
        /// <param name="team"></param>
        /// <param name="number"></param>
        /// <param name="containOther"></param>
        /// <returns></returns>
        List<HeroMono> GetDeadTargetByHeroTeam(HeroTeamMono team,int number,bool containOther = true)
        {
            List<HeroMono> dead = team.GetTeamDead(containOther);
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < (dead.Count > number ? number : dead.Count); i++)
            {
                int r = UnityEngine.Random.Range(0, dead.Count);
                HeroMono hero = dead[r];
                while (result.Contains(hero))
                {
                    r = UnityEngine.Random.Range(0, dead.Count);
                    hero = dead[r];
                }
                result.Add(dead[r]);
            }
            return result;
        }

        /// <summary>
        /// 从指定的阵营中获取不论死活的目标
        /// </summary>
        /// <param name="team"></param>
        /// <param name="number"></param>
        /// <param name="containOther"></param>
        /// <returns></returns>
        List<HeroMono> GetTargetHerosByHeroTeam(HeroTeamMono team,int number,bool containOther = true)
        {
            List<HeroMono> dead = team.GetTeamHero(containOther);
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < (dead.Count > number ? number : dead.Count); i++)
            {
                int r = UnityEngine.Random.Range(0, dead.Count);
                HeroMono hero = dead[r];
                while (result.Contains(hero))
                {
                    r = UnityEngine.Random.Range(0, dead.Count);
                    hero = dead[r];
                }
                result.Add(dead[r]);
            }
            return result;
        }

        /// <summary>
        /// 判断是否阵营已经失败了,如果失败了返回这个阵营组，否则返回空字符串
        /// </summary>
        /// <returns></returns>
        public string CheckHasTeamGroupFailed()
        {
            for (int i = 0; i < _AllTeamGroups.Count; i++)
            {
                string teamGroup = _AllTeamGroups[i];
                if(CheckTeamGroupFailed(teamGroup))
                {
                    return teamGroup;
                }
            }
            return "";
        }

        /// <summary>
        /// 判断指定阵营组是否已经完全失败了
        /// </summary>
        /// <param name="teamGroup"></param>
        /// <returns></returns>
        public bool CheckTeamGroupFailed(string teamGroup)
        {
            for (int j = 0; j < _AllTeams.Count; j++)
            {
                if (_AllTeams[j].TeamGroup == teamGroup && !_AllTeams[j].CheckIsFailed())
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 获取还有战斗能力的阵营组列表
        /// </summary>
        /// <returns></returns>
        public List<string> CheckAliveTeamGroups()
        {
            List<string> result = new List<string>();
            for (int i = 0; i < _AllTeamGroups.Count; i++)
            {
                string teamGroup = _AllTeamGroups[i];
                if(!CheckTeamGroupFailed(teamGroup))
                {
                    result.Add(teamGroup);
                }
            }
            return result;
        }

        /// <summary>
        /// 检测最终获胜的阵营组
        /// </summary>
        /// <returns></returns>
        public string CheckWinTeamGroup()
        {
            List<string> aliveTeamGroups = CheckAliveTeamGroups();
            if(aliveTeamGroups.Count == 1)
            {
                return aliveTeamGroups[0];
            }
            return "";
        }

        /// <summary>
        /// 退出的时候清除调用
        /// </summary>
        public void Clear()
        {
            //清除所有阵营的对象
            _AllTeams.Clear();
            //清除行动列表
            _AttackList.Clear();
            //清空Stage
            BattleStage.Clear();
            //清除当前回合英雄引用
            _CurTurnHero = null;
            //清除技能控制器
            SkillController.Instance.Clear();
        }
    }
}