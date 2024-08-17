using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using King.Tools;
using AEventManager;
using System;
using System.Text;
using static King.TurnBasedCombat.EventsConst;

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
        /// 选择目标的面板
        /// </summary>
        public GameObject ChooseTarget;
        /// <summary>
        /// 目标选择的提示
        /// </summary>
        public Text ChooseTargetTip;
        /// <summary>
        /// 取消技能释放，重新选择
        /// </summary>
        public Button BtnCancleSkill;
        /// <summary>
        /// 确认释放技能按钮
        /// </summary>
        public Button BtnUseSkill;

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
        /// 当前选择的使用的技能
        /// </summary>
        private BaseSkill _CurSkill;
        /// <summary>
        /// 当前技能选择的目标对象
        /// </summary>
        private List<HeroMono> _CurChooseTargetHeros = new List<HeroMono>();

        /// <summary>
        /// 初始化输入UI
        /// </summary>
        public void Init()
        {
            FirstMenu = new List<Button>();
            SecondMenu = new List<Button>();
            ThirdMenu = new List<Button>();
            TextTip.text = "";
            this.ChooseTarget.SetActive(false);
        }

        void OnEnable()
        {
            EventManager.Instance.AddEvent(EventsConst.OnHeroTargetChoosed,_OnHeroTargetChoosed);
        }

        void OnDisable()
        {
            EventManager.Instance.RemoveEvent(EventsConst.OnHeroTargetChoosed,_OnHeroTargetChoosed);
        }

        private void _OnHeroTargetChoosed(object sender, EventArgs e)
        {
            CommonHeroMonoEventArgs args = e as CommonHeroMonoEventArgs; 
            if(_CurChooseTargetHeros.Contains(args.hero))
            {
                //已经选中的英雄，再次点击将会取消选中
                _CurChooseTargetHeros.Remove(args.hero);
                //将英雄高亮设置为可选状态
                args.hero.HeroCanChoose();
                if(_CurChooseTargetHeros.Count == 0)
                {
                    this.BtnUseSkill.interactable = false;
                }
                return;
            }
            if(_CurSkill.TargetNumber == _CurChooseTargetHeros.Count)
            {
                Debug.Log("已经选择了足够多的目标，将会移除第一个选择目标然后添加新的目标");
                //已经选中的英雄，再次点击将会取消选中
                HeroMono heroMono = _CurChooseTargetHeros[0];
                _CurChooseTargetHeros.Remove(heroMono);
                //将英雄高亮设置为可选状态
                heroMono.HeroCanChoose();
            }
            //添加这个目标的选择
            _CurChooseTargetHeros.Add(args.hero);
            //高亮这个目标
            args.hero.HeroChoose();
            this.BtnUseSkill.interactable = true;
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
            this.HideChooseTargetsUI();
        }

        /// <summary>
        /// 展示界面上选择技能目标对象
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="skill"></param>
        public void ShowInputTargetsUI(HeroMono hero,BaseSkill skill)
        {
            _CurHero = hero;
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
                    //这里不能直接Attack了，要玩家手动选择目标才可以攻击目标对象
                    // _CurHero.Attack(skill.SkillType);
                    //关闭输入UI
                    DisappearInputUI();
                    //打开选择目标的提示面板
                    ShowChooseTargetsUI(skill);
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

        /// <summary>
        /// 显示选择目标的面板提示和操作
        /// </summary>
        public void ShowChooseTargetsUI(BaseSkill skill)
        {
            this._CurSkill = skill;
            this.ChooseTarget.SetActive(true);
            this.BtnCancleSkill.onClick.RemoveAllListeners();
            this.BtnCancleSkill.onClick.AddListener(()=>{
                this.HideChooseTargetsUI();
                ShowInputUI(_CurHero);
            });
            this.BtnUseSkill.onClick.RemoveAllListeners();
            this.BtnUseSkill.onClick.AddListener(()=>{
                this.HideChooseTargetsUI();
                _CurHero.Attack(_CurSkill.SkillType,_CurChooseTargetHeros);
            });
            StringBuilder stringBuilder = new StringBuilder();
            Debug.Log(string.Format("目标类型：{0}", skill.TargetType.ToString()));
            stringBuilder.AppendLine(string.Format("目标类型：{0}", skill.TargetType.ToString()));
            Debug.Log(string.Format("目标数量：{0}",skill.TargetNumber));
            stringBuilder.AppendLine(string.Format("目标数量：{0}",skill.TargetNumber));
            Debug.Log(string.Format("是否可选被控:{0}",skill.TargetContainsControlledHero));
            stringBuilder.AppendLine(string.Format("是否可选被控:{0}",skill.TargetContainsControlledHero));
            this.ChooseTargetTip.text = stringBuilder.ToString();
            this._CurChooseTargetHeros.Clear();
            this.BtnUseSkill.interactable = false;
            //高亮可选目标
            BattleController.Instance.HighlightTargets(_CurHero,skill.TargetType,skill.TargetContainsControlledHero);
        }

        /// <summary>
        /// 关闭选择目标的提示面板
        /// </summary>
        public void HideChooseTargetsUI()
        {
            this.ChooseTarget.SetActive(false);
            BattleController.Instance?.DisableChooseHeroes();
        }
    }
}