using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace King.TurnBasedCombat
{
    /// <summary>
    /// 参与战斗的英雄的对象模型
    /// </summary>
    [System.Serializable]
    public class Hero
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID;
        /// <summary>
        /// 英雄名字
        /// </summary>
        public string Name;
        /// <summary>
        /// 英雄的真名（仿照魔灵召唤里面觉醒英雄会出现专属名字）
        /// </summary>
        public string RealName;
        /// <summary>
        /// 头像路径
        /// </summary>
        public string Ico;
        /// <summary>
        /// 英雄等级
        /// </summary>
        public int Level;
        /// <summary>
        /// 英雄星级
        /// </summary>
        public Global.StarLevel StarLevel;
        /// <summary>
        /// 生命值
        /// </summary>
        public long BaseLife;
        /// <summary>
        /// 魔力值
        /// </summary>
        public long BaseMagic;
        /// <summary>
        /// 物理攻击力
        /// </summary>
        public long BaseAttack;
        /// <summary>
        /// 物理防御力
        /// </summary>
        public long BaseDefense;
        /// <summary>
        /// 魔法攻击力
        /// </summary>
        public long BaseMagicAttack;
        /// <summary>
        /// 魔法防御力
        /// </summary>
        public long BaseMagicDefense;
        /// <summary>
        /// 速度
        /// </summary>
        public long BaseSpeed;
        /// <summary>
        /// 英雄的技能
        /// </summary>
        public List<Skill> Skills;
        /// <summary>
        /// 英雄的描述
        /// </summary>
        public string Description;

        /// <summary>
        /// 空的构造函数
        /// </summary>
        public Hero() { }

        /// <summary>
        /// 赋予ID的构造函数
        /// </summary>
        /// <param name="ID"></param>
        public Hero(string ID)
        {
            this.ID = ID;
            this.Name = "新英雄";
            this.RealName = "新英雄";
            this.Ico = "";
            this.Level = 1;
            this.StarLevel = Global.StarLevel.One;
            this.BaseLife = 100;
            this.BaseMagic = 100;
            this.BaseAttack = 10;
            this.BaseDefense = 5;
            this.BaseMagicAttack = 15;
            this.BaseMagicDefense = 10;
            this.BaseSpeed = 15;
            this.Skills = new List<Skill>();
            this.Description = "描述信息";
        }

        /// <summary>
        /// 读取表格数据使用的构造函数
        /// </summary>
        /// <param name="temps"></param>
        public Hero(string[] temps)
        {
            int offset = 0;
            ID = temps[offset].ToString(); offset++;
            Name = temps[offset].ToString(); offset++;
            RealName = temps[offset].ToString(); offset++;
            Ico = temps[offset].ToString(); offset++;
            Level = int.Parse(temps[offset]); offset++;
            StarLevel = (Global.StarLevel)int.Parse(temps[offset]); offset++;
            BaseLife = long.Parse(temps[offset]); offset++;
            BaseMagic = long.Parse(temps[offset]); offset++;
            BaseAttack = long.Parse(temps[offset]); offset++;
            BaseDefense = long.Parse(temps[offset]); offset++;
            BaseMagicAttack = long.Parse(temps[offset]); offset++;
            BaseMagicDefense = long.Parse(temps[offset]); offset++;
            BaseSpeed = long.Parse(temps[offset]); offset++;
            Skills = new List<Skill>();
            string[] skill = temps[offset].ToString().Split(';'); offset++;
            for (int i = 0; i < skill.Length; i++)
            {
                string[] s = skill[i].Split('|');
                if (s.Length == 2)
                {
                    Skills.Add(SkillTable.Instance.GetSkillByIDAndLevel(s[0], int.Parse(s[1])));
                }
            }
            Description = temps[offset].ToString(); offset++;
        }

        public override string ToString()
        {
            string result = "";
            result += this.ID + "\t";
            result += this.Name + "\t";
            result += this.RealName + "\t";
            result += this.Ico + "\t";
            result += this.Level + "\t";
            result += (int)this.StarLevel + "\t";
            result += this.BaseLife + "\t";
            result += this.BaseMagic + "\t";
            result += this.BaseAttack + "\t";
            result += this.BaseDefense + "\t";
            result += this.BaseMagicAttack + "\t";
            result += this.BaseMagicDefense + "\t";
            result += this.BaseSpeed + "\t";
            string temp = "";
            for (int i = 0; i < this.Skills.Count; i++)
            {
                if (this.Skills[i] == null)
                    continue;
                if (i == this.Skills.Count - 1)
                {
                    temp += this.Skills[i].ID + "|" + this.Skills[i].Level;
                }
                else
                {
                    temp += this.Skills[i].ID + "|" + this.Skills[i].Level + ";";
                }
            }
            result += temp + "\t";
            result += this.Description;
            return result;
        }
    }
}