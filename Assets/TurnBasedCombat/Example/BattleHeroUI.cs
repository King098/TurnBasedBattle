using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace King.TurnBasedCombat
{
    public class BattleHeroUI : BaseBattleHeroUI
    {
        public Image ChooseImg;
        public Image HeroIco;
        public Text TextName;
        public Text TextHP;
        public Text TextMP;

        public override void Init(HeroMono hero)
        {
            ChooseImg.enabled = false;
            TextName.text = hero.Name;
            King.Tools.UnityStaticTool.LoadResources<Sprite>(hero.Hero.Ico, (img) =>
             {
                 if (img != null)
                 {
                     HeroIco.sprite = img;
                 }
             });
            TextHP.text = hero.CurrentLife + "/" + hero.CurrentMaxLife;
            TextMP.text = hero.CurrentMagic + "/" + hero.CurrentMaxMagic;
        }

        /// <summary>
        /// 当这个英雄的回合的时候调用此UI变化
        /// </summary>
        public override void Select()
        {
            ChooseImg.enabled = true;
        }
        /// <summary>
        /// 当这个英雄回合结束的时候调用
        /// </summary>
        public override void Deselect()
        {
            ChooseImg.enabled = false;
        }

        /// <summary>
        /// 当获得一个buff或者debuff的时候将会回调
        /// </summary>
        public override void OnAddBuff(Buff buff)
        {
            BattleController.Instance.DebugLog(LogType.INFO,"增加一个Buff：" + buff.Name);
        }

        /// <summary>
        /// 当移除一个buff或者debuff的时候将会回调
        /// </summary>
        public override void OnRemoveBuff(Buff buff)
        {
            BattleController.Instance.DebugLog(LogType.INFO,"移除一个Buff：" + buff.Name);
        }

        /// <summary>
        /// 当一个buff或者debuff执行一次的时候将会回调
        public override void OnBuffAction(Buff buff)
        {
            BattleController.Instance.DebugLog(LogType.INFO,"执行一个Buff：" + buff.Name + " 剩余回合数:"+buff.StayTurn);
        }
    }
}