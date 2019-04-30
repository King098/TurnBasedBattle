using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace King.TurnBasedCombat
{
    /*[SkillDescription-Start]
    所有技能基础，可以加在技能特效，但是不使用技能特效
    可以使用技能消耗和暴击设置
    可以指定多个目标
    不使用英雄动画机制
    [SkillDescription-End]
     */

    /// <summary>
    /// 技能控制和显示的基类
    /// </summary>
    public class BaseSkill : MonoBehaviour
    {
        #region 各种私有变量定义
        /// <summary>
        /// 这个技能的数据模型
        /// </summary>
        protected Skill _Skill;
        /// <summary>
        /// 这个技能是属于哪个英雄对象
        /// </summary>
        protected HeroMono _Hero;
        /// <summary>
        /// 这个技能当前剩余的冷却回合数
        /// </summary>
        protected int _CDTurn;
        /// <summary>
        /// 这个技能剩余蓄力回合数
        /// </summary>
        protected int _DelayTurn;
        /// <summary>
        /// 当前技能执行的回合
        /// </summary>
        protected int _CurActionTurn;
        #endregion

        #region 各种属性get set方法
        /// <summary>
        /// 技能名字
        /// </summary>
        public string Name { get { return _Skill.Name; } }
        /// <summary>
        /// 技能类型
        /// </summary>
        public Global.SkillType SkillType { get { return _Skill.SkillType; } }
        /// <summary>
        /// 是否是被动技能
        /// </summary>
        public bool IsPassiveSkill { get { return _Skill.IsPassiveSkill; } }
        /// <summary>
        /// 技能发动的时机
        /// </summary>
        public Global.BuffActiveState ActiveState { get { return _Skill.ActiveState; } }
        /// <summary>
        /// 技能目标
        /// </summary>
        public Global.SkillTargetType TargetType { get { return _Skill.TargetType; } }
        /// <summary>
        /// 技能等级
        /// </summary>
        public int Level { get { return _Skill.Level; } }
        /// <summary>
        /// 发动技能将会消耗的各种英雄属性值变化
        /// </summary>
        public HeroProperty CostHeroProperty { get { return _Skill.CostHeroProperty; } }
        /// <summary>
        /// 当前剩余的冷却回合数
        /// </summary>
        public int CurrentCostTurn
        {
            get
            {
                return _CDTurn;
            }
            set
            {
                _CDTurn = value;
                if (_CDTurn < 0)
                {
                    _CDTurn = 0;
                }
            }
        }
        /// <summary>
        /// 技能蓄力回合数
        /// </summary>
        public int MaxDelayTurn { get { return _Skill.DelayTurn; } }
        /// <summary>
        /// 当前剩余的蓄力回合数
        /// </summary>
        public int CurrentDelayTurn
        {
            get
            {
                return _DelayTurn;
            }
            set
            {
                _DelayTurn = value;
                if (_DelayTurn < 0)
                {
                    _DelayTurn = 0;
                }
            }
        }
        /// <summary>
        /// 技能将会对目标造成的各种属性值的变化
        /// </summary>
        public HeroProperty CauseHeroProperty { get { return _Skill.CauseHeroProperty; } }
        /// <summary>
        /// 技能的物理攻击数值
        /// </summary>
        public long Attack
        {
            get
            {
                return _Skill.HurtHeroProperty.Attack.RealValue(_Hero.CurrentAttack);
            }
        }
        /// <summary>
        /// 技能的魔力攻击数值
        /// </summary>
        public long MagicAttack
        {
            get
            {
                return _Skill.HurtHeroProperty.MagicAttack.RealValue(_Hero.CurrentMagicAttack);
            }
        }
        /// <summary>
        /// 技能会产生的buff和debuff
        /// </summary>
        public List<Buff> SkillBuff { get { return _Skill.SkillBuff; } }
        /// <summary>
        /// 技能产生的伤害暴击几率
        /// </summary>
        public int CriticalChance { get { return _Skill.CriticalChance; } }
        /// <summary>
        /// 这个技能的描述信息
        /// </summary>
        public string Description { get { return _Skill.Description; } }
        /// <summary>
        /// 是否正在使用这个技能
        /// </summary>
        public bool IsUsingSkill { get; set; }
        #endregion

        /// <summary>
        /// 这个技能会使用到的特效
        /// </summary>
        public List<GameObject> SkillEffects = new List<GameObject>();

        /// <summary>
        /// 初始化这个技能数据
        /// </summary>
        /// <param name="skill">技能数据</param>
        /// <param name="hero">技能所属英雄数据</param>
        public virtual void Init(Skill skill, HeroMono hero)
        {
            _Skill = skill;
            _Hero = hero;
            for (int i = 0; i < skill.SkillEffectPaths.Count; i++)
            {
                if (i == skill.SkillEffectPaths.Count - 1)
                {
                    King.Tools.UnityStaticTool.LoadResources<GameObject>(skill.SkillEffectPaths[i], (obj) =>
                    {
                        SkillEffects.Add(obj);
                        //全部加载完成
                        //向技能控制其中添加技能所需要的特效对象
                        SkillController.Instance.RegisterSkillEffect(this);
                    });
                }
                else
                {
                    King.Tools.UnityStaticTool.LoadResources<GameObject>(skill.SkillEffectPaths[i], (obj) =>
                    {
                        SkillEffects.Add(obj);
                    });
                }
            }
        }

        /// <summary>
        /// 获取这个技能的ID
        /// </summary>
        public virtual string GetSkillID()
        {
            return _Skill.ID;
        }

        /// <summary>
        /// 获取这个技能的有等级
        /// </summary>
        public virtual int GetSkillLevel()
        {
            return _Skill.Level;
        }

        /// <summary>
        /// 获取这个技能的ID和等级
        /// </summary>
        public virtual KeyValuePair<string, int> GetSkillIDAndLevel()
        {
            return new KeyValuePair<string, int>(_Skill.ID, _Skill.Level);
        }

        /// <summary>
        /// 是否正在CD冷却中
        /// </summary>
        /// <returns>如果正在冷却中，返回true，否则返回false</returns>
        public virtual bool IsCostTurn()
        {
            if (CurrentCostTurn > 0)
                return true;
            return false;
        }

        /// <summary>
        /// 是否正在技能蓄力回合状态
        /// </summary>
        /// <returns>如果正在蓄力中则返回true，否则返回false</returns>
        public virtual bool IsDelayTurn()
        {
            if (CurrentDelayTurn > 0)
                return true;
            return false;
        }

        /// <summary>
        /// 当前是否能够使用这个技能
        /// </summary>
        /// <returns>是否可以使用这个技能</returns>
        public virtual bool CanUseSkill()
        {
            //是否没有冷却完毕
            if (IsCostTurn())
                return false;
            //是否正在蓄力中
            if (IsDelayTurn())
                return false;
            //是否需要消耗的数值够使用这个技能
            if (Mathf.Abs(CostHeroProperty.CurrentLife.RealTempValue(_Hero.CurrentMaxLife)) > _Hero.CurrentLife)
                return false;
            if (Mathf.Abs(CostHeroProperty.CurrentMagic.RealTempValue(_Hero.CurrentMaxMagic)) > _Hero.CurrentMagic)
                return false;
            if (CostHeroProperty.MaxLife.RealValue(_Hero.CurrentMaxLife) < 0)
                return false;
            if (CostHeroProperty.MaxMagic.RealValue(_Hero.CurrentMaxMagic) < 0)
                return false;
            //下列数值是不会有消耗的
            /*
            if (_Hero.CurrentAttack < CostHeroProperty.Attack.RealTempValue(_Hero.CurrentAttack))
                return false;
            if (_Hero.CurrentDefense < CostHeroProperty.Defense.RealTempValue(_Hero.CurrentDefense))
                return false;
            if (_Hero.CurrentMagicAttack < CostHeroProperty.MagicAttack.RealTempValue(_Hero.CurrentMagicAttack))
                return false;
            if (_Hero.CurrentMagicDefense < CostHeroProperty.MagicDefense.RealTempValue(_Hero.CurrentMagicDefense))
                return false;
            if (_Hero.CurrentSpeed < CostHeroProperty.Speed.RealTempValue(_Hero.CurrentSpeed))
                return false;
             * */
            return true;
        }

        /// <summary>
        /// 使用这个技能执行的效果
        /// </summary>
        public virtual void ExcuteSkill(List<HeroMono> targets)
        {
            IsUsingSkill = true;
            ExcutingSkill(targets);
            ExcutedSkill(targets);
        }

        /// <summary>
        /// 技能执行过程中
        /// </summary>
        public virtual void ExcutingSkill(List<HeroMono> targets)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].IsDead())
                    continue;
                targets[i].Defense(SkillType, _Hero);
            }
        }

        /// <summary>
        /// 技能执行完毕
        /// </summary>
        public virtual void ExcutedSkill(List<HeroMono> targets)
        {
            //如果当前执行的回合数不是当前回合数，则执行结束，否则不执行
            if (_CurActionTurn != BattleController.Instance.TurnNumber)
            {
                _CurActionTurn = BattleController.Instance.TurnNumber;
                IsUsingSkill = false;
                if (!IsPassiveSkill)
                {
                    BattleController.Instance.DebugLog(LogType.INFO,"Actioning");
                    //如果不是被动技能，则进入下一阶段，否则不处理
                    BattleController.Instance.ToActionState();
                }
            }
            StartCoroutine(SkillController.Instance.WaitForTime(_Skill.SkillEndDelay, () =>
                 {
                     //关闭技能特效
                     for (int i = 0; i < SkillController.Instance.SkillEffect[GetSkillIDAndLevel()].Count; i++)
                     {
                         SkillController.Instance.SkillEffect[GetSkillIDAndLevel()][i].gameObject.SetActive(false);
                     }
                 }));
        }


        /// <summary>
        /// 百分比几率预测
        /// </summary>
        public virtual bool IsInPercent(int percent)
        {
            int r = Random.Range(0, 100);
            if (r < percent)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 技能消耗
        /// </summary>
        public virtual void Cost()
        {
            _Hero.CurrentMaxLife = CostHeroProperty.MaxLife.RealValue(_Hero.CurrentMaxLife);
            _Hero.CurrentMaxMagic = CostHeroProperty.MaxMagic.RealValue(_Hero.CurrentMaxMagic);
            _Hero.CurrentLife = CostHeroProperty.CurrentLife.RealValue(_Hero.CurrentLife);
            _Hero.CurrentMagic = CostHeroProperty.CurrentMagic.RealValue(_Hero.CurrentMagic);
            //技能CD
            _CDTurn = CostHeroProperty.Turn;
            //其他的属性是不会有消耗的
        }

        /// <summary>
        /// 增加技能Buff或者Debuff
        /// </summary>
        public virtual void AddSkillBuff(HeroMono attacker, HeroMono defender)
        {
            for (int i = 0; i < _Skill.SkillBuff.Count; i++)
            {
                //表示buff或者debuff预测成功，则进行添加
                if (this.IsInPercent(_Skill.SkillBuff[i].SuccessChance))
                {
                    BattleController.Instance.DebugLog(LogType.INFO,"添加一个buff");
                    if (_Skill.SkillBuff[i].TargetType == Global.BuffTargetType.Attacker)
                    {
                        attacker.AddBuff(_Skill.SkillBuff[i]);
                    }
                    else if(_Skill.SkillBuff[i].TargetType == Global.BuffTargetType.Defenser)
                    {
                        defender.AddBuff(_Skill.SkillBuff[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 技能防御
        /// </summary>
        public virtual void DefenseSkill(HeroMono attacker, HeroMono defender)
        {
            long hurt = 0;
            //完全物理攻击，没有任何添加
            hurt = attacker.CurrentAttack + this.Attack - defender.CurrentDefense;
            defender.CurrentLife -= hurt;
            //播放动画
            defender.PlayTriggerAnimation(HeroAnimation.MagicDefense1);
            if (!defender.HasLife())
            {
                defender.ExcuteBuff(Global.BuffActiveState.IsDead);
                defender.ExcuteSkill(Global.BuffActiveState.IsDead);
            }
            else
            {
                //如果是物理攻击防御技能则发动
                defender.ExcuteBuff(Global.BuffActiveState.Attacked);
                defender.ExcuteSkill(Global.BuffActiveState.Attacked);
            }
            //显示HeroHub
            if (hurt > 0)
            {
                defender.ShowHeroHub(HubType.DecreseLife, new ValueUnit(hurt, Global.UnitType.Value));
            }
            else if (hurt == 0)
            {
                defender.ShowHeroHub(HubType.Miss);
            }
            //进行buff判断
            AddSkillBuff(attacker,defender);
            Debug.Log((attacker.IsPlayerHero ? "玩家的" : "敌人的") + attacker.Name + "使用技能" + this.Name + "对" + defender.Name + "造成" + hurt + "点伤害");
        }

        /// <summary>
        /// 设置技能特效
        /// </summary>
        public virtual void SetSkillEffect(HeroMono attacker, List<HeroMono> targets, System.Action callback)
        {

        }
    }
}