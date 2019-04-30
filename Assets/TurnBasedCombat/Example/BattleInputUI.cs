using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using King.Tools;

namespace King.TurnBasedCombat
{
    /// <summary>
    /// 用于显示让玩家进行输入控制的UI显示
    /// </summary>
    public class BattleInputUI : MonoBehaviour
    {
        #region 单例设计
        private static BattleInputUI m_self = null;
        public static BattleInputUI Instance
        {
            get
            {
                return m_self;
            }
        }

        void Awake()
        {
            m_self = this;
            Init();
        }
        #endregion

        /// <summary>
        /// 用于显示Tips
        /// </summary>
        public Text TextTip;
        /// <summary>
        /// 菜单按钮的预制体
        /// </summary>
        public GameObject ButtonPrefab;
        /// <summary>
        /// 一级菜单网格
        /// </summary>
        public GridLayoutGroup FirstMenuGrid;
        /// <summary>
        /// 二级菜单网格
        /// </summary>
        public GridLayoutGroup SecondMenuGrid;
        /// <summary>
        /// 三级菜单网格
        /// </summary>
        public GridLayoutGroup ThirdMenuGrid;
        /// <summary>
        /// 一级菜单
        /// </summary>
        private List<Button> FirstMenu;
        /// <summary>
        /// 二级菜单
        /// </summary>
        private List<Button> SecondMenu;
        /// <summary>
        /// 三级菜单
        /// </summary>
        private List<Button> ThirdMenu;
        /// <summary>
        /// 当前创建的英雄控制面板
        /// </summary>
        private HeroMono _CurHero;

        /// <summary>
        /// 初始化输入UI
        /// </summary>
        public void Init()
        {
            FirstMenu = new List<Button>();
            SecondMenu = new List<Button>();
            ThirdMenu = new List<Button>();
            TextTip.text = "";
        }

        /// <summary>
        /// 显示一个输入UI
        /// </summary>
        /// <param name="hero">传入当前回合的英雄</param>
        public void ShowInputUI(HeroMono hero)
        {
            //保存这个英雄的信息
            _CurHero = hero;
            FirstMenu.Clear();
            //设置tips
            TextTip.text = "当前英雄回合:" + hero.Name;
            Button button = null;
            if (UnityStaticTool.CreateObjectReletiveTo<Button>(ButtonPrefab, FirstMenuGrid.transform, out button))
            {
                button.GetComponentInChildren<Text>().text = "技能";
                button.onClick.AddListener(OnSkillButtonClick);
            }
        }

        /// <summary>
        /// 点击了技能菜单
        /// </summary>
        void OnSkillButtonClick()
        {
            //首先关闭二级菜单和三级菜单
            UnityStaticTool.DestoryChilds(SecondMenuGrid.gameObject, true);
            UnityStaticTool.DestoryChilds(ThirdMenuGrid.gameObject, true);
            //然后创建新的二级菜单
            foreach(BaseSkill skill in _CurHero.Skills.Values)
            {
                if(skill.IsPassiveSkill)
                    continue;
                Button button = null;
                if (UnityStaticTool.CreateObjectReletiveTo<Button>(ButtonPrefab, SecondMenuGrid.transform, out button))
                {
                    button.name = skill.Name;
                    button.GetComponentInChildren<Text>().text = skill.Name;
                    if (!skill.CanUseSkill())
                    {
                        button.interactable = false;
                        continue;
                    }
                    button.onClick.AddListener(delegate { this.OnSkillChooseClick(button.gameObject); });
                }
            }
        }

        /// <summary>
        /// 选择了一个技能释放
        /// </summary>
        void OnSkillChooseClick(GameObject button)
        {
            BattleController.Instance.DebugLog(LogType.INFO,"选择了" + button.name);
            foreach (BaseSkill skill in _CurHero.Skills.Values)
            {
                if (skill.Name == button.name)
                {
                    _CurHero.Attack(skill.SkillType);
                    //关闭输入UI
                    DisappearInputUI();
                }
            }
        }

        /// <summary>
        /// 关闭输入UI
        /// </summary>
        public void DisappearInputUI()
        {
            UnityStaticTool.DestoryChilds(FirstMenuGrid.gameObject,true);
            UnityStaticTool.DestoryChilds(SecondMenuGrid.gameObject, true);
            UnityStaticTool.DestoryChilds(ThirdMenuGrid.gameObject, true);
            TextTip.text = "";
        }
    }
}