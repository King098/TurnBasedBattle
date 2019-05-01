using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using King.Tools;
using UnityEngine.UI;

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
        /// 当前使用的战斗UI
        /// </summary>
        public BaseBattleUI CurrentBattleUI;
        /// <summary>
        /// 当前使用的用户输入控制器
        /// </summary>
        public BaseInputController CurrentInputController;
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
        /// 当前进入战斗的玩家队伍信息
        /// </summary>
        private List<HeroMono> _PlayerTeam;
        /// <summary>
        /// 当前进入战斗的敌人队伍信息
        /// </summary>
        private List<HeroMono> _EnemyTeam;
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
            if (CurrentBattleUI == null)
            {
                CurrentBattleUI = GetComponent<BaseBattleUI>();
                if (CurrentBattleUI == null)
                {
                    CurrentBattleUI = this.gameObject.AddComponent<BaseBattleUI>();
                }
                DebugLog(LogType.ERROR,"Not found Battle UI,Use BaseBattleUI instead!");
            }
            _SystemState = Global.CombatSystemType.ExitSystem;
            _PlayerTeam = new List<HeroMono>();
            _EnemyTeam = new List<HeroMono>();
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
            //显示当前回合英雄高亮
            CurrentBattleUI.SelectHero(_CurTurnHero);
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
            //出去当前英雄的其他英雄所有技能CD和蓄力进行减少
            //for (int i = 0; i < _PlayerTeam.Count; i++)
            //{
            //    if (_PlayerTeam[i] != _CurTurnHero)
            //    {
            //        _PlayerTeam[i].CoolDownSkillCD(1);
            //        _PlayerTeam[i].CoolDownSkillDelay(1);
            //    }
            //}
            //for (int i = 0; i < _EnemyTeam.Count; i++)
            //{
            //    if (_EnemyTeam[i] != _CurTurnHero)
            //    {
            //        _EnemyTeam[i].CoolDownSkillCD(1);
            //        _EnemyTeam[i].CoolDownSkillDelay(1);
            //    }
            //}
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
                if (_CurTurnHero.IsPlayerHero && !IsAutoBattle)
                {
                    //等待玩家的输入
                    CurrentInputController.WaitForInput(_CurTurnHero);
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
                    CurrentBattleUI.DeselectHero(_CurTurnHero);
                    DebugLog(LogType.INFO,"AfterAction");
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
            if (CheckPlayerFailed())
            {
                DebugLog(LogType.INFO,"玩家失败");
            }
            else if (CheckEnemyFailed())
            {
                DebugLog(LogType.INFO,"玩家胜利");
            }
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
        public void StartBattle(List<Hero> player, List<Hero> enemy)
        {
            //初始化BattleUI
            CurrentBattleUI.Init();
            //创建所有的参与战斗的英雄UI
            CurrentBattleUI.CreateHeros(player, enemy);
            //进入到战斗系统初始化阶段
            _SystemState = Global.CombatSystemType.InitSystem;
            _IsExcuteAction = false;
            _IsBattling = true;
        }

        /// <summary>
        /// 向控制器注册玩家英雄的Mono对象
        /// </summary>
        /// <param name="hero"></param>
        public void RegisterPlayerHeroMono(HeroMono hero)
        {
            _PlayerTeam.Add(hero);
        }


        /// <summary>
        /// 向控制器注册敌人英雄的Mono对象
        /// </summary>
        /// <param name="hero"></param>
        public void RegisterEnemyHeroMono(HeroMono hero)
        {
            _EnemyTeam.Add(hero);
        }

        /// <summary>
        /// 获取当前技能的目标对象列表
        /// </summary>
        /// <param name="attacker">技能发起者</param>
        /// <param name="skillTarget">技能目标类型</param>
        /// <returns>返回目标对象列表</returns>
        public List<HeroMono> SkillTarget(HeroMono attacker, Global.SkillTargetType skillTarget)
        {
            List<HeroMono> result = new List<HeroMono>();
            switch (skillTarget)
            {
                case Global.SkillTargetType.None:
                    break;
                case Global.SkillTargetType.OneEnemy:
                    result.AddRange(GetAliveTarget(!attacker.IsPlayerHero, 1));
                    break;
                case Global.SkillTargetType.TwoEnemy:
                    result.AddRange(GetAliveTarget(!attacker.IsPlayerHero, 2));
                    break;
                case Global.SkillTargetType.ThreeEnemy:
                    result.AddRange(GetAliveTarget(!attacker.IsPlayerHero, 3));
                    break;
                case Global.SkillTargetType.FourEnemy:
                    result.AddRange(GetAliveTarget(!attacker.IsPlayerHero, 4));
                    break;
                case Global.SkillTargetType.FiveEnemy:
                    result.AddRange(GetAliveTarget(!attacker.IsPlayerHero, 5));
                    break;
                case Global.SkillTargetType.AllEnemy:
                    if (attacker.IsPlayerHero)
                    {
                        result.AddRange(GetEnemyTeamAlive());
                    }
                    else
                    {
                        result.AddRange(GetPlayerTeamAlive());
                    }
                    break;
                case Global.SkillTargetType.Self:
                    result.Add(attacker);
                    break;
                case Global.SkillTargetType.OneSelf:
                    result.AddRange(GetAliveTarget(attacker.IsPlayerHero, 1));
                    break;
                case Global.SkillTargetType.TwoSelf:
                    result.AddRange(GetAliveTarget(attacker.IsPlayerHero, 2));
                    break;
                case Global.SkillTargetType.ThreeSelf:
                    result.AddRange(GetAliveTarget(attacker.IsPlayerHero, 3));
                    break;
                case Global.SkillTargetType.FourSelf:
                    result.AddRange(GetAliveTarget(attacker.IsPlayerHero, 4));
                    break;
                case Global.SkillTargetType.FiveSelf:
                    result.AddRange(GetAliveTarget(attacker.IsPlayerHero, 5));
                    break;
                case Global.SkillTargetType.AllSelf:
                    if (attacker.IsPlayerHero)
                    {
                        result.AddRange(GetPlayerTeamAlive());
                    }
                    else
                    {
                        result.AddRange(GetEnemyTeamAlive());
                    }
                    break;
                case Global.SkillTargetType.OneEnemyWithDead:
                    result.AddRange(GetTarget(!attacker.IsPlayerHero, 1));
                    break;
                case Global.SkillTargetType.TwoEnemyWithDead:
                    result.AddRange(GetTarget(!attacker.IsPlayerHero, 2));
                    break;
                case Global.SkillTargetType.ThreeEnemyWithDead:
                    result.AddRange(GetTarget(!attacker.IsPlayerHero, 3));
                    break;
                case Global.SkillTargetType.FourEnemyWithDead:
                    result.AddRange(GetTarget(!attacker.IsPlayerHero, 4));
                    break;
                case Global.SkillTargetType.FiveEnemyWithDead:
                    result.AddRange(GetTarget(!attacker.IsPlayerHero, 5));
                    break;
                case Global.SkillTargetType.AllEnemyWithDead:
                    if (!attacker.IsPlayerHero)
                    {
                        result.AddRange(_PlayerTeam);
                    }
                    else
                    {
                        result.AddRange(_EnemyTeam);
                    }
                    break;
                case Global.SkillTargetType.OneSelfWithDead:
                    result.AddRange(GetTarget(attacker.IsPlayerHero, 1));
                    break;
                case Global.SkillTargetType.TwoSelfWithDead:
                    result.AddRange(GetTarget(attacker.IsPlayerHero, 2));
                    break;
                case Global.SkillTargetType.ThreeSelfWithDead:
                    result.AddRange(GetTarget(attacker.IsPlayerHero, 3));
                    break;
                case Global.SkillTargetType.FourSelfWithDead:
                    result.AddRange(GetTarget(attacker.IsPlayerHero, 4));
                    break;
                case Global.SkillTargetType.FiveSelfWithDead:
                    result.AddRange(GetTarget(attacker.IsPlayerHero, 5));
                    break;
                case Global.SkillTargetType.AllSelfWithDead:
                    if (attacker.IsPlayerHero)
                    {
                        result.AddRange(_PlayerTeam);
                    }
                    else
                    {
                        result.AddRange(_EnemyTeam);
                    }
                    break;
                case Global.SkillTargetType.OneDeadEnemy:
                    result.AddRange(GetDeadTarget(!attacker.IsPlayerHero, 1));
                    break;
                case Global.SkillTargetType.TwoDeadEnemy:
                    result.AddRange(GetDeadTarget(!attacker.IsPlayerHero, 2));
                    break;
                case Global.SkillTargetType.ThreeDeadEnemy:
                    result.AddRange(GetDeadTarget(!attacker.IsPlayerHero, 3));
                    break;
                case Global.SkillTargetType.FourDeadEnemy:
                    result.AddRange(GetDeadTarget(!attacker.IsPlayerHero, 4));
                    break;
                case Global.SkillTargetType.FiveDeadEnemy:
                    result.AddRange(GetDeadTarget(!attacker.IsPlayerHero, 5));
                    break;
                case Global.SkillTargetType.AllDeadEnemy:
                    if (!attacker.IsPlayerHero)
                    {
                        result.AddRange(GetPlayerTeamDead());
                    }
                    else
                    {
                        result.AddRange(GetEnemyTeamDead());
                    }
                    break;
                case Global.SkillTargetType.OneDeadSelf:
                    result.AddRange(GetDeadTarget(attacker.IsPlayerHero, 1));
                    break;
                case Global.SkillTargetType.TwoDeadSelf:
                    result.AddRange(GetDeadTarget(attacker.IsPlayerHero, 2));
                    break;
                case Global.SkillTargetType.ThreeDeadSelf:
                    result.AddRange(GetDeadTarget(attacker.IsPlayerHero, 3));
                    break;
                case Global.SkillTargetType.FourDeadSelf:
                    result.AddRange(GetDeadTarget(attacker.IsPlayerHero, 4));
                    break;
                case Global.SkillTargetType.FiveDeadSelf:
                    result.AddRange(GetDeadTarget(attacker.IsPlayerHero, 5));
                    break;
                case Global.SkillTargetType.AllDeadSelf:
                    if (attacker.IsPlayerHero)
                    {
                        result.AddRange(GetPlayerTeamDead());
                    }
                    else
                    {
                        result.AddRange(GetEnemyTeamDead());
                    }
                    break;
            }
            return result;
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
                    if (CheckPlayerFailed() || CheckEnemyFailed())
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
        /// 获取玩家队列中剩余的存活英雄对象
        /// </summary>
        /// <returns></returns>
        List<HeroMono> GetPlayerTeamAlive()
        {
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < _PlayerTeam.Count; i++)
            {
                if (!_PlayerTeam[i].IsDead())
                {
                    result.Add(_PlayerTeam[i]);
                }
            }
            return result;
        }


        /// <summary>
        /// 获取玩家队列中死亡英雄对象
        /// </summary>
        /// <returns></returns>
        List<HeroMono> GetPlayerTeamDead()
        {
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < _PlayerTeam.Count; i++)
            {
                if (_PlayerTeam[i].IsDead())
                {
                    result.Add(_PlayerTeam[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取敌人队列中剩余的存活英雄对象
        /// </summary>
        /// <returns></returns>
        List<HeroMono> GetEnemyTeamAlive()
        {
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < _EnemyTeam.Count; i++)
            {
                if (!_EnemyTeam[i].IsDead())
                {
                    result.Add(_EnemyTeam[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取敌人队列中死亡英雄对象
        /// </summary>
        /// <returns></returns>
        List<HeroMono> GetEnemyTeamDead()
        {
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < _EnemyTeam.Count; i++)
            {
                if (_EnemyTeam[i].IsDead())
                {
                    result.Add(_EnemyTeam[i]);
                }
            }
            return result;
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
                _AttackList.AddRange(GetEnemyTeamAlive());
                _AttackList.AddRange(GetPlayerTeamAlive());
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
        /// <param name="playerTeam">是否从玩家队列中获取</param>
        /// <param name="number">获取的数量</param>
        /// <returns>返回目标列表</returns>
        List<HeroMono> GetAliveTarget(bool playerTeam, int number)
        {
            List<HeroMono> alive = new List<HeroMono>();
            if (playerTeam)
            {
                alive.AddRange(GetPlayerTeamAlive());
            }
            else
            {
                alive.AddRange(GetEnemyTeamAlive());
            }
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < (alive.Count > number ? number : alive.Count); i++)
            {
                int r = Random.Range(0, alive.Count);
                HeroMono hero = alive[r];
                while (result.Contains(hero))
                {
                    r = Random.Range(0, alive.Count);
                    hero = alive[r];
                }
                result.Add(alive[r]);
            }
            return result;
        }

        /// <summary>
        /// 获取一定数量的死亡的目标
        /// </summary>
        /// <param name="playerTeam">是否从玩家队列中获取</param>
        /// <param name="number">获取的数量</param>
        /// <returns>返回目标列表</returns>
        List<HeroMono> GetDeadTarget(bool playerTeam, int number)
        {
            List<HeroMono> dead = new List<HeroMono>();
            if (playerTeam)
            {
                dead.AddRange(GetPlayerTeamDead());
            }
            else
            {
                dead.AddRange(GetEnemyTeamDead());
            }
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < (dead.Count > number ? number : dead.Count); i++)
            {
                int r = Random.Range(0, dead.Count);
                HeroMono hero = dead[r];
                while (result.Contains(hero))
                {
                    r = Random.Range(0, dead.Count);
                    hero = dead[r];
                }
                result.Add(dead[r]);
            }
            return result;
        }

        /// <summary>
        /// 获取一定数量的目标
        /// </summary>
        /// <param name="playerTeam">是否从玩家队列中获取</param>
        /// <param name="number">获取的数量</param>
        /// <returns>返回目标列表</returns>
        List<HeroMono> GetTarget(bool playerTeam, int number)
        {
            List<HeroMono> dead = new List<HeroMono>();
            if (playerTeam)
            {
                dead.AddRange(_PlayerTeam);
            }
            else
            {
                dead.AddRange(_EnemyTeam);
            }
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < (dead.Count > number ? number : dead.Count); i++)
            {
                int r = Random.Range(0, dead.Count);
                HeroMono hero = dead[r];
                while (result.Contains(hero))
                {
                    r = Random.Range(0, dead.Count);
                    hero = dead[r];
                }
                result.Add(dead[r]);
            }
            return result;
        }

        /// <summary>
        /// 检测玩家是否挑战失败
        /// </summary>
        /// <returns>如果玩家队列全部阵亡则返回true，否则返回false</returns>
        public bool CheckPlayerFailed()
        {
            for (int i = 0; i < _PlayerTeam.Count; i++)
            {
                if (!_PlayerTeam[i].IsDead())
                    return false;
            }
            return true;
        }


        /// <summary>
        /// 检测敌人是否挑战失败
        /// </summary>
        /// <returns>如果敌人队列全部阵亡则返回true，否则返回false</returns>
        public bool CheckEnemyFailed()
        {
            for (int i = 0; i < _EnemyTeam.Count; i++)
            {
                if (!_EnemyTeam[i].IsDead())
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 退出的时候清除调用
        /// </summary>
        public void Clear()
        {
            //清除玩家队列对象
            _PlayerTeam.Clear();
            //清除敌人队列对象
            _EnemyTeam.Clear();
            //清除行动列表
            _AttackList.Clear();
            //清除当前回合英雄引用
            _CurTurnHero = null;
            //清除BattleUI
            CurrentBattleUI.Clear();
            //清除技能控制器
            SkillController.Instance.Clear();
        }
    }
}