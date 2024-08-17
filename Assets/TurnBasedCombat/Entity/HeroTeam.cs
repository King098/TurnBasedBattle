using System;
using System.Collections.Generic;

namespace King.TurnBasedCombat
{
    public class HeroTeam
    {
        /// <summary>
        /// 默认定义自己的阵营组
        /// </summary>
        public const string MineTeamGroup = "MineTeam";
        /// <summary>
        /// 默认定义敌方的阵营组
        /// </summary>
        public const string EnemyTeamGroup = "EnemyTeam";
        /// <summary>
        /// 一个队伍的所有上阵的英雄
        /// </summary>
        public List<Hero> Heroes = new List<Hero>();
        /// <summary>
        /// 队伍所属的阵营，如果两个队伍阵营一致，表示是友方，否则是敌方
        /// </summary>
        public string TeamGroup = "";
        /// <summary>
        /// 队伍在场上所占的位置索引
        /// </summary>
        public int TeamIndex = 0;
        /// <summary>
        /// 区分队伍是谁在控制的
        /// </summary>
        public HeroTeamType TeamType = HeroTeamType.Mine;

        public HeroTeam(List<Hero> heroes,HeroTeamType type, int teamIndex,string teamGroup)
        {
            Heroes.Clear();
            Heroes.AddRange(heroes);
            TeamType = type;
            TeamIndex = teamIndex;
            TeamGroup = teamGroup;
        }
    }
}