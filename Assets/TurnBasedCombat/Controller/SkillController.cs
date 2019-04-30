using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using King.Tools;


namespace King.TurnBasedCombat
{
    /// <summary>
    /// 技能控制器，单例类，用于管理技能创建，技能攻击方法，技能防御方法，技能特效展示等
    /// </summary>
    public class SkillController : MonoBehaviour
    {
        #region 单例设计
        private static SkillController m_self = null;
        public static SkillController Instance
        {
            get
            {
                return m_self;
            }
        }

        void Awake()
        {
            m_self = this;
            Init();
        }
        #endregion

        //用于保存各个技能使用的技能特效
        private Dictionary<KeyValuePair<string, int>, List<GameObject>> _SkillEffect;
        public Dictionary<KeyValuePair<string, int>, List<GameObject>> SkillEffect
        {
            get
            {
                return _SkillEffect;
            }
        }

        /// <summary>
        /// 初始化技能控制器
        /// </summary>
        void Init()
        {
            _SkillEffect = new Dictionary<KeyValuePair<string, int>, List<GameObject>>();
        }


        /// <summary>
        /// 注册一个技能控制器的特效
        /// </summary>
        /// <param name="skill">技能控制器</param>
        public void RegisterSkillEffect(BaseSkill skill)
        {
            if (skill.SkillEffects.Count > 0 && !_SkillEffect.ContainsKey(skill.GetSkillIDAndLevel()))
            {
                Transform obj = null;
                for (int i = 0; i < skill.SkillEffects.Count; i++)
                {
                    if (UnityStaticTool.CreateObjectTo<Transform>(skill.SkillEffects[i], this.transform, Vector3.zero, out obj))
                    {
                        obj.gameObject.SetActive(false);
                        if (_SkillEffect.ContainsKey(skill.GetSkillIDAndLevel()))
                        {
                            _SkillEffect[skill.GetSkillIDAndLevel()].Add(obj.gameObject);
                        }
                        else
                        {
                            _SkillEffect.Add(skill.GetSkillIDAndLevel(), new List<GameObject>() { obj.gameObject });
                        }
                    }
                }
            }
            else
            {
                BattleController.Instance.DebugLog(LogType.ERROR,"Skill effect register failed! No effect found or effect is exist!");
            }
        }

        /// <summary>
        /// 为英雄添加技能
        /// </summary>
        /// <param name="skill">要添加的技能</param>
        /// <param name="hero">技能所属的英雄对象</param>
        /// <returns>返回创建的技能对象</returns>
        public BaseSkill CreateSkillTo(Skill skill, HeroMono hero)
        {
            BaseSkill result = null;
            System.Type type = System.Reflection.Assembly.GetExecutingAssembly().GetType("King.TurnBasedCombat." + skill.SkillMono);
            result = hero.gameObject.AddComponent(type) as BaseSkill;
            if (result == null)
            {
                BattleController.Instance.DebugLog(LogType.ERROR,skill.SkillMono + " Can't find,Please Check SkillMono is vaild or not!");
                return null;
            }
            result.Init(skill, hero);
            return result;
        }

        /// <summary>
        /// 防御一个技能
        /// </summary>
        /// <param name="attacker">技能发起者</param>
        /// <param name="denfender">技能防御者</param>
        /// <param name="skill">技能对象</param>
        public void DefenseSkill<T>(HeroMono attacker, HeroMono defender, T skill) where T : BaseSkill
        {
            skill.DefenseSkill(attacker, defender);
        }

