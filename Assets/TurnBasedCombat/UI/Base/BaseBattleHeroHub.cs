using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King.TurnBasedCombat
{
    public enum HubType
    {
        Critical = 1,
        Miss = 2,
        DecreseLife = 3,
        IncreseLife = 4,
        DecreseMagic = 5,
        IncreseMagic = 6,
        IncreseAttack = 7,
        DecreseAttack = 8,
        IncreseDefense = 9,
        DecreseDefense = 10,
        IncreseMagicAttack = 11,
        DecreseMagicAttack = 12,
        IncreseMagicDefense = 13,
        DecreseMagicDefense = 14,
        IncreseMaxLife = 15,
        DecreseMaxLife = 16,
        IncreseMaxMagic = 17,
        DecreseMaxMagic = 18,
        IncreseSpeed = 19,
        DecreseSpeed = 20,
    }

    [System.Serializable]
    public class HeroHubInfo
    {
        public HubType type;
        public string Name;
        public Color TextColor;
    }
    public class BaseBattleHeroHub : MonoBehaviour , IBattleHeroHub
    {
		/// <summary>
        /// 所有可能出现的Hub信息
        /// </summary>
        public List<HeroHubInfo> Hubs;

		/// <summary>
        /// 根据Hub类型获取Hub属性
        /// </summary>
        /// <param name="type">Hub类型</param>
        /// <returns>返回此Hub的信息</returns>
        protected virtual HeroHubInfo GetHubInfoByType(HubType type)
        {
            for (int i = 0; i < Hubs.Count; i++)
            {
                if (Hubs[i].type == type)
                {
                    return Hubs[i];
                }
            }
            return null;
        }


		/// <summary>
        /// 显示一个Hub
        /// </summary>
        /// <param name="hero">为哪个英雄显示</param>
        /// <param name="type">Hub类型</param>
        /// <param name="value">数值变化</param>
        public virtual void ShowHubInfo(HeroMono hero, HubType type, ValueUnit value = null)
        {

        }
    }
}