using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using King.Tools;

namespace King.TurnBasedCombat
{
    public enum HeroAnimation
    {
        None = 0,
        Idle = 1,
        Walk = 2,
        Attack1 = 3,
        Attack2 = 4,
        Attack3 = 5,
        Attack4 = 6,
        MagicAttack1 = 8,
        MagicAttack2 = 9,
        MagicAttack3 = 10,
        MagicAttack4 = 11,
        Defense1 = 12,
        Defense2 = 13,
        Defense3 = 14,
        Defense4 = 15,
        MagicDefense1 = 16,
        MagicDefense2 = 17,
        MagicDefense3 = 18,
        MagicDefense4 = 19,
        Dead = 20,
    }
    /// <summary>
    /// 英雄的Mono类，记录数据和战斗状态
    /// </summary>
    public class HeroMono : MonoBehaviour
    {
        /// <summary>
        /// 英雄的数据
        /// </summary>
        private Hero _Hero;
        public Hero Hero
        {
            get
            {
                return _Hero;
            }
        }

        /// <summary>
        /// 战斗过程中的临时数据(所有数值单位都为准确数值，不能为百分比)
        /// </summary>
        private HeroProperty _CurrentHero;

        /// <summary>
        /// 当前英雄的技能，Mono类的
        /// </summary>
        private Dictionary<Global.SkillType, BaseSkill> _Skills;
        public Dictionary<Global.SkillType, BaseSkill> Skills
        {
            get
            {
                return _Skills;
            }
        }
        /// <summary>
        /// 增益状态存储列表
        /// </summary>
        private List<Buff> _BuffList;
        /// <summary>
        /// 负面状态存储列表
        /// </summary>
        private List<Buff> _DebuffList;

