﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace King.TurnBasedCombat
{

    /*[SkillDescription-Start]
    魔法攻击技能，可以加载技能特效，但只使用第一个技能特效
    可以使用技能消耗和暴击设置
    只能指定一个目标，如果使用了多目标设置，仍然只随机一个敌人的目标
    制作成一局只能触发一次的技能，直接将技能释放消耗回合数设置足够大即可
    技能特效移动数值为开始结束均不进行延时操作，移动速度为3f
    使用英雄动画 MagicAttack1 作为攻击动画 MagicDefense1 作为敌人防御动画
    技能表现为攻击者使用MagicAttack1攻击防御者，防御者使用Defense1防御，
    播放技能特效从攻击者处移动到防御者处，特效消失
    [SkillDescription-End]
     */


    /// <summary>
    /// 普通魔法攻击(将在原地释放法术，然后攻击敌人)
    /// </summary>
    public class MagicSkill : BaseSkill
    {
        public override async Task Init(Skill skill, HeroMono hero)
        {
            // SkillEffects.Add(Resources.Load<GameObject>("SkillEffect/MagicSkill/effect"));
            await base.Init(skill, hero);
        }

        public override bool CanUseSkill()
        {
            return base.CanUseSkill();
        }

        public override void ExcuteSkill(List<HeroMono> targets)
        {
            if (targets.Count > 0)
            {
                IsUsingSkill = true;
                ExcutingSkill(targets);
            }
        }

        public override void ExcutingSkill(List<HeroMono> targets)
        {
            //这里播放动画
            _Hero.PlayTriggerAnimation(HeroAnimation.MagicAttack1);
            this.SetSkillEffect(_Hero, targets, () =>
            {
                //这里对目标攻击
                targets[0].Defense(SkillType, _Hero);
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
            long hurt = 0;
            //完全魔法攻击，没有任何添加
            bool is_maigc_critical = this.IsInPercent(CriticalChance);
            hurt = attacker.CurrentMagicAttack + Mathf.RoundToInt(this.MagicAttack * (is_maigc_critical ? 2 : 1)) - defender.CurrentMagicDefense;
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
            if (is_maigc_critical)
            {
                if (hurt > 0)
                {
                    defender.ShowHeroHub(HubType.Critical, new ValueUnit(hurt,Global.UnitType.Value));
                }
                else if (hurt == 0)
                {
                    defender.ShowHeroHub(HubType.Miss);
                }
            }
            //正常的显示
            else if (hurt > 0)
            {
                defender.ShowHeroHub(HubType.DecreseLife, new ValueUnit(hurt,Global.UnitType.Value));
            }
            else if (hurt == 0)
            {
                defender.ShowHeroHub(HubType.Miss);
            }
            //进行buff判断
            AddSkillBuff(attacker,defender);
            // Debug.Log((attacker.IsPlayerHero ? "玩家的" : "敌人的") + attacker.Name + "使用技能" + this.Name + "对" + defender.Name + "造成" + hurt + "点伤害");
        }

        /// <summary>
        /// 设置技能特效
        /// </summary>
        public override void SetSkillEffect(HeroMono attacker, List<HeroMono> targets, System.Action callback)
        {
            SkillController.Instance.SkillEffect[GetSkillIDAndLevel()][0].transform.position = attacker.HeroPosition;
            Vector3 scale = SkillController.Instance.SkillEffect[GetSkillIDAndLevel()][0].transform.localScale;
            if (attacker.TeamIndex == 0 || attacker.TeamIndex == 3)
            {
                scale.x = Mathf.Abs(scale.x) * -1f;
            }
            else
            {
                scale.x = Mathf.Abs(scale.x);
            }
            SkillController.Instance.SkillEffect[GetSkillIDAndLevel()][0].transform.localScale = scale;
            StartCoroutine(SkillController.Instance.MagicSkillEffect(GetSkillIDAndLevel(),SkillController.Instance.SkillEffect[GetSkillIDAndLevel()][0], attacker.AttackPosition.position, targets[0].HeroPosition,3f,0f,0f, callback));
        }
    }
}