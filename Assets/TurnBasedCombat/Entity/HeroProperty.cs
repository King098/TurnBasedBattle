using UnityEngine;
using System.Collections;

namespace King.TurnBasedCombat
{
    /// <summary>
    /// 英雄数值实体类，可以用此类记录buff变化情况，技能变化情况
    /// </summary>
    [System.Serializable]
    public class HeroProperty
    {
        /// <summary>
        /// 血量上限的数值单位对象
        /// </summary>
        public ValueUnit MaxLife;
        /// <summary>
        /// 魔力上限的数值单位对象
        /// </summary>
        public ValueUnit MaxMagic;
        /// <summary>
        /// 当前血量的数值单位对象
        /// </summary>
        public ValueUnit CurrentLife;
        /// <summary>
        /// 当前魔力数值单位对象
        /// </summary>
        public ValueUnit CurrentMagic;
        /// <summary>
        /// 当前物理攻击数值单位对象
        /// </summary>
        public ValueUnit Attack;
        /// <summary>
        /// 当前物理防御数值单位对象
        /// </summary>
        public ValueUnit Defense;
        /// <summary>
        /// 当前魔力攻击数值单位对象
        /// </summary>
        public ValueUnit MagicAttack;
        /// <summary>
        /// 当前魔力防御数值单位对象
        /// </summary>
        public ValueUnit MagicDefense;
        /// <summary>
        /// 当前速度数值单位对象
        /// </summary>
        public ValueUnit Speed;
        /// <summary>
        /// 当前回合数是增加还是减少
        /// </summary>
        public int Turn;

        /// <summary>
        /// 空的构造函数
        /// </summary>
        public HeroProperty() 
        {
            MaxLife = new ValueUnit();
            MaxMagic = new ValueUnit();
            CurrentLife = new ValueUnit();
            CurrentMagic = new ValueUnit();
            Attack = new ValueUnit();
            Defense = new ValueUnit();
            MagicAttack = new ValueUnit();
            MagicDefense = new ValueUnit();
            Speed = new ValueUnit();
            Turn = 0;
        }

        /// <summary>
        /// 通过读取数据使用的构造函数
        /// </summary>
        /// <param name="temp">读取到的数据</param>
        public HeroProperty(string temp)
        {
            string[] strs = temp.Split(':');
            if (strs.Length == 10)
            {
                int offset = 0;
                MaxLife = new ValueUnit(strs[offset]); offset++;
                MaxMagic = new ValueUnit(strs[offset]); offset++;
                CurrentLife = new ValueUnit(strs[offset]); offset++;
                CurrentMagic = new ValueUnit(strs[offset]); offset++;
                Attack = new ValueUnit(strs[offset]); offset++;
                Defense = new ValueUnit(strs[offset]); offset++;
                MagicAttack = new ValueUnit(strs[offset]); offset++;
                MagicDefense = new ValueUnit(strs[offset]); offset++;
                Speed = new ValueUnit(strs[offset]); offset++;
                Turn = int.Parse(strs[offset]); offset++;
            }
        }

        /// <summary>
        /// 复制构造函数
        /// </summary>
        /// <param name="property"></param>
        public HeroProperty(HeroProperty property)
        {
            this.MaxLife = property.MaxLife;
            this.MaxMagic = property.MaxMagic;
            this.CurrentLife = property.CurrentLife;
            this.CurrentMagic = property.CurrentMagic;
            this.Attack = property.Attack;
            this.Defense = property.Defense;
            this.MagicAttack = property.MagicAttack;
            this.MagicDefense = property.MagicDefense;
            this.Speed = property.Speed;
            this.Turn = property.Turn;
        }

        public HeroProperty(Hero hero,float speedMix)
        {
            MaxLife = new ValueUnit(hero.BaseLife,Global.UnitType.Value);
            MaxMagic = new ValueUnit(hero.BaseMagic, Global.UnitType.Value);
            CurrentLife = new ValueUnit(hero.BaseLife, Global.UnitType.Value);
            CurrentMagic = new ValueUnit(hero.BaseMagic, Global.UnitType.Value);
            Attack = new ValueUnit(hero.BaseAttack, Global.UnitType.Value);
            Defense = new ValueUnit(hero.BaseDefense, Global.UnitType.Value);
            MagicAttack = new ValueUnit(hero.BaseMagicAttack, Global.UnitType.Value);
            MagicDefense = new ValueUnit(hero.BaseMagicDefense, Global.UnitType.Value);
            Speed = new ValueUnit(Mathf.RoundToInt(hero.BaseSpeed * Random.Range(speedMix, 1f)), Global.UnitType.Value);
            Turn = 0;
        }


        public override string ToString()
        {
            string result = "";
            result += MaxLife.ToString() + ":";
            result += MaxMagic.ToString() + ":";
            result += CurrentLife.ToString() + ":";
            result += CurrentMagic.ToString() + ":";
            result += Attack.ToString() + ":";
            result += Defense.ToString() + ":";
            result += MagicAttack.ToString() + ":";
            result += MagicDefense.ToString() + ":";
            result += Speed.ToString() + ":";
            result += Turn.ToString();
            return result;
        }
    }
}