        public string ID { get { return _Hero.ID; } }
        public string Name
        {
            get
            {
                //可以根据情况返回Name或者RealName
                return _Hero.Name;
            }
        }
        public int Level { get { return _Hero.Level; } }
        public Global.StarLevel StarLevel { get { return _Hero.StarLevel; } }
        public long CurrentMaxLife
        {
            get
            {
                return _CurrentHero.MaxLife.Value;
            }
            set
            {
                ValueUnit old_value = _CurrentHero.MaxLife;
                _CurrentHero.MaxLife.Value = value;
                if (_CurrentHero.MaxLife.Value < 0)
                {
                    _CurrentHero.MaxLife.Value = 0;
                }
                //TODO 刷新UI
                BattleController.Instance.CurrentBattleUI.RefreshUI(this, Global.BuffType.IncreaseMaxLife, old_value, _CurrentHero.MaxLife);
            }
        }
        public long CurrentLife
        {
            get
            {
                return _CurrentHero.CurrentLife.Value;
            }
            set
            {
                ValueUnit old_value = _CurrentHero.CurrentLife;
                _CurrentHero.CurrentLife.Value = value;
                if (_CurrentHero.CurrentLife.Value < 0)
                {
                    _CurrentHero.CurrentLife.Value = 0;
                }
                else if (_CurrentHero.CurrentLife.Value > _CurrentHero.MaxLife.Value)
                {
                    _CurrentHero.CurrentLife.Value = _CurrentHero.MaxLife.Value;
                }
                if (_CurrentHero.CurrentLife.Value == 0)
                {
                    //血量增加后
                    IsDeath = false;
                    if(!HasIsDeadSkill())
                    {
                        //如果血量为0并且没有死亡触发技能，则死亡
                        OnDead();
                    }
                }
                //生命值过低时候触发的调用
                if (_CurrentHero.CurrentLife.Value > 0 && _CurrentHero.CurrentLife.Value <= _CurrentHero.CurrentLife.Value * SystemSetting.HeroLowLifePercent)
                {
                    ExcuteBuff(Global.BuffActiveState.LifeLow);
                    ExcuteSkill(Global.BuffActiveState.LifeLow);
                }
                //TODO 刷新UI上的数值变化
                BattleController.Instance.CurrentBattleUI.RefreshUI(this, Global.BuffType.IncreaseLife, old_value, _CurrentHero.CurrentLife);
            }
        }
        public long CurrentMaxMagic
        {
            get
            {
                return _CurrentHero.MaxMagic.Value;
            }
            set
            {
                ValueUnit old_value = _CurrentHero.MaxMagic;
                _CurrentHero.MaxMagic.Value = value;
                if (_CurrentHero.MaxMagic.Value < 0)
                {
                    _CurrentHero.MaxMagic.Value = 0;
                }
                //TODO 刷新UI数据
                BattleController.Instance.CurrentBattleUI.RefreshUI(this, Global.BuffType.IncreaseMaxMagic, old_value, _CurrentHero.MaxMagic);
            }
        }
        public long CurrentMagic
        {
            get
            {
                return _CurrentHero.CurrentMagic.Value;
            }
            set
            {
                ValueUnit old_value = _CurrentHero.CurrentMagic;
                _CurrentHero.CurrentMagic.Value = value;
                if (_CurrentHero.CurrentMagic.Value < 0)
                {
                    _CurrentHero.CurrentMagic.Value = 0;
                }
                else if (_CurrentHero.CurrentMagic.Value > _CurrentHero.MaxMagic.Value)
                {
                    _CurrentHero.CurrentMagic.Value = _CurrentHero.MaxMagic.Value;
                }
                //魔力值过低时候触发的调用
                if (_CurrentHero.CurrentMagic.Value > 0 && _CurrentHero.CurrentMagic.Value <= _CurrentHero.CurrentMagic.Value * SystemSetting.HeroLowMagicPercent)
                {
                    ExcuteBuff(Global.BuffActiveState.MagicLow);
                    ExcuteSkill(Global.BuffActiveState.MagicLow);
                }
                //TODO 刷新UI上的数值变化
                BattleController.Instance.CurrentBattleUI.RefreshUI(this, Global.BuffType.IncreaseMagic, old_value, _CurrentHero.CurrentMagic);
            }
        }
        public long CurrentAttack
        {
            get
            {
                return _CurrentHero.Attack.Value;
            }
            set
            {
                ValueUnit old_value = _CurrentHero.Attack;
                _CurrentHero.Attack.Value = value;
                if (_CurrentHero.Attack.Value < 0)
                {
                    _CurrentHero.Attack.Value = 0;
                }
                //TODO 刷新UI上的数值变化
                BattleController.Instance.CurrentBattleUI.RefreshUI(this, Global.BuffType.IncreaseAttack, old_value, _CurrentHero.Attack);
            }
        }
        public long CurrentDefense
        {
            get
            {
                return _CurrentHero.Defense.Value;
            }
            set
            {
                ValueUnit old_value = _CurrentHero.Defense;
                _CurrentHero.Defense.Value = value;
                if (_CurrentHero.Defense.Value < 0)
                {
                    _CurrentHero.Defense.Value = 0;
                }
                //TODO 刷新UI上的数值变化
                BattleController.Instance.CurrentBattleUI.RefreshUI(this, Global.BuffType.IncreaseDefense, old_value, _CurrentHero.Defense);
            }
        }
        public long CurrentMagicAttack
        {
            get
            {
                return _CurrentHero.MagicAttack.Value;
            }
            set
            {
                ValueUnit old_value = _CurrentHero.MagicAttack;
                _CurrentHero.MagicAttack.Value = value;
                if (_CurrentHero.MagicAttack.Value < 0)
                {
                    _CurrentHero.MagicAttack.Value = 0;
                }
                //TODO 刷新UI上的数值变化
                BattleController.Instance.CurrentBattleUI.RefreshUI(this, Global.BuffType.IncreaseMagicAttack, old_value, _CurrentHero.MagicAttack);
            }
        }
        public long CurrentMagicDefense
        {
            get
            {
                return _CurrentHero.MagicDefense.Value;
            }
            set
            {
                ValueUnit old_value = _CurrentHero.MagicDefense;
                _CurrentHero.MagicDefense.Value = value;
                if (_CurrentHero.MagicDefense.Value < 0)
                {
                    _CurrentHero.MagicDefense.Value = 0;
                }
                //TODO 刷新UI上的数值变化
                BattleController.Instance.CurrentBattleUI.RefreshUI(this, Global.BuffType.IncreaseMagicDefense, old_value, _CurrentHero.MagicDefense);
            }
        }
        public long CurrentSpeed
        {
            get
            {
                return _CurrentHero.Speed.Value;
            }
            set
            {
                ValueUnit old_value = _CurrentHero.Speed;
                _CurrentHero.Speed.Value = value;
                if (_CurrentHero.Speed.Value < 0)
                {
                    _CurrentHero.Speed.Value = 0;
                }
                //TODO 刷新UI上的数值变化
                BattleController.Instance.CurrentBattleUI.RefreshUI(this, Global.BuffType.IncreaseSpeed, old_value, _CurrentHero.Speed);
            }
        }
        public int CurrentTurnLast
        {
            get
            {
                return _CurrentHero.Turn;
            }
            set
            {
                if (_CurrentHero.Turn != 0)
                {
                    _CurrentHero.Turn = value;
                }
                else
                {
                    _CurrentHero.Turn = 0;
                }
            }
        }
        /// <summary>
        /// 这个英雄的描述信息
        /// </summary>
        public string Description { get { return _Hero.Description; } }
        /// <summary>
        /// 当前这个英雄技能的目标
        /// </summary>
        public List<HeroMono> CurrentTargets { get; set; }
        /// <summary>
        /// 是否是玩家的所属英雄
        /// </summary>
        public bool IsPlayerHero { get; set; }
        /// <summary>
        /// 这个英雄当前所在的位置
        /// </summary>
        public Vector3 HeroPosition { get; set; }
        /// <summary>
        /// 是否死亡
        /// </summary>
        public bool IsDeath { get; set; }


