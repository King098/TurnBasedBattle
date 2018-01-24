using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King.TurnBasedCombat
{
    public interface IBattleHeroHub
    {
		/// <summary>
        /// 显示一个Hub
        /// </summary>
        /// <param name="hero">为哪个英雄显示</param>
        /// <param name="type">Hub类型</param>
        /// <param name="value">数值变化</param>
        void ShowHubInfo(HeroMono hero, HubType type, ValueUnit value = null);
    }
}