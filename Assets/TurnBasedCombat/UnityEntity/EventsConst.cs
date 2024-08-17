using System;
using System.Collections.Generic;

namespace King.TurnBasedCombat
{
    public class EventsConst
    {
        /// <summary>
        /// 当战斗开始的时候
        /// </summary>
        public const string OnCombatBattleStart = nameof(OnCombatBattleStart);
        /// <summary>
        /// 当战斗结束的时候
        /// </summary>
        public const string OnCombatBattleEnd = nameof(OnCombatBattleEnd);
        /// <summary>
        /// 当战斗系统初始化完成的时候
        /// </summary>
        public const string OnInitSystem = nameof(OnInitSystem);
        /// <summary>
        /// 每个回合开始的时候
        /// </summary>
        public const string OnBeforeAction = nameof(OnBeforeAction);
        /// <summary>
        /// 每个回合执行的时候
        /// </summary>
        public const string OnActioning = nameof(OnActioning);
        /// <summary>
        /// 每个回合执行之后
        /// </summary>
        public const string OnAfterAction = nameof(OnAfterAction);
        /// <summary>
        /// 当退出战斗系统的时候
        /// </summary>
        public const string OnExitSystem = nameof(OnExitSystem);
        /// <summary>
        /// 内部使用的事件，完成数据准备的初始化
        /// </summary>
        public const string OnInternalInitSystem = nameof(OnInternalInitSystem);    
        /// <summary>
        /// 等待玩家输入的事件广播
        /// </summary>
        public const string OnWaitingPlayerInput = nameof(OnWaitingPlayerInput);
        /// <summary>
        /// 当玩家属性改变的时候
        /// </summary>
        public const string OnHeroPropertyChanged = nameof(OnHeroPropertyChanged);
        /// <summary>
        /// 当英雄添加buff的时候
        /// </summary>
        public const string OnAddBuff = nameof(OnAddBuff);
        /// <summary>
        /// 当英雄移除buff的时候
        /// </summary>
        public const string OnRemoveBuff = nameof(OnRemoveBuff);
        /// <summary>
        /// 当buff执行的时候
        /// </summary>
        public const string OnBuffAction = nameof(OnBuffAction);
        /// <summary>
        /// 当英雄死亡的时候
        /// </summary>
        public const string OnHeroDead = nameof(OnHeroDead);
        /// <summary>
        /// 当英雄被控的时候
        /// </summary>  
        public const string OnHeroMonoControlled = nameof(OnHeroMonoControlled);
        /// <summary>
        /// 当目标英雄被选中的时候
        /// </summary>
        public const string OnHeroTargetChoosed = nameof(OnHeroTargetChoosed);

        /// <summary>
        /// 系统内部分发的战斗开始的事件
        /// </summary>
        public class CombatBattleStartEventArgs : EventArgs
        {
            public List<HeroTeamMono> teams;
            public CombatBattleStartEventArgs(List<HeroTeamMono> teams)
            {
                this.teams = new List<HeroTeamMono>();
                this.teams = teams;
            }
        }

        /// <summary>
        /// 系统内部初始化完成的事件
        /// </summary>
        public class CombatBattleInternalInitedEventArgs : EventArgs
        {
            public List<HeroTeam> teams;
            public CombatBattleInternalInitedEventArgs(List<HeroTeam> teams)
            {
                this.teams = new List<HeroTeam>();
                this.teams = teams;
            }
        }

        /// <summary>
        /// 通用的英雄事件数据
        /// </summary>
        public class CommonHeroMonoEventArgs : EventArgs
        {
            public HeroMono hero;
            public CommonHeroMonoEventArgs(HeroMono hero)
            {
                this.hero = hero;
            }
        }

        /// <summary>
        /// 玩家属性变化事件
        /// </summary>
        public class HeroPropertyEventArgs : EventArgs
        {
            public HeroMono hero;
            public Global.BuffType type;
            public ValueUnit oldValue;
            public ValueUnit newValue;
            public HeroPropertyEventArgs(HeroMono hero,Global.BuffType type,ValueUnit oldVal,ValueUnit newVal)
            {
                this.hero = hero;
                this.type = type;
                this.oldValue = oldVal;
                this.newValue = newVal;
            }
        }

        /// <summary>
        /// 通用的英雄buff事件
        /// </summary>
        public class CommonHeroBuffEventArgs : EventArgs
        {
            public HeroMono hero;
            public Buff buff;

            public CommonHeroBuffEventArgs(HeroMono hero, Buff buff)
            {
                this.hero = hero;
                this.buff = buff;
            }
        }

        /// <summary>
        /// 通用的战斗胜利的事件
        /// </summary>
        public class CommonCombatEndEventArgs : EventArgs
        {
            public string winTeamGroup;
            public CommonCombatEndEventArgs(string winTeamGroup)
            {
                this.winTeamGroup = winTeamGroup;
            }
        }
    }
}