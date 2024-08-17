using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace King.TurnBasedCombat
{
    /// <summary>
    /// 技能对象
    /// </summary>
    [System.Serializable]
    public class Skill
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID;
        /// <summary>
        /// 技能名字
        /// </summary>
        public string Name;
        /// <summary>
        /// 技能图标
        /// </summary>
        public string Ico;
        /// <summary>
        /// 技能类型
        /// </summary>
        public Global.SkillType SkillType;
        /// <summary>
        /// 技能控制脚本
        /// </summary>
        public string SkillMono;
        /// <summary>
        /// 技能结束时候等待多长时间
        /// </summary>
        public float SkillEndDelay;
        /// <summary>
        /// 是否是被动技能（默认不是）
        /// </summary>
        public bool IsPassiveSkill;
        /// <summary>
        /// 技能特效路径
        /// </summary>
        public List<string> SkillEffectPaths;
        /// <summary>
        /// 技能发动的时机
        /// </summary>
        public Global.BuffActiveState ActiveState;
        /// <summary>
        /// 技能的目标类型
        /// </summary>
        public Global.SkillTargetType TargetType;
        /// <summary>
        /// 技能的目标数量
        /// </summary>
        public int TargetNumber;
        /// <summary>
        /// 目标是否包含被控的英雄对象
        /// </summary>
        public bool TargetContainsControlledHero;
        /// <summary>
        /// 技能等级
        /// </summary>
        public int Level;
        /// <summary>
        /// 来发动技能英雄消耗的属性值变化对象
        /// </summary>
        public HeroProperty CostHeroProperty;
        /// <summary>
        /// 技能需要蓄力的回合数
        /// </summary>
        public int DelayTurn;
        /// <summary>
        /// 造成目标的属性值变化对象
        /// </summary>
        public HeroProperty CauseHeroProperty;
        /// <summary>
        /// 攻击目标造成的伤害(这个对象主要用于物理攻击和魔法攻击叠加)
        /// </summary>
        public HeroProperty HurtHeroProperty;
        /// <summary>
        /// 技能暴击几率，单位为百分比
        /// </summary>
        public int CriticalChance;
        /// <summary>
        /// 技能将会按照几率产生的各种buff
        /// </summary>
        public List<Buff> SkillBuff;
        /// <summary>
        /// 技能的描述
        /// </summary>
        public string Description;

        /// <summary>
        /// 空的构造函数
        /// </summary>
        public Skill() { }

        /// <summary>
        /// 编辑器使用的构造函数
        /// </summary>
        /// <param name="ID">ID</param>
        /// <param name="Level">等级</param>
        public Skill(string ID, int Level)
        {
            this.ID = ID;
            this.Level = Level;
            this.Name = "新技能";
            this.Ico = "";
            this.SkillType = Global.SkillType.Physic;
            this.SkillMono = "BaseSkill";
            this.SkillEndDelay = 0f;
            this.IsPassiveSkill = false;
            this.SkillEffectPaths = new List<string>();
            this.ActiveState = Global.BuffActiveState.Actioning;
            this.TargetType = Global.SkillTargetType.None;
            this.CostHeroProperty = new HeroProperty();
            this.DelayTurn = 0;
            this.CauseHeroProperty = new HeroProperty();
            this.HurtHeroProperty = new HeroProperty();
            this.CriticalChance = 0;
            this.SkillBuff = new List<Buff>();
            this.Description = "技能描述";
        }

        public Skill(string[] temps)
        {
            int offset = 0;
            ID = temps[offset].ToString(); offset++;
            Name = temps[offset].ToString(); offset++;
            Ico = temps[offset].ToString(); offset++;
            SkillType = (Global.SkillType)int.Parse(temps[offset]); offset++;
            SkillMono = temps[offset].ToString(); offset++;
            SkillEndDelay = float.Parse(temps[offset].ToString());offset++;
            IsPassiveSkill = temps[offset].ToString() == "0" ? false : true; offset++;
            SkillEffectPaths = new List<string>();
            SkillEffectPaths.AddRange(temps[offset].ToString().Split(new string[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries)); offset++;
            ActiveState = (Global.BuffActiveState)int.Parse(temps[offset]); offset++;
            TargetType = (Global.SkillTargetType)int.Parse(temps[offset]); offset++;
            TargetNumber = int.Parse(temps[offset]); offset++;
            TargetContainsControlledHero = temps[offset].ToString() == "0" ? false : true; offset++;
            Level = int.Parse(temps[offset]); offset++;
            CostHeroProperty = new HeroProperty(temps[offset]); offset++;
            DelayTurn = int.Parse(temps[offset]); offset++;
            CauseHeroProperty = new HeroProperty(temps[offset]); offset++;
            HurtHeroProperty = new HeroProperty(temps[offset]); offset++;
            CriticalChance = int.Parse(temps[offset]); offset++;
            SkillBuff = new List<Buff>();
            string[] buffs = temps[offset].ToString().Split(';'); offset++;
            for (int i = 0; i < buffs.Length; i++)
            {
                string[] bf = buffs[i].Split('|');
                if (bf.Length == 2)
                {
                    SkillBuff.Add(new Buff(bf[0], int.Parse(bf[1])));
                }
            }
            Description = temps[offset].ToString(); offset++;
        }

        public override string ToString()
        {
            string result = "";
            result += this.ID + "\t";
            result += this.Name + "\t";
            result += this.Ico + "\t";
            result += (int)this.SkillType + "\t";
            result += this.SkillMono + "\t";
            result += this.SkillEndDelay + "\t";
            result += (this.IsPassiveSkill ? "1" : "0") + "\t";
            string temp = "";
            for (int i = 0; i < this.SkillEffectPaths.Count; i++)
            {
                if (i == this.SkillEffectPaths.Count - 1)
                {
                    temp += this.SkillEffectPaths[i];
                }
                else
                {
                    temp += this.SkillEffectPaths[i] + "|";
                }
            }
            result += temp + "\t";
            result += (int)this.ActiveState + "\t";
            result += (int)this.TargetType + "\t";
            result += this.TargetNumber + "\t";
            result += (this.TargetContainsControlledHero ? "1" : "0") + "\t";
            result += this.Level + "\t";
            result += this.CostHeroProperty.ToString() + "\t";
            result += this.DelayTurn + "\t";
            result += this.CauseHeroProperty.ToString() + "\t";
            result += this.HurtHeroProperty.ToString() + "\t";
            result += this.CriticalChance + "\t";
            temp = "";
            for (int i = 0; i < this.SkillBuff.Count; i++)
            {
                if (i == this.SkillBuff.Count - 1)
                {
                    temp += this.SkillBuff[i].ID + "|" + this.SkillBuff[i].SuccessChance;
                }
                else
                {
                    temp += this.SkillBuff[i].ID + "|" + this.SkillBuff[i].SuccessChance + ";";
                }
            }
            result += temp + "\t";
            result += this.Description;
            return result;
        }
    }
}