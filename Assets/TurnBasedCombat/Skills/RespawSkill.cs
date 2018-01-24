using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace King.TurnBasedCombat
{
    /*[SkillDescription-Start]
    死亡时触发的被动技能，可以加载技能特效，但只使用第一个技能特效
    可以使用技能消耗和暴击设置
    可以指定多个目标（不论是队友还是敌人）
    制作成一局只能触发一次的技能，直接将技能释放消耗回合数设置足够大即可
    使用英雄动画 Idle 作为防御者动作
    技能表现为技能使用者死亡时候触发，为指定的目标对象修改指定数值
    同时播放技能特效，0.5秒后特效播放结束
    [SkillDescription-End]
     */

    /// <summary>
    /// 重生术（拥有很长的CD时间，可以在英雄死亡的瞬间重生，并获得Value/Unit的生命值）
    /// </summary>
    public class RespawSkill : BaseSkill
    {
        public override void Init(Skill skill, HeroMono hero)
        {
            // SkillEffects.Add(Resources.Load<GameObject>("SkillEffect/RespwanSkill/effect"));
            base.Init(skill, hero);
        }


        public override bool CanUseSkill()
        {
            return base.CanUseSkill();
        }


        public override void ExcuteSkill(List<HeroMono> targets)
        {
            IsUsingSkill = true;
            ExcutingSkill(targets);
        }


        public override void ExcutingSkill(List<HeroMono> targets)
        {
            this.SetSkillEffect(_Hero, targets, () =>
            {
                //对目标使用
                for (int i = 0; i < targets.Count; i++)
                {
                    targets[i].Defense(Global.SkillType.Respwan, _Hero);
                }
                //结束技能
                ExcutedSkill(targets);
            });
        }


        public override void ExcutedSkill(List<HeroMono> targets)
        {
            base.ExcutedSkill(targets);
        }

        public override bool IsCostTurn()
        {
            return base.IsCostTurn();
        }

        public override bool IsDelayTurn()
        {
            return base.IsDelayTurn();
        }

        public override void Cost()
        {
            base.Cost();
        }

        public override void DefenseSkill(HeroMono attacker, HeroMono defender)
        {
            //完全魔法攻击，没有任何添加
            defender.CurrentLife += this.CauseHeroProperty.CurrentLife.RealTempValue(defender.CurrentMaxLife);
            //设置为未死亡状态
            defender.IsDeath = false;
            //播放动画
            defender.PlayTriggerAnimation(HeroAnimation.Idle);
            //正常的显示
            defender.ShowHeroHub(HubType.IncreseLife, new ValueUnit(this.CauseHeroProperty.CurrentLife.RealTempValue(defender.CurrentMaxLife), Global.UnitType.Value));
            //进行buff判断
            AddSkillBuff(attacker,defender);
            // Debug.Log((attacker.IsPlayerHero ? "玩家的" : "敌人的") + attacker.Name + "使用技能" + this.Name + "对" + defender.Name + "造成0点伤害");
        }

        /// <summary>
        /// 设置技能特效
        /// </summary>
        public override void SetSkillEffect(HeroMono attacker, List<HeroMono> targets, System.Action callback)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                SkillController.Instance.SkillEffect[GetSkillIDAndLevel()][i].transform.position = targets[i].HeroPosition;
                SkillController.Instance.SkillEffect[GetSkillIDAndLevel()][i].gameObject.SetActive(true);
                SkillController.Instance.SkillEffect[GetSkillIDAndLevel()][i].GetComponent<Animator>().SetTrigger("show");
            }
            StartCoroutine(SkillController.Instance.WaitForTime(_Skill.SkillEndDelay, () =>
                {
                    // for (int i = 0; i < SkillController.Instance.SkillEffect[GetSkillIDAndLevel()].Count; i++)
                    // {
                    //     SkillController.Instance.SkillEffect[GetSkillIDAndLevel()][i].gameObject.SetActive(false);
                    // }
                    if (callback != null)
                    {
                        callback();
                    }
                }));
        }
    }
}