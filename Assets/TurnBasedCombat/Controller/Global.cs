using UnityEngine;
using System.Collections;

namespace King.TurnBasedCombat
{
    /// <summary>
    /// 各种枚举类的定义
    /// </summary>
    public class Global
    {
        /// <summary>
        /// 表格存储路径
        /// </summary>
        public static string TablePath = Application.dataPath + "/TurnBasedCombat/Table/";

        /// <summary>
        /// 星级等级的枚举，可以根据项目需求进行扩展，可用于英雄星级物品星级等
        /// </summary>
        public enum StarLevel
        {
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6
        }

        /// <summary>
        /// 战斗系统状态的枚举值
        /// </summary>
        public enum CombatSystemType
        {
            /// <summary>
            /// 初始化系统状态
            /// </summary>
            InitSystem = 0,
            /// <summary>
            /// 一个回合开始之前的状态
            /// </summary>
            BeforeAction = 1,
            /// <summary>
            /// 一个回合正在进行的状态
            /// </summary>
            Actioning = 2,
            /// <summary>
            /// 一个回合结束时候的状态
            /// </summary>
            AfterAction = 3,
            /// <summary>
            /// 一场战斗结束的状态
            /// </summary>
            CombatEnd = 4,
            /// <summary>
            /// 退出战斗系统的时候的状态
            /// </summary>
            ExitSystem = 5
        }

        /// <summary>
        /// Buff激活状态
        /// </summary>
        public enum BuffActiveState
        {
            /// <summary>
            /// 回合开始之前
            /// </summary>
            BeforeAction = 1,
            /// <summary>
            /// 回合进行中
            /// </summary>
            Actioning = 2,
            /// <summary>
            /// 回合结束之后
            /// </summary>
            AfterAction = 3,
            /// <summary>
            /// 死亡
            /// </summary>
            IsDead = 4,
            /// <summary>
            /// 生命值过低
            /// </summary>
            LifeLow = 5,
            /// <summary>
            /// 魔力值过低
            /// </summary>
            MagicLow = 6,
            /// <summary>
            /// 被攻击时
            /// </summary>
            Attacked = 7,
            /// <summary>
            /// 添加buff的时候
            /// </summary>
            AddBuff = 8,
            /// <summary>
            /// 移除buff的时候
            /// </summary>
            RemoveBuff = 9,
            /// <summary>
            /// 添加debuff的时候
            /// </summary>
            AddDebuff = 10,
            /// <summary>
            /// 移除debuff的时候
            /// </summary>
            RemoveDebuff = 11,
            /// <summary>
            /// buff生效的时候
            /// </summary>
            BuffAction = 12,
            /// <summary>
            /// debuff生效的时候
            /// </summary>
            DebuffAction = 13,
            /// <summary>
            /// 被控制的时候
            /// </summary>
            Controlled = 14,
        }

        /// <summary>
        /// 数值单位的枚举
        /// </summary>
        public enum UnitType
        {
            /// <summary>
            /// 单纯的数值
            /// </summary>
            Value = 0,
            /// <summary>
            /// 使用百分比的数值
            /// </summary>
            Percent = 1
        }

        /// <summary>
        /// 技能类型枚举，根据技能的效果或者实现方式区分
        /// </summary>
        public enum SkillType
        {
            /// <summary>
            /// 最基础的技能
            /// </summary>
            Base = 1,
            /// <summary>
            /// 最基础的物理攻击
            /// </summary>
            Physic = 2,
            /// <summary>
            /// 最基础的魔法攻击
            /// </summary>
            Magic = 3,
            /// <summary>
            /// 重生技能
            /// </summary>
            Respwan = 4,
            /// <summary>
            /// 多目标物理攻击技能
            /// </summary>
            MutiPhysic = 5,
            /// <summary>
            /// 多目标魔法攻击技能
            /// </summary>
            MutiMagic = 6,
        }

        /// <summary>
        /// 各种Buff Debuff的类型枚举
        /// </summary>
        public enum BuffType
        {
            /// <summary>
            /// 增加生命值的buff
            /// </summary>
            IncreaseLife = 1,
            /// <summary>
            /// 增加魔法值的buff
            /// </summary>
            IncreaseMagic = 2,
            /// <summary>
            /// 增加速度的buff
            /// </summary>
            IncreaseSpeed = 3,
            /// <summary>
            /// 增加物理攻击的buff
            /// </summary>
            IncreaseAttack = 4,
            /// <summary>
            /// 增加物理防御的buff
            /// </summary>
            IncreaseDefense = 5,
            /// <summary>
            /// 增加魔法攻击的buff
            /// </summary>
            IncreaseMagicAttack = 6,
            /// <summary>
            /// 增加魔法防御的buff
            /// </summary>
            IncreaseMagicDefense = 7,
            /// <summary>
            /// 增加回合数的buff
            /// </summary>
            ActiveTurn = 8,
            /// <summary>
            /// 增加最大生命值的buff
            /// </summary>
            IncreaseMaxLife = 9,
            /// <summary>
            /// 增加最大魔法值的buff
            /// </summary>
            IncreaseMaxMagic = 10,
            /// <summary>
            /// 不能添加buff
            /// </summary>
            CannotAddBuff = 11,
            /// <summary>
            /// 不能移除buff
            /// </summary>
            CannotRemoveBuff = 12,
            /// <summary>
            /// buff不生效
            /// </summary>
            CannotBuffAction = 13,
            /// <summary>
            /// 控制buff，可以使用其他阵营的英雄
            /// </summary>
            Control = 14,
            /// <summary>
            /// 跳过回合
            /// </summary>
            SkipTurn = 15,
        }

        /// <summary>
        /// Buff目标枚举
        /// </summary>
        public enum BuffTargetType
        {
            None = 0,
            /// <summary>
            /// 技能释放者
            /// </summary>
            Attacker = 1,
            /// <summary>
            /// 技能防御者
            /// </summary>
            Defenser = 2,
        }

        /// <summary>
        /// 技能目标的枚举类型
        /// </summary>
        public enum SkillTargetType
        {
            /// <summary>
            /// 没有目标
            /// </summary>
            None = 0,
            /// <summary>
            /// 活着的敌方阵营对象
            /// </summary>
            AliveEnemy = 1,
            /// <summary>
            /// 死亡的敌方阵营对象
            /// </summary>
            DeadEnemy = 2,
            /// <summary>
            /// 不论死活的敌方阵营对象
            /// </summary>
            Enemy = 3,
            /// <summary>
            /// 还活着的自己的阵营的对象
            /// </summary>
            AliveMine = 4,
            /// <summary>
            /// 死亡的自己的阵营的对象
            /// </summary>
            DeadMine = 5,
            /// <summary>
            /// 不论死活的自己的阵营的对象
            /// </summary>
            Mine = 6,
            /// <summary>
            /// 活着的友方的阵营的对象
            /// </summary>
            AliveFriendly = 7,
            /// <summary>
            /// 死亡的友方的阵营对象
            /// </summary>
            DeadFriendly = 8,
            /// <summary>
            /// 不论死活的友方的阵营对象
            /// </summary>
            Friendly = 9,
            /// <summary>
            /// 只对当前英雄生效
            /// </summary>
            Self = 10,
            /// <summary>
            /// 所有活着的英雄，不管阵营归属
            /// </summary>
            AllAlive = 11,
            /// <summary>
            /// 所有死亡的英雄，不管阵营归属
            /// </summary>
            AllDead = 12,
            /// <summary>
            /// 所有的不论死活的英雄不管阵营归属
            /// </summary>
            All = 13
        }
    }
}