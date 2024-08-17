using System;
using System.Collections;
using System.Collections.Generic;
using AEventManager;
using UnityEngine;
using static King.TurnBasedCombat.EventsConst;

namespace King.TurnBasedCombat
{
    public class BaseBattleUI : MonoBehaviour , IBattleUI
    {
        protected virtual void OnDisable()
        {
            EventManager.Instance.RemoveEvent(EventsConst.OnInitSystem,_Init);
            EventManager.Instance.RemoveEvent(EventsConst.OnCombatBattleStart,_OnBattleStart);
            EventManager.Instance.RemoveEvent(EventsConst.OnHeroPropertyChanged,_OnHeroPropertyChanged);
            EventManager.Instance.RemoveEvent(EventsConst.OnBeforeAction,_OnBeforeAction);
            EventManager.Instance.RemoveEvent(EventsConst.OnAfterAction,_OnAfterAction);
            EventManager.Instance.RemoveEvent(EventsConst.OnHeroDead,_OnHeroDead);
            EventManager.Instance.RemoveEvent(EventsConst.OnAddBuff,_OnAddBuff);
            EventManager.Instance.RemoveEvent(EventsConst.OnRemoveBuff,_OnRemoveBuff);
            EventManager.Instance.RemoveEvent(EventsConst.OnBuffAction,_OnBuffAction);
            EventManager.Instance.RemoveEvent(EventsConst.OnExitSystem,_OnExitSystem);
        }
        protected virtual void OnEnable()
        {
            EventManager.Instance.AddEvent(EventsConst.OnInitSystem,_Init);
            EventManager.Instance.AddEvent(EventsConst.OnCombatBattleStart,_OnBattleStart);
            EventManager.Instance.AddEvent(EventsConst.OnHeroPropertyChanged,_OnHeroPropertyChanged);
            EventManager.Instance.AddEvent(EventsConst.OnBeforeAction,_OnBeforeAction);
            EventManager.Instance.AddEvent(EventsConst.OnAfterAction,_OnAfterAction);
            EventManager.Instance.AddEvent(EventsConst.OnHeroDead,_OnHeroDead);
            EventManager.Instance.AddEvent(EventsConst.OnAddBuff,_OnAddBuff);
            EventManager.Instance.AddEvent(EventsConst.OnRemoveBuff,_OnRemoveBuff);
            EventManager.Instance.AddEvent(EventsConst.OnBuffAction,_OnBuffAction);
            EventManager.Instance.AddEvent(EventsConst.OnExitSystem,_OnExitSystem);
        }

        private void _OnExitSystem(object sender, EventArgs e)
        {
            this.Clear();
        }

        private void _OnBuffAction(object sender, EventArgs e)
        {
            CommonHeroBuffEventArgs args = e as CommonHeroBuffEventArgs;
            this.OnBuffAction(args.hero,args.buff);
        }

        private void _OnRemoveBuff(object sender, EventArgs e)
        {
            CommonHeroBuffEventArgs args = e as CommonHeroBuffEventArgs;
            this.OnRemoveBuff(args.hero,args.buff);
        }

        private void _OnAddBuff(object sender, EventArgs e)
        {
            CommonHeroBuffEventArgs args = e as CommonHeroBuffEventArgs;
            this.OnAddBuff(args.hero,args.buff);
        }

        private void _OnHeroDead(object sender, EventArgs e)
        {
            CommonHeroMonoEventArgs args = e as CommonHeroMonoEventArgs;
            this.HeroDead(args.hero);
        }

        private void _OnAfterAction(object sender, EventArgs e)
        {
            CommonHeroMonoEventArgs args = e as CommonHeroMonoEventArgs;
            this.DeselectHero(args.hero);
        }

        private void _OnBeforeAction(object sender, EventArgs e)
        {
            CommonHeroMonoEventArgs args = e as CommonHeroMonoEventArgs;
            this.SelectHero(args.hero);
        }

        private void _OnHeroPropertyChanged(object sender, EventArgs e)
        {
            HeroPropertyEventArgs args = e as HeroPropertyEventArgs;
            this.RefreshUI(args.hero,args.type,args.oldValue,args.newValue);
        }

        private void _OnBattleStart(object sender, EventArgs e)
        {
            CombatBattleStartEventArgs args = e as CombatBattleStartEventArgs;
            this.CreateHeros(args.teams);
        }

        private void _Init(object sender, EventArgs e)
        {
            Init();
        }

        /// <summary>
        /// 初始化UI控制器
        /// </summary>
        public virtual void Init(){}

        /// <summary>
        /// 使用阵营对象创建UI上所需的英雄面板
        /// </summary>
        /// <param name="teams"></param>
        public virtual void CreateHeros(List<HeroTeamMono> teams){}

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