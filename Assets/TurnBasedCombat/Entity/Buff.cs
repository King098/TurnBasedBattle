using UnityEngine;
using System.Collections;

namespace King.TurnBasedCombat
{
    /// <summary>
    /// Buff Debuff的实现类，主要是数据模型
    /// </summary>
    [System.Serializable]
    public class Buff
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID;
        /// <summary>
        /// buff名字
        /// </summary>
        public string Name;
        /// <summary>
        /// buff类型
        /// </summary>
        public Global.BuffType BuffType;
        /// <summary>
        /// buff的目标
        /// </summary>
        public Global.BuffTargetType TargetType;
        /// <summary>
        /// Buff效果在哪个状态
        /// </summary>
        public Global.BuffActiveState BuffActiveState;
        /// <summary>
        /// buff改变的英雄数值
        /// </summary>
        public HeroProperty ChangeHeroProperty;
        /// <summary>
        /// 是否是增益状态
        /// </summary>
        public bool IsBuff;
        /// <summary>
        /// Buff描述
        /// </summary>
        public string Description;



        /// <summary>
        /// buff成功几率（不参与读取数据赋值，应该根据技能分配成功比率）
        /// </summary>
        public int SuccessChance;

        /// <summary>
        /// buff持续剩余回合数
        /// </summary>
        public int StayTurn { get; set; }

        /// <summary>
        /// 空的构造函数
        /// </summary>
        public Buff() { }

        /// <summary>
        /// Buff构造函数
        /// </summary>
        /// <param name="ID">ID</param>
        public Buff(string ID)
        {
            this.ID = ID;
            this.Name = "Buff";
            this.BuffType = Global.BuffType.IncreaseLife;
            this.TargetType = Global.BuffTargetType.None;
            this.BuffActiveState = Global.BuffActiveState.BeforeAction;
            this.ChangeHeroProperty = new HeroProperty();
            this.IsBuff = true;
            this.Description = "buff描述";
        }

        /// <summary>
        /// 通过读取表格数据使用的构造函数
        /// </summary>
        /// <param name="temps">读取的一条表格数据</param>
        public Buff(string[] temps)
        {
            int offset = 0;
            ID = temps[offset].ToString(); offset++;
            Name = temps[offset].ToString(); offset++;
            BuffType = (Global.BuffType)int.Parse(temps[offset]); offset++;
            TargetType = (Global.BuffTargetType)int.Parse(temps[offset]); offset++;
            BuffActiveState = (Global.BuffActiveState)int.Parse(temps[offset]); offset++;
            ChangeHeroProperty = new HeroProperty(temps[offset]); offset++;
            IsBuff = temps[offset] == "0" ? false : true; offset++;
            SuccessChance = int.Parse(temps[offset]);offset++;
            Description = temps[offset].ToString(); offset++;
            //赋值buff持续回合
            StayTurn = ChangeHeroProperty.Turn;
        }


        /// <summary>
        /// 根据技能进行构造数据
        /// </summary>
        /// <param name="ID">技能ID</param>
        /// <param name="chance">技能成功几率</param>
        public Buff(string ID, int chance)
        {
            Buff buff = BuffTable.Instance.GetBuffByID(ID);
            this.ID = buff.ID;
            this.Name = buff.Name;
            this.BuffType = buff.BuffType;
            this.TargetType = buff.TargetType;
            this.BuffActiveState = buff.BuffActiveState;
            this.StayTurn = buff.StayTurn;
            this.ChangeHeroProperty = buff.ChangeHeroProperty;
            this.IsBuff = buff.IsBuff;
            this.Description = buff.Description;
            this.SuccessChance = chance;
        }

        /// <summary>
        /// 可能需要的初始化函数,可以在使用buff的时候调用
        /// </summary>
        public void Init() 
        {
            StayTurn = this.ChangeHeroProperty.Turn;
        }

        /// <summary>
        /// 每过一个回合调用
        /// </summary>
        public void TurnChanged()
        {
            if (StayTurn <= 0)
            {
                OnBuffEnd();
                return;
            }
            StayTurn -= 1;
        }

        /// <summary>
        /// 当一个buff或者debuff效果结束的时候调用
        /// </summary>
        public void OnBuffEnd()
        { 
        }
    }
}