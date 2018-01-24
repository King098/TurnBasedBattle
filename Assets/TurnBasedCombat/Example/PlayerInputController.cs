using UnityEngine;
using System.Collections;

namespace King.TurnBasedCombat
{
    /// <summary>
    /// 玩家的输入类（用于控制己方战斗过程）
    /// </summary>
    public class PlayerInputController : BaseInputController
    {
        /// <summary>
        /// 输入控制器初始化
        /// </summary>
        public override void Init()
        {
            _IsWaitingInput = false;
        }

        /// <summary>
        /// 控制器调用表示开始接收一个玩家输入
        /// </summary>
        public override void WaitForInput(HeroMono hero)
        {
            _IsWaitingInput = true;
            //显示输入UI
            BattleInputUI.Instance.ShowInputUI(hero);
        }
    }
}