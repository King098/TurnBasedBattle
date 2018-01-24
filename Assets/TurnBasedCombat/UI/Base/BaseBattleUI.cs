using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King.TurnBasedCombat
{
    public class BaseBattleUI : MonoBehaviour , IBattleUI
    {
		/// <summary>
        /// 初始化UI控制器
        /// </summary>
		public virtual void Init(){}

		/// <summary>
        /// 创建UI上的所有参与战斗的英雄Mono对象并向BattleController注册这个对象
        /// </summary>
        /// <param name="player">玩家队伍数据</param>
        /// <param name="enemy">敌人队伍数据</param>
		public virtual void CreateHeros(List<Hero> player,List<Hero> enemy){}


		/// <summary>
        /// 注册玩家英雄UI信息
        /// </summary>
        /// <param name="hero">注册的英雄对象</param>
		public virtual void RegisterPlayerHero(HeroMono hero){}
		

		/// <summary>
        /// 注册敌人英雄UI信息
        /// </summary>
        /// <param name="hero">注册的英雄对象</param>
        public virtual void RegisterEnemyHero(HeroMono hero){}

		/// <summary>
        /// 刷新UI
        /// </summary>
        /// <param name="hero">传入的刷新数据</param>
        public virtual void RefreshUI(HeroMono hero,Global.BuffType type,ValueUnit old_value,ValueUnit new_value){}


		/// <summary>
        /// 高亮一个英雄
        /// </summary>
        /// <param name="hero"></param>
        public virtual void SelectHero(HeroMono hero){}

		/// <summary>
        /// 取消所有高亮显示
        /// </summary>
        public virtual void DeselectHero(HeroMono hero){}

		/// <summary>
        /// 退出系统的时候调用
        /// </summary>
        public virtual void Clear(){}
		
		/// <summary>
        /// 英雄死亡时候回调用
        /// </summary>
		public virtual void HeroDead(HeroMono hero){}

        /// <summary>
        /// 当获得一个buff或者debuff的时候将会回调
        /// </summary>
        public virtual void OnAddBuff(HeroMono hero,Buff buff){}

        /// <summary>
        /// 当移除一个buff或者debuff的时候将会回调
        /// </summary>
        public virtual void OnRemoveBuff(HeroMono hero,Buff buff){}

        /// <summary>
        /// 当一个buff或者debuff执行一次的时候将会回调
        /// </summary>
        public virtual void OnBuffAction(HeroMono hero,Buff buff){}
    }
}