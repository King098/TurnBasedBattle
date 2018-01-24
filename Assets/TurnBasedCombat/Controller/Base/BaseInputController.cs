using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace King.TurnBasedCombat
{
	/// <summary>
	/// 玩家输入基类
	/// </summary>
    public class BaseInputController : MonoBehaviour , IInputController
    {
		/// <summary>
        /// 是否正在等待玩家输入
        /// </summary>
        protected bool _IsWaitingInput;

		/// <summary>
        /// 输入控制器初始化
        /// </summary>
        public virtual void Init()
        {
            _IsWaitingInput = false;
        }

        /// <summary>
        /// 控制器调用表示开始接收一个玩家输入
        /// </summary>
        public virtual void WaitForInput(HeroMono hero)
        {
            _IsWaitingInput = true;
        }
    }
}