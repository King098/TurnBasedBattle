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
        /// 创建的英雄信息的预制体
        /// </summary>
        public GameObject BattleHeroUIPrefab;
        /// <summary>
        /// 玩家英雄对象父级
        /// </summary>
        public Transform PlayerHeroParent;
        /// <summary>
        /// 敌人英雄对象父级
        /// </summary>
        public Transform EnemyHeroParent;
        /// <summary>
        /// 玩家队伍的所有站位坐标
        /// </summary>
        public List<Transform> PlayerTeamPos;
        /// <summary>
        /// 敌人队伍的所有站位坐标
        /// </summary>
        public List<Transform> EnemyTeamPos;
        /// <summary>
        /// 用于创建战斗中的英雄的预制体对象
        /// </summary>
        public GameObject HeroPrefab;
        /// <summary>
        /// 玩家英雄队列父级
        /// </summary>
        public Transform PlayerTeam;
        /// <summary>
        /// 敌人英雄队列父级
        /// </summary>
        public Transform EnemyTeam;

        /// <summary>
        /// 初始化UI控制器
        /// </summary>
        public override void Init()
        {
            _HeroUI = new Dictionary<HeroMono, BattleHeroUI>();
            //清空玩家和敌人列表
            UnityStaticTool.DestoryChilds(PlayerHeroParent.gameObject, true);
            UnityStaticTool.DestoryChilds(EnemyHeroParent.gameObject, true);
        }

        /// <summary>
        /// 创建UI上的所有参与战斗的英雄Mono对象并向BattleController注册这个对象
        /// </summary>
        /// <param name="player">玩家队伍数据</param>
        /// <param name="enemy">敌人队伍数据</param>
        public override void CreateHeros(List<Hero> player,List<Hero> enemy)
        {
            //清除玩家和敌人队列的所有数据
            UnityStaticTool.DestoryChilds(PlayerTeam.gameObject, true);
            UnityStaticTool.DestoryChilds(EnemyTeam.gameObject, true);
            //创建玩家的英雄对象
            for (int i = 0; i < player.Count; i++)
            {
                HeroMono hero_temp = null;
                GameObject prefab = Resources.Load<GameObject>("Hero/" + player[i].ID);
                if (UnityStaticTool.CreateObjectReletiveTo<HeroMono>((prefab == null ? HeroPrefab : prefab), PlayerTeam, out hero_temp))
                {
                    hero_temp.Init(player[i], true);
                    hero_temp.transform.position = PlayerTeamPos[i].position;
                    //记录英雄坐标
                    hero_temp.HeroPosition = PlayerTeamPos[i].position;
                    //记录这个英雄的对象引用
                    BattleController.Instance.RegisterPlayerHeroMono(hero_temp);
                }
            }
            //创建传入进来的敌人对象
            for (int i = 0; i < enemy.Count; i++)
            {
                HeroMono enemy_temp = null;
                GameObject prefab = Resources.Load<GameObject>("Hero/" + player[i].ID);
                if (UnityStaticTool.CreateObjectReletiveTo<HeroMono>((prefab == null ? HeroPrefab : prefab), EnemyTeam, out enemy_temp))
                {
                    enemy_temp.Init(enemy[i], false);
                    enemy_temp.transform.position = EnemyTeamPos[i].position;
                    //记录英雄的坐标
                    enemy_temp.HeroPosition = EnemyTeamPos[i].position;
                    //记录这个敌人英雄的对象引用
                    BattleController.Instance.RegisterEnemyHeroMono(enemy_temp);
                }
            }
        }

        /// <summary>
        /// 注册玩家英雄UI信息
        /// </summary>
        /// <param name="hero">注册的英雄对象</param>
        public override void RegisterPlayerHero(HeroMono hero)
        {
            BattleHeroUI hero_UI = null;
            if (UnityStaticTool.CreateObjectReletiveTo<BattleHeroUI>(BattleHeroUIPrefab, PlayerHeroParent, out hero_UI))
            {
                BattleController.Instance.DebugLog(LogType.INFO,"Register player hero in UI successfully!");
                hero_UI.Init(hero);
                _HeroUI.Add(hero, hero_UI);
            }
            else
            {
                BattleController.Instance.DebugLog(LogType.INFO,"Failed to register player hero!");
            }
        }

        /// <summary>
        /// 注册敌人英雄UI信息
        /// </summary>
        /// <param name="hero">注册的英雄对象</param>
        public override void RegisterEnemyHero(HeroMono hero)
        {
            BattleHeroUI hero_UI = null;
            if (UnityStaticTool.CreateObjectReletiveTo<BattleHeroUI>(BattleHeroUIPrefab, EnemyHeroParent, out hero_UI))
            {
                BattleController.Instance.DebugLog(LogType.INFO,"Register enemy hero in UI successfully!");
                hero_UI.Init(hero);
                _HeroUI.Add(hero, hero_UI);
            }
            else
            {
                BattleController.Instance.DebugLog(LogType.INFO,"Failed to register enemy hero!");
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
            //清除玩家队列
            UnityStaticTool.DestoryChilds(PlayerTeam.gameObject, true);
            //清除敌人队列
            UnityStaticTool.DestoryChilds(EnemyTeam.gameObject, true);
            //清除玩家UI
            UnityStaticTool.DestoryChilds(PlayerHeroParent.gameObject, true);
            //清除敌人UI
            UnityStaticTool.DestoryChilds(EnemyHeroParent.gameObject, true);
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