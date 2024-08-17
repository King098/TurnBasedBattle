using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace King.TurnBasedCombat
{
	/*[SkillDescription-Start]
    多目标物理攻击技能，可以加载多个技能特效，根据目标情况去使用特效
    可以使用技能消耗和暴击设置
    敌人目标数量应该为 目标设置的数量和当前剩余敌人数量取最小值
	可以设置目标敌人数量（当敌人数量设置为one enemy时效果等同于PhysicSkill技能脚本）
    制作成一局只能触发一次的技能，直接将技能释放消耗回合数设置足够大即可
    使用英雄动画 Walk 作为玩家移动动画 Attack1 作为攻击动画 Defense1 作为敌人防御动画
    技能表现为攻击者通过Walk动画移动到防御者面前，然后使用Attack1攻击防御者，防御者使用Defense1防御，
    同时播放技能特效，0.5秒后攻击者移动回原来的位置，特效播放结束
    [SkillDescription-End]
     */

    public class MutiPhysicSkill : PhysicSkill
    {
        public override async Task Init(Skill skill, HeroMono hero)
        {
            // for (int i = 0; i < 6; i++)
            // {
            //     SkillEffects.Add(Resources.Load<GameObject>("SkillEffect/PhysicalSkill/effect"));
            // }
            await base.Init(skill, hero);
        }

        public override bool CanUseSkill()
        {
            return base.CanUseSkill();
        }


        public override void ExcuteSkill(List<HeroMono> targets)
        {
            ExcutingSkill(targets);
        }

        public override void ExcutingSkill(List<HeroMono> targets)
        {
            if (targets.Count > 0)
            {
                IsUsingSkill = true;
                StartCoroutine(ToAttack(targets));
            }
        }


        IEnumerator ToAttack(List<HeroMono> targets)
        {
			_Hero.PlayTriggerAnimation(HeroAnimation.Walk);
            float time = 0f;
            Vector3 target_pos = Vector3.zero;
            for (int i = 0; i < targets.Count; i++)
            {
                target_pos += targets[i].AttackPosition.position;
            }
            target_pos /= targets.Count;
            //移动到目标出
            while (time <= 1f)
            {
                time += Time.deltaTime * 2f;
                _Hero.transform.position = Vector3.Lerp(_Hero.HeroPosition, target_pos, time);
                yield return new WaitForEndOfFrame();
            }
            //这里攻击目标
            //播放特效
            this.SetSkillEffect(_Hero, targets, null);
            //播放攻击动画
            _Hero.PlayTriggerAnimation(HeroAnimation.Attack1);
            // targets[0].Defense(SkillType, _Hero);
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].Defense(SkillType, _Hero);
            }
			//攻击结束调用技能结束方法
            ExcutedSkill(targets);
            yield return new WaitForSeconds(0.5f);
            //移动会原始位置
            while (time >= 0)
            {
                time -= Time.deltaTime * 3f;
                _Hero.transform.position = Vector3.Lerp(_Hero.HeroPosition, target_pos, time);
                yield return new WaitForEndOfFrame();
            }
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

        public override void DefenseSkill(HeroMono attacker, HeroMono defender)
        {
            long hurt = 0;
            //完全物理攻击，没有任何添加
            bool is_physicl_critical = this.IsInPercent(CriticalChance);
            hurt = attacker.CurrentAttack + Mathf.RoundToInt(this.Attack * (is_physicl_critical ? 2 : 1)) - defender.CurrentDefense;
            defender.CurrentLife -= hurt;
            //播放动画
            defender.PlayTriggerAnimation(HeroAnimation.Defense1);
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
            if (is_physicl_critical)
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
            for (int i = 0; i < targets.Count; i++)
            {
                SkillController.Instance.SkillEffect[GetSkillIDAndLevel()][i].transform.position = targets[i].HeroPosition;
                Vector3 scale = SkillController.Instance.SkillEffect[GetSkillIDAndLevel()][i].transform.localScale;
                if (attacker.TeamIndex == 0 || attacker.TeamIndex == 3)
                {
                    scale.x = Mathf.Abs(scale.x) * -1f;
                }
                else
                {
                    scale.x = Mathf.Abs(scale.x);
                }
                SkillController.Instance.SkillEffect[GetSkillIDAndLevel()][i].transform.localScale = scale;
                SkillController.Instance.SkillEffect[GetSkillIDAndLevel()][i].gameObject.SetActive(true);
                SkillController.Instance.SkillEffect[GetSkillIDAndLevel()][i].GetComponent<Animator>().SetTrigger("show");
            }
        }
    }
}