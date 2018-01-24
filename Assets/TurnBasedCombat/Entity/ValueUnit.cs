using UnityEngine;
using System.Collections;

namespace King.TurnBasedCombat
{
    /// <summary>
    /// 数值加单位组成的新的数据类型(只支持int数值)
    /// </summary>
    [System.Serializable]
    public class ValueUnit
    {
        /// <summary>
        /// 数值(负值表示消耗，正值表示增长)
        /// </summary>
        public long Value;
        /// <summary>
        /// 单位
        /// </summary>
        public Global.UnitType Unit;

        /// <summary>
        /// 空的构造函数
        /// </summary>
        public ValueUnit() { }

        /// <summary>
        /// 读取数值的构造方法
        /// </summary>
        /// <param name="temp">读取到的数据</param>
        public ValueUnit(string temp)
        {
            string[] strs = temp.Split('|');
            if (strs.Length == 2)
            {
                this.Value = long.Parse(strs[0]);
                this.Unit = (Global.UnitType)int.Parse(strs[1]);
            }
            else
            {
                this.Value = 0;
                this.Unit = Global.UnitType.Value;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="v">数值</param>
        /// <param name="u">单位</param>
        public ValueUnit(long v, Global.UnitType u)
        {
            this.Value = v;
            this.Unit = u;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="v">数值</param>
        /// <param name="u">单位（0数值，1百分比）</param>
        public ValueUnit(long v, string u)
        {
            this.Value = v;
            this.Unit = (Global.UnitType)int.Parse(u);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="v">数值</param>
        /// <param name="u">单位（0数值，1百分比）</param>
        public ValueUnit(long v, int u)
        {
            this.Value = v;
            this.Unit = (Global.UnitType)u;
        }

        /// <summary>
        /// 复制构造函数
        /// </summary>
        /// <param name="value"></param>
        public ValueUnit(ValueUnit value)
        {
            this.Value = value.Value;
            this.Unit = value.Unit;
        }

        /// <summary>
        /// 获取百分比比率
        /// </summary>
        /// <param name="result">保存比率的变量</param>
        /// <returns>返回是否成功获取百分比</returns>
        public bool GetPercent(out float result)
        {
            if (Unit == Global.UnitType.Percent)
            {
                result = Value / 100f;
                return true;
            }
            result = 0;
            return false;
        }

        /// <summary>
        /// 获取数值
        /// </summary>
        /// <param name="result">保存数值的变量</param>
        /// <returns>返回是否成功获取数值</returns>
        public bool GetValue(out long result)
        {
            if (Unit == Global.UnitType.Value)
            {
                result = Value;
                return true;
            }
            result = 0;
            return false;
        }

        /// <summary>
        /// 真实计算后叠加了基础数据的数值
        /// </summary>
        /// <param name="baseValue">基于哪个数值进行的计算</param>
        /// <returns>返回计算之后的数值</returns>
        public long RealValue(long baseValue)
        {
            if (Unit == Global.UnitType.Value)
            {
                return baseValue + Value;
            }
            else if (Unit == Global.UnitType.Percent)
            {
                return baseValue + Mathf.RoundToInt(baseValue * (Value / 100f));
            }
            return baseValue + Value;
        }

        /// <summary>
        /// 真是计算后没有叠加基础数据的数值
        /// </summary>
        /// <param name="baseValue">基础数据</param>
        /// <returns>返回基于基础数据的加成数值</returns>
        public long RealTempValue(long baseValue)
        {
            if (Unit == Global.UnitType.Value)
            {
                return Value;
            }
            else if (Unit == Global.UnitType.Percent)
            {
                return Mathf.RoundToInt(baseValue * (Value / 100f));
            }
            return Value;
        }

        public override string ToString()
        {
            return Value + "|" + ((int)Unit).ToString();
        }
    }
}
