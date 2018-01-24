using UnityEngine;

namespace King.TurnBasedCombat
{
    /// <summary>
    /// 每个英雄对应的UI上的元素存储
    /// </summary>
    public class BaseBattleHeroUI : MonoBehaviour , IBattleHeroUI
    {
        /// <summary>
        /// 战斗界面英雄UI的初始化函数
        /// </summary>
        public virtual void Init(HeroMono hero){}

        /// <summary>
        /// 当这个英雄是当前回合的英雄时候调用
        /// </summary>
        public virtual void Select(){}

		/// <summary>
        /// 当这个英雄回合结束的时候调用
        /// </summary>
        public virtual void Deselect(){}

        /// <summary>
        /// 当获得一个buff或者debuff的时候将会回调
        /// </summary>
        public virtual void OnAddBuff(Buff buff){}

        /// <summary>
        /// 当移除一个buff或者debuff的时候将会回调
        /// </summary>
        public virtual void OnRemoveBuff(Buff buff){}

        /// <summary>
        /// 当一个buff或者debuff执行一次的时候将会回调
        public virtual void OnBuffAction(Buff buff){}
    }
}