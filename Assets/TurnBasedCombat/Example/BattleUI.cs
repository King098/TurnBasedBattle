using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using King.Tools;
using UnityEngine.UI;

namespace King.TurnBasedCombat
{
    /// <summary>
    /// 用于显示战斗的UI控制器
    /// </summary>
    public class BattleUI : BaseBattleUI
    {
        /// <summary>
        /// 用于记录每个英雄对应的UI对象，产生关联
        /// </summary>
        private Dictionary<HeroMono, BattleHeroUI> _HeroUI;

        /// <summary>
        /// 所有阵营UI展示的父级节点对象
        /// </summary>
        public List<Transform> TeamSlotRoots;

        /// <summary>
        /// 创建的英雄信息的预制体
        /// </summary>
        public GameObject BattleHeroUIPrefab;

        /// <summary>
        /// 初始化UI控制器
        /// </summary>
        public override void Init()
        {
            
            
        }

        /// <summary>
        /// 生成每个阵营的UI对象并关联HeroMono
        /// </summary>
        /// <param name="teams"></param>
        public override void CreateHeros(List<HeroTeamMono> teams)
        {
            _HeroUI = new Dictionary<HeroMono, BattleHeroUI>();
            for(int i = 0;i<TeamSlotRoots.Count;i++)
            {
                UnityStaticTool.DestoryChilds(TeamSlotRoots[i].gameObject);
            }
            for(int i = 0;i<teams.Count;i++)
            {
                for(int j = 0;j<teams[i].Heros.Count;j++)
                {
                    HeroMono heroMono = teams[i].Heros[j];
                    GameObject go = GameObject.Instantiate(BattleHeroUIPrefab, TeamSlotRoots[i],false);
                    BattleHeroUI ui = go.GetComponent<BattleHeroUI>();
                    ui.Init(heroMono);
                    _HeroUI.Add(heroMono,ui);
                }
            }
        }

        /// <summary>
        /// 刷新UI
        /// </summary>
        /// <param name="hero">传入的刷新数据</param>
        public override void RefreshUI(HeroMono hero,Global.BuffType type,ValueUnit old_value,ValueUnit new_value)
        {
            if (_HeroUI.ContainsKey(hero))
            {
                _HeroUI[hero].Init(hero);
            }
        }

        /// <summary>
        /// 高亮一个英雄
        /// </summary>
        /// <param name="hero"></param>
        public override void SelectHero(HeroMono hero)
        {
            _HeroUI[hero].Select();
        }

        /// <summary>
        /// 取消所有高亮显示
        /// </summary>
        public override void DeselectHero(HeroMono hero)
        {
            _HeroUI[hero].Deselect();
        }

        /// <summary>
        /// 退出系统的时候调用
        /// </summary>
        public override void Clear()
        {
            //清除关联字典
            _HeroUI.Clear();
        }

        /// <summary>
        /// 英雄死亡时候回调用
        /// </summary>
        public override void HeroDead(HeroMono hero)
        {
            BattleController.Instance.DebugLog(LogType.WARNING,"英雄" + hero.Name + "死亡");
        }

        /// <summary>
        /// 当获得一个buff或者debuff的时候将会回调
        /// </summary>
        public override void OnAddBuff(HeroMono hero,Buff buff)
        {
            BattleController.Instance.DebugLog(LogType.INFO,"英雄 "+ hero.Name + " 增加了一个buff " + buff.Name);
        }

        /// <summary>
        /// 当移除一个buff或者debuff的时候将会回调
        /// </summary>
        public override void OnRemoveBuff(HeroMono hero,Buff buff)
        {
            BattleController.Instance.DebugLog(LogType.INFO,"英雄 "+ hero.Name + " 移除了一个buff " + buff.Name);
        }

        /// <summary>
        /// 当一个buff或者debuff执行一次的时候将会回调
        /// </summary>
        public override void OnBuffAction(HeroMono hero,Buff buff)
        {
            BattleController.Instance.DebugLog(LogType.INFO,"英雄 "+ hero.Name + " 执行了一个buff " + buff.Name);
        }
    }
}