        /// <summary>
        /// 执行一个Buff或者Debuff
        /// </summary>
        /// <param name="hero">buff执行对象</param>
        /// <param name="buff">要执行的buff</param>
        public void ExcuteBuff(HeroMono hero, Buff buff)
        {
            long temp = 0;
            switch (buff.BuffType)
            {
                case Global.BuffType.IncreaseAttack:
                    temp = buff.ChangeHeroProperty.Attack.RealTempValue(hero.CurrentAttack);
                    hero.CurrentAttack += temp;
                    if (temp > 0)
                    {
                        hero.ShowHeroHub(HubType.IncreseAttack, new ValueUnit(temp, Global.UnitType.Value));
                    }
                    else
                    {
                        hero.ShowHeroHub(HubType.DecreseAttack, new ValueUnit(-temp, Global.UnitType.Value));
                    }
                    break;
                case Global.BuffType.IncreaseDefense:
                    temp = buff.ChangeHeroProperty.Defense.RealTempValue(hero.CurrentDefense);
                    hero.CurrentDefense += temp;
                    if (temp > 0)
                    {
                        hero.ShowHeroHub(HubType.IncreseDefense, new ValueUnit(temp, Global.UnitType.Value));
                    }
                    else
                    {
                        hero.ShowHeroHub(HubType.DecreseDefense, new ValueUnit(-temp, Global.UnitType.Value));
                    }
                    break;
                case Global.BuffType.IncreaseLife:
                    temp = buff.ChangeHeroProperty.CurrentLife.RealTempValue(hero.CurrentMaxLife);
                    hero.CurrentLife += temp;
                    if (temp > 0)
                    {
                        hero.ShowHeroHub(HubType.IncreseLife, new ValueUnit(temp, Global.UnitType.Value));
                    }
                    else
                    {
                        hero.ShowHeroHub(HubType.DecreseLife, new ValueUnit(-temp, Global.UnitType.Value));
                    }
                    break;
                case Global.BuffType.IncreaseMagic:
                    temp = buff.ChangeHeroProperty.CurrentMagic.RealTempValue(hero.CurrentMaxMagic);
                    hero.CurrentMagic += temp;
                    if (temp > 0)
                    {
                        hero.ShowHeroHub(HubType.IncreseMagic, new ValueUnit(temp, Global.UnitType.Value));
                    }
                    else
                    {
                        hero.ShowHeroHub(HubType.DecreseMagic, new ValueUnit(-temp, Global.UnitType.Value));
                    }
                    break;
                case Global.BuffType.IncreaseMagicAttack:
                    temp = buff.ChangeHeroProperty.MagicAttack.RealTempValue(hero.CurrentMagicAttack);
                    hero.CurrentMagicAttack += temp;
                    if (temp > 0)
                    {
                        hero.ShowHeroHub(HubType.IncreseMagicAttack, new ValueUnit(temp, Global.UnitType.Value));
                    }
                    else
                    {
                        hero.ShowHeroHub(HubType.DecreseMagicAttack, new ValueUnit(-temp, Global.UnitType.Value));
                    }
                    break;
                case Global.BuffType.IncreaseMagicDefense:
                    temp = buff.ChangeHeroProperty.MagicDefense.RealTempValue(hero.CurrentMagicDefense);
                    hero.CurrentMagicDefense += temp;
                    if (temp > 0)
                    {
                        hero.ShowHeroHub(HubType.IncreseMagicDefense, new ValueUnit(temp, Global.UnitType.Value));
                    }
                    else
                    {
                        hero.ShowHeroHub(HubType.DecreseMagicDefense, new ValueUnit(-temp, Global.UnitType.Value));
                    }
                    break;
                case Global.BuffType.IncreaseSpeed:
                    temp = buff.ChangeHeroProperty.Speed.RealTempValue(hero.CurrentSpeed);
                    hero.CurrentSpeed += temp;
                    if (temp > 0)
                    {
                        hero.ShowHeroHub(HubType.IncreseSpeed, new ValueUnit(temp, Global.UnitType.Value));
                    }
                    else
                    {
                        hero.ShowHeroHub(HubType.DecreseSpeed, new ValueUnit(-temp, Global.UnitType.Value));
                    }
                    break;
                case Global.BuffType.IncreaseMaxLife:
                    temp = buff.ChangeHeroProperty.MaxLife.RealTempValue(hero.CurrentMaxLife);
                    hero.CurrentMaxLife += temp;
                    if (temp > 0)
                    {
                        hero.ShowHeroHub(HubType.IncreseMaxLife, new ValueUnit(temp, Global.UnitType.Value));
                    }
                    else
                    {
                        hero.ShowHeroHub(HubType.DecreseMaxLife, new ValueUnit(-temp, Global.UnitType.Value));
                    }
                    break;
                case Global.BuffType.IncreaseMaxMagic:
                    temp = buff.ChangeHeroProperty.MaxMagic.RealTempValue(hero.CurrentMaxMagic);
                    hero.CurrentMaxMagic += temp;
                    if (temp > 0)
                    {
                        hero.ShowHeroHub(HubType.IncreseMaxMagic, new ValueUnit(-temp, Global.UnitType.Value));
                    }
                    else
                    {
                        hero.ShowHeroHub(HubType.DecreseMaxMagic, new ValueUnit(-temp, Global.UnitType.Value));
                    }
                    break;
                case Global.BuffType.ActiveTurn:
                    hero.CurrentTurnLast += buff.ChangeHeroProperty.Turn;
                    BattleController.Instance.DebugLog(LogType.INFO,hero.Name + "回合数增加:" + buff.ChangeHeroProperty.Turn);
                    break;
            }
        }

        public IEnumerator MagicSkillEffect(KeyValuePair<string, int> skillType, GameObject effect, Vector3 startPos, Vector3 endPos, float speed, float start_delay, float end_delay, System.Action callback)
        {
            effect.gameObject.SetActive(true);
            yield return new WaitForSeconds(start_delay);
            float time = 0;
            while (time <= 1f)
            {
                time += Time.deltaTime * speed;
                effect.transform.position = Vector3.Lerp(startPos, endPos, time);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(end_delay);
            FreeSkillEffect(skillType);
            //技能特效结束
            if (callback != null)
            {
                callback();
            }
        }


        public IEnumerator WaitForTime(float time, System.Action callback)
        {
            yield return new WaitForSeconds(time);
            if (callback != null)
            {
                callback();
            }
        }

        /// <summary>
        /// 停止一个技能的特效
        /// </summary>
        /// <param name="skill_id">技能ID</param>
        public void FreeSkillEffect(KeyValuePair<string, int> skill)
        {
            for (int i = 0; i < _SkillEffect[skill].Count; i++)
            {
                _SkillEffect[skill][i].SetActive(false);
            }
        }

        /// <summary>
        /// 退出的时候调用的清除
        /// </summary>
        public void Clear()
        {
            //清空技能特效字典
            _SkillEffect.Clear();
        }
    }
}