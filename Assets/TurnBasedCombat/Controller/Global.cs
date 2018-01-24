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
        /// 英雄头像存储路径
        /// </summary>
        public static string ICOPath = Application.dataPath + "/TurnBasedCombat/Resources/ICO/";

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
            /// 随机一个敌人
            /// </summary>
            OneEnemy = 1,
            /// <summary>
            /// 随机两个敌人
            /// </summary>
            TwoEnemy = 2,
            /// <summary>
            /// 随机三个敌人
            /// </summary>
            ThreeEnemy = 3,
            /// <summary>
            /// 随机四个敌人
            /// </summary>
            FourEnemy = 4,
            /// <summary>
            /// 随机五个敌人
            /// </summary>
            FiveEnemy = 5,
            /// <summary>
            /// 全体敌人
            /// </summary>
            AllEnemy = 6,
            /// <summary>
            /// 只能是自身，即技能发起者
            /// </summary>
            Self = 7,
            /// <summary>
            /// 随机一个队友
            /// </summary>
            OneSelf = 8,
            /// <summary>
            /// 随机两个队友
            /// </summary>
            TwoSelf = 9,
            /// <summary>
            /// 随机三个队友
            /// </summary>
            ThreeSelf = 10,
            /// <summary>
            /// 随机四个队友
            /// </summary>
            FourSelf = 11,
            /// <summary>
            /// 随机五个队友
            /// </summary>
            FiveSelf = 12,
            /// <summary>
            /// 全体队友
            /// </summary>
            AllSelf = 13,
            /// <summary>
            /// 随机一个敌人
            /// </summary>
            OneEnemyWithDead = 14,
            /// <summary>
            /// 随机两个敌人
            /// </summary>
            TwoEnemyWithDead = 15,
            /// <summary>
            /// 随机三个敌人
            /// </summary>
            ThreeEnemyWithDead = 16,
            /// <summary>
            /// 随机四个敌人
            /// </summary>
            FourEnemyWithDead = 17,
            /// <summary>
            /// 随机五个敌人
            /// </summary>
            FiveEnemyWithDead = 18,
            /// <summary>
            /// 全体敌人
            /// </summary>
            AllEnemyWithDead = 19,
            /// <summary>
            /// 随机一个队友
            /// </summary>
            OneSelfWithDead = 20,
            /// <summary>
            /// 随机两个队友
            /// </summary>
            TwoSelfWithDead = 21,
            /// <summary>
            /// 随机三个队友
            /// </summary>
            ThreeSelfWithDead = 22,
            /// <summary>
            /// 随机四个队友
            /// </summary>
            FourSelfWithDead = 23,
            /// <summary>
            /// 随机五个队友
            /// </summary>
            FiveSelfWithDead = 24,
            /// <summary>
            /// 全体队友
            /// </summary>
            AllSelfWithDead = 25,
            /// <summary>
            /// 随机一个敌人
            /// </summary>
            OneDeadEnemy = 26,
            /// <summary>
            /// 随机两个敌人
            /// </summary>
            TwoDeadEnemy = 27,
            /// <summary>
            /// 随机三个敌人
            /// </summary>
            ThreeDeadEnemy = 28,
            /// <summary>
            /// 随机四个敌人
            /// </summary>
            FourDeadEnemy = 29,
            /// <summary>
            /// 随机五个敌人
            /// </summary>
            FiveDeadEnemy = 30,
            /// <summary>
            /// 全体敌人
            /// </summary>
            AllDeadEnemy = 31,
            /// <summary>
            /// 随机一个队友
            /// </summary>
            OneDeadSelf = 32,
            /// <summary>
            /// 随机两个队友
            /// </summary>
            TwoDeadSelf = 33,
            /// <summary>
            /// 随机三个队友
            /// </summary>
            ThreeDeadSelf = 34,
            /// <summary>
            /// 随机四个队友
            /// </summary>
            FourDeadSelf = 35,
            /// <summary>
            /// 随机五个队友
            /// </summary>
            FiveDeadSelf = 36,
            /// <summary>
            /// 全体队友
            /// </summary>
            AllDeadSelf = 37,
        }
    }
}