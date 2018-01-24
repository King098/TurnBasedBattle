using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King.TurnBasedCombat
{
	/// <summary>
	/// 玩家输入基类
	/// </summary>
    public interface IInputController
    {

		/// <summary>
        /// 输入控制器初始化
        /// </summary>
        void Init();

        /// <summary>
        /// 控制器调用表示开始接收一个玩家输入
        /// </summary>
        void WaitForInput(HeroMono hero);
    }
}