        #region 面板属性定义

        /// <summary>
        /// 动画控制器
        /// </summary>
        public Animator Animator { get; set; }
        /// <summary>
        /// 用于显示英雄图像的对象
        /// </summary>
        public Image Img { get; set; }

        /// <summary>
        /// 英雄的各种战斗提示预制体
        /// </summary>
        public GameObject HeroHubPrefab;
        /// <summary>
        /// 被攻击的时候的站位
        /// </summary>
        public Transform AttackPosition;
        #endregion

        public void Init(Hero hero, bool isPlayer)
        {
            _Hero = hero;
            //获取动画控制器
            Animator = GetComponent<Animator>();
            //获取英雄的图像
            Img = GetComponentInChildren<Image>();
            CurrentTargets = new List<HeroMono>();
            IsPlayerHero = isPlayer;
            //翻转对象
            if (IsPlayerHero)
            {
                //AttackPosition.localPosition = new Vector3(AttackPosition.localPosition.x * -1f, AttackPosition.localPosition.y, AttackPosition.localPosition.z);
                Img.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else
            {
                //AttackPosition.localPosition = new Vector3(AttackPosition.localPosition.x * -1f, AttackPosition.localPosition.y, AttackPosition.localPosition.z);
                Img.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            //初始化buff debuff列表
            _BuffList = new List<Buff>();
            _DebuffList = new List<Buff>();
            //初始化数据
            _CurrentHero = new HeroProperty(_Hero, SystemSetting.HeroSpeedMix);
            //创建一个血量和提示变化用的对象池
            ObjectPool.Instance.InitComponentPools<BattleHeroHub>(Name + "_Hub", HeroHubPrefab, 1);
            //注册这个英雄的UI信息
            if (IsPlayerHero)
            {
                BattleController.Instance.CurrentBattleUI.RegisterPlayerHero(this);
            }
            else
            {
                BattleController.Instance.CurrentBattleUI.RegisterEnemyHero(this);
            }
            //向SkillController注册技能
            _Skills = new Dictionary<Global.SkillType, BaseSkill>();
            for (int i = 0; i < _Hero.Skills.Count; i++)
            {
                BaseSkill skill = SkillController.Instance.CreateSkillTo(_Hero.Skills[i], this);
                if (skill != null)
                {
                    _Skills.Add(_Hero.Skills[i].SkillType, skill);
                }
            }
        }

        /// <summary>
        /// 使用一个技能去攻击
        /// </summary>
        /// <param name="skill">使用的技能</param>
        /// <param name="isDelay">是否要蓄力，默认不需要</param>
        public void Attack(Global.SkillType skill)
        {
            //判断技能是否可以释放
            if (!_Skills[skill].CanUseSkill())
            {
                _Skills[skill].ExcutedSkill(null);
                return;
            }
            if (_Skills.ContainsKey(skill))
            {
                //检查技能是否正在蓄力
                if (_Skills[skill].IsDelayTurn())
                {
                    _Skills[skill].ExcutedSkill(null);
                }
                else
                {
                    //首先去除技能的消耗
                    _Skills[skill].Cost();
                    //检查技能是否需要蓄力
                    if (_Skills[skill].MaxDelayTurn > 0)
                    {
                        _Skills[skill].CurrentDelayTurn = _Skills[skill].MaxDelayTurn;
                        _Skills[skill].ExcutedSkill(null);
                    }
                    else
                    {
                        _Skills[skill].ExcuteSkill(BattleController.Instance.SkillTarget(this, _Skills[skill].TargetType));
                    }
                }
            }
        }

        /// <summary>
        /// 不做任何判断直接执行一个技能
        /// </summary>
        /// <param name="skill"></param>
        public void ExcuteSkill(Global.SkillType skill)
        {
            _Skills[skill].ExcuteSkill(BattleController.Instance.SkillTarget(this, _Skills[skill].TargetType));
        }

        /// <summary>
        /// 被一个英雄使用一个技能攻击之后的防守
        /// </summary>
        /// <param name="skill">使用的技能</param>
        /// <param name="attacker">攻击者</param>
        public void Defense(Global.SkillType skill, HeroMono attacker)
        {
            //防御的时候是判断技能属于物理还是防御，然后叠加，然后检测是否有技能对此攻击技能有影响，还有buff的影响
            SkillController.Instance.DefenseSkill(attacker, this, attacker._Skills[skill]);
        }

        /// <summary>
        /// 冷却技能CD
        /// </summary>
        /// <param name="turn">冷却回合数</param>
        public void CoolDownSkillCD(int turn = 1)
        {
            foreach (BaseSkill skill in _Skills.Values)
            {
                if (skill.IsCostTurn())
                {
                    skill.CurrentCostTurn -= turn;
                    BattleController.Instance.DebugLog(LogType.INFO,skill.Name + "冷却剩余回合 : " + skill.CurrentCostTurn);
                }
            }
        }

        /// <summary>
        /// 冷却技能蓄力回合数
        /// </summary>
        /// <param name="turn">冷却回合数</param>
        /// <returns>如果蓄力完毕，则返回这个技能，否则返回null</returns>
        public BaseSkill CoolDownSkillDelay(int turn = 1)
        {
            foreach (BaseSkill skill in _Skills.Values)
            {
                if (skill.CurrentDelayTurn > 0)
                {
                    skill.CurrentDelayTurn -= turn;
                    BattleController.Instance.DebugLog(LogType.INFO,skill.Name + "蓄力剩余回合 : " + skill.CurrentDelayTurn);
                    return skill;
                }
            }
            return null;
        }

        /// <summary>
        /// 在特定状态下执行一个技能
        /// </summary>
        /// <param name="activeState">特定的状态</param>
        /// <returns>如果有技能发动，则返回true，否则返回false</returns>
        public bool ExcuteSkill(Global.BuffActiveState activeState)
        {
            if (IsDead())
            {
                if (activeState != Global.BuffActiveState.IsDead)
                {
                    OnDead();
                    return false;
                }
                if (!HasIsDeadSkill())
                {
                    OnDead();
                    return false;
                }
            }
            bool result = false;
            foreach (BaseSkill skill in _Skills.Values)
            {
                if (skill.IsPassiveSkill && skill.ActiveState == activeState)
                {
                    Attack(skill.SkillType);
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 当前英雄是否已经死亡
        /// </summary>
        /// <returns></returns>
        public bool IsDead()
        {
            if (IsDeath)
                return true;
            if (CurrentLife <= 0 && !HasIsDeadSkill())
                return true;
            return false;
        }

        /// <summary>
        /// 是否还有生命值
        /// </summary>
        /// <returns>如果有生命值则返回true，否则返回false</returns>
        public bool HasLife()
        {
            if (CurrentLife <= 0)
                return false;
            return true;
        }


        /// <summary>
        /// 是否有死亡之后发动的技能
        /// </summary>
        /// <returns>如果有死亡后发动的技能则返回true，否则返回false</returns>
        public bool HasIsDeadSkill()
        {
            foreach (BaseSkill skill in Skills.Values)
            {
                //如果有死亡时候发动的技能并且可以发动，则表示有技能可以使用
                if (skill.ActiveState == Global.BuffActiveState.IsDead && (skill.CanUseSkill() || skill.IsUsingSkill))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否正在使用某个技能
        /// </summary>
        /// <returns></returns>
        public bool IsUsingSkill()
        {
            foreach (BaseSkill skill in Skills.Values)
            {
                if (skill.IsUsingSkill)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 当前英雄可用的技能列表
        /// </summary>
        /// <returns>返回可用技能的技能类别列表</returns>
        public List<Global.SkillType> GetUseableSkills()
        {
            List<Global.SkillType> result = new List<Global.SkillType>();
            foreach (Global.SkillType type in _Skills.Keys)
            {
                if (_Skills[type].CanUseSkill() && !_Skills[type].IsPassiveSkill)
                {
                    result.Add(type);
                }
            }
            return result;
        }

        /// <summary>
        /// 自动执行的技能不能执行被动技能，只能使用主动技能
        /// </summary>
        public void AutoExcuteSkill()
        {
            if (IsDead())
                return;
            List<Global.SkillType> skills = new List<Global.SkillType>();
            skills.AddRange(GetUseableSkills());
            Attack(skills[Random.Range(0, skills.Count)]);
        }

        /// <summary>
        /// 检测buff或者debuff是否存在
        /// </summary>
        public Buff CheckBuffExist(List<Buff> buff_list, Buff buff)
        {
            for (int i = 0; i < buff_list.Count; i++)
            {
                if (buff_list[i].ID == buff.ID)
                {
                    return buff_list[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 给英雄添加一个增益或者负面状态
        /// </summary>
        /// <param name="buff">状态</param>
        public void AddBuff(Buff buff)
        {
            if (buff.IsBuff)
            {
                if (CheckBuffExist(_BuffList, buff) == null)
                {
                    buff.Init();
                    _BuffList.Add(buff);
                }
                else
                {
                    buff = CheckBuffExist(_BuffList, buff);
                    _BuffList.Remove(buff);
                    buff.Init();
                    _BuffList.Add(buff);
                }
            }
            else
            {
                if (CheckBuffExist(_DebuffList, buff) == null)
                {
                    buff.Init();
                    _DebuffList.Add(buff);
                }
                else
                {
                    buff = CheckBuffExist(_DebuffList, buff);
                    _DebuffList.Remove(buff);
                    buff.Init();
                    _DebuffList.Add(buff);
                }
            }
            BattleController.Instance.CurrentBattleUI.OnAddBuff(this, buff);
        }

        /// <summary>
        /// 移出一个增益或者负面状态
        /// </summary>
        /// <param name="buff">状态</param>
        public void RemoveBuff(Buff buff)
        {
            if (buff.IsBuff)
            {
                _BuffList.Remove(buff);
            }
            else
            {
                _DebuffList.Remove(buff);
            }
            BattleController.Instance.CurrentBattleUI.OnRemoveBuff(this, buff);
        }

        /// <summary>
        /// 执行一遍某个状态会激活得buff
        /// </summary>
        /// <param name="buffState">执行的状态</param>
        public void ExcuteBuff(Global.BuffActiveState buffState)
        {
            if (IsDead() && buffState != Global.BuffActiveState.IsDead)
                return;
            //执行增益状态
            for (int i = 0; i < _BuffList.Count; i++)
            {
                if (buffState == _BuffList[i].BuffActiveState)
                {
                    SkillController.Instance.ExcuteBuff(this, _BuffList[i]);
                    BuffTurnCost(_BuffList[i]);
                    BattleController.Instance.CurrentBattleUI.OnBuffAction(this, _BuffList[i]);
                    if (_BuffList[i].StayTurn <= 0)
                    {
                        //表示这个buff已经结束了，需要移除
                        RemoveBuff(_BuffList[i]);
                    }
                }
            }
            //执行负面状态
            for (int i = 0; i < _DebuffList.Count; i++)
            {
                if (buffState == _DebuffList[i].BuffActiveState)
                {
                    SkillController.Instance.ExcuteBuff(this, _DebuffList[i]);
                    BuffTurnCost(_DebuffList[i]);
                    BattleController.Instance.CurrentBattleUI.OnBuffAction(this, _DebuffList[i]);
                    if (_DebuffList[i].StayTurn <= 0)
                    {
                        //表示这个buff已经结束了，需要移除
                        RemoveBuff(_DebuffList[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 每回合buff或者debuff执行的时候将会调用
        /// </summary>
        public void BuffTurnCost(Buff buff)
        {
            if (IsDead())
                return;
            if (buff.IsBuff)
            {
                for (int i = 0; i < _BuffList.Count; i++)
                {
                    if (_BuffList[i].ID == buff.ID)
                    {
                        _BuffList[i].StayTurn -= 1;
                        buff = _BuffList[i];
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _DebuffList.Count; i++)
                {
                    if (_DebuffList[i].ID == buff.ID)
                    {
                        _DebuffList[i].StayTurn -= 1;
                        buff = _DebuffList[i];
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// 清除所有增益状态
        /// </summary>
        public void ClearBuff()
        {
            for (int i = 0; i < _BuffList.Count; i++)
            {
                BattleController.Instance.CurrentBattleUI.OnRemoveBuff(this, _BuffList[i]);
            }
            _BuffList.Clear();
        }


        /// <summary>
        /// 清除所有负面状态
        /// </summary>
        public void ClearDebuff()
        {
            for (int i = 0; i < _DebuffList.Count; i++)
            {
                BattleController.Instance.CurrentBattleUI.OnRemoveBuff(this, _DebuffList[i]);
            }
            _DebuffList.Clear();
        }

        /// <summary>
        /// 清除所有的状态
        /// </summary>
        public void ClearAllBuff()
        {
            ClearBuff();
            ClearDebuff();
        }


        /// <summary>
        /// 显示一个英雄Hub
        /// </summary>
        /// <param name="type">Hub类型</param>
        public void ShowHeroHub(HubType type, ValueUnit value = null)
        {
            BaseBattleHeroHub hub = ObjectPool.Instance.GetComponentFromPool<BaseBattleHeroHub>(Name + "_Hub");
            if (hub != null)
            {
                hub.ShowHubInfo(this, type, value);
            }
        }

        /// <summary>
        /// 当英雄死亡的时候触发
        /// </summary>
        public void OnDead()
        {
            if (!IsDeath)
            {
                IsDeath = true;
                BattleController.Instance.CurrentBattleUI.HeroDead(this);
            }
        }


        public void PlayTriggerAnimation(HeroAnimation animation)
        {
            AnimatorControllerParameter[] param = Animator.parameters;
            for (int i = 0; i < param.Length; i++)
            {
                if (param[i].type == AnimatorControllerParameterType.Trigger && SystemSetting.HeroAnimationParameters[animation] == param[i].name)
                {
                    Animator.SetTrigger(param[i].name);
                    return;
                }
            }
            BattleController.Instance.DebugLog(LogType.ERROR,"Not found hero:" + Name + " trigger animation:" + SystemSetting.HeroAnimationParameters[animation]);
        }

        public void PlayIntAnimation(HeroAnimation animation, int value)
        {
            AnimatorControllerParameter[] param = Animator.parameters;
            for (int i = 0; i < param.Length; i++)
            {
                if (param[i].type == AnimatorControllerParameterType.Int && SystemSetting.HeroAnimationParameters[animation] == param[i].name)
                {
                    Animator.SetInteger(param[i].name, value);
                    return;
                }
            }
            BattleController.Instance.DebugLog(LogType.ERROR,"Not found hero:" + Name + " integer animation:" + SystemSetting.HeroAnimationParameters[animation]);
        }

        public void PlayFloatAnimation(HeroAnimation animation, float value)
        {
            AnimatorControllerParameter[] param = Animator.parameters;
            for (int i = 0; i < param.Length; i++)
            {
                if (param[i].type == AnimatorControllerParameterType.Float && SystemSetting.HeroAnimationParameters[animation] == param[i].name)
                {
                    Animator.SetFloat(param[i].name, value);
                    return;
                }
            }
            BattleController.Instance.DebugLog(LogType.ERROR,"Not found hero:" + Name + " float animation:" + SystemSetting.HeroAnimationParameters[animation]);
        }

        public void PlayBoolAnimation(HeroAnimation animation, bool value)
        {
            AnimatorControllerParameter[] param = Animator.parameters;
            for (int i = 0; i < param.Length; i++)
            {
                if (param[i].type == AnimatorControllerParameterType.Float && SystemSetting.HeroAnimationParameters[animation] == param[i].name)
                {
                    Animator.SetBool(param[i].name, value);
                    return;
                }
            }
            BattleController.Instance.DebugLog(LogType.ERROR,"Not found hero:" + Name + " float animation:" + SystemSetting.HeroAnimationParameters[animation]);
        }
    }
}