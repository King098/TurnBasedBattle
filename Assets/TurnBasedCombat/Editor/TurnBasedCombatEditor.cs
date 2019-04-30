using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace King.TurnBasedCombat
{
    public class TurnBasedCombatEditor : EditorWindow
    {
        private string[] ToolBars = new string[] { "英雄配置", "技能配置", "Buff/Debuff配置" };
        private int CurrentSelectIndex = 0;

        #region HeroTable
        private Vector2 HeroTableLeftScrollBar;
        private string HeroTableLeftSelectHero;
        private string HeroTableCreateIDText = "在此输入新英雄ID";
        private float HeroTableLabelWidth = 200f;
        private int HeroTableLeftSelectIndex;
        private Vector2 HeroTableRightScrollBar;
        private string CurrentHeroIco;
        private Sprite HeroInfoIco;
        #endregion
        #region SkillTable
        private Vector2 SkillTableLeftScrollBar;
        private int SkillTableLeftSelectIndex;
        private string SkillTableLeftSelectSkillID;
        private int SkillTableLeftSelectSkillLevel;
        private string SkillTableCreateIDText = "在此输入新技能ID";
        private int SkillTableCreateLevelText = 0;
        private Vector2 SkillTableRightScrollBar;
        private float SkillTableLabelWidth = 200f;
        private bool SkillTableCostPropertyFold = false;
        private bool SkillTableCausePropertyFold = false;
        private bool SkillTableHurtPropertyFold = false;
        private string CurrentSkillIco;
        private Sprite SkillInfoIco;
        private TextAsset SkillInfoMono;
        private Object SkillEffect;
        #endregion
        #region Buff/Deff Table
        private Vector2 BuffTableLeftScrollBar;
        private string BuffTableLeftSelectBuff = "";
        private int BuffTableLeftSelectIndex = 0;
        private string BuffTableCreateIDText = "在此输入新ID";
        private Vector2 BuffTableRightScrollBar;
        private float BuffTableLabelWidth = 200f;
        private bool BuffTableFoldShow = false;
        #endregion

        private static TurnBasedCombatEditor window;

        [MenuItem("Turn Based Combat/Turn Based Combat (Editor)")]
        static void Init()
        {
            window = EditorWindow.GetWindow<TurnBasedCombatEditor>("回合制游戏设计");
            TableController.Instance.LoadAllTable();
        }

        void OnDestroy()
        {
            Debug.Log("关闭编辑器");
            SkillChooseWindow.CloseWindow();
            BuffChooseWindow.CloseWindow();
        }

        void OnGUI()
        {

            GUILayout.BeginVertical();
            {
                //ToolBar
                CurrentSelectIndex = GUILayout.Toolbar(CurrentSelectIndex, ToolBars);
                //分隔
                GUILayout.Space(10f);
                //渲染下层
                if (CurrentSelectIndex == 0)
                {
                    //英雄表配置
                    HeroTableGUI();
                }
                else if (CurrentSelectIndex == 1)
                {
                    //技能表配置
                    SkillTableGUI();
                }
                else if (CurrentSelectIndex == 2)
                {
                    //Buff/Debuff配置
                    BuffTableGUI();
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// 渲染英雄表配置界面
        /// </summary>
        void HeroTableGUI()
        {
            GUILayout.BeginHorizontal();
            {
                //左侧英雄列表
                GUILayout.BeginVertical(GUILayout.MinWidth(100f), GUILayout.MaxWidth(300f));
                {
                    //显示一个Label
                    GUILayout.Label("英雄列表");
                    //获取将要显示的英雄按钮信息 
                    string[] Heros = GetNamesInHeroTable();
                    //英雄列表
                    HeroTableLeftScrollBar = GUILayout.BeginScrollView(HeroTableLeftScrollBar);
                    {
                        if (Heros != null && Heros.Length > 0)
                        {
                            HeroTableLeftSelectIndex = GUILayout.SelectionGrid(HeroTableLeftSelectIndex, Heros, 1);
                            //强制让其不会访问越界
                            if (HeroTableLeftSelectIndex >= Heros.Length)
                            {
                                HeroTableLeftSelectIndex = Heros.Length - 1;
                            }
                            //记录选择的英雄的ID
                            HeroTableLeftSelectHero = Heros[HeroTableLeftSelectIndex].Split('|')[0].Trim();
                        }
                    }
                    GUILayout.EndScrollView();
                    //分隔
                    GUILayout.FlexibleSpace();
                    //增加英雄的ID
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("新英雄的ID", GUILayout.Width(100f));
                        HeroTableCreateIDText = GUILayout.TextField(HeroTableCreateIDText);
                    }
                    GUILayout.EndHorizontal();
                    //增加新英雄按钮
                    if (GUILayout.Button(new GUIContent("添加新英雄")))
                    {
                        if (HeroTable.Instance.list.ContainsKey(HeroTableCreateIDText))
                        {
                            if (EditorUtility.DisplayDialog("提示", HeroTableCreateIDText + " 已经存在，不能创建相同ID的英雄!", "确定"))
                            {
                                HeroTableCreateIDText = "";
                            }
                        }
                        else
                        {
                            //创建新的英雄数据,并添加进入数据列表中
                            Hero hero = new Hero(HeroTableCreateIDText);
                            HeroTable.Instance.list.Add(hero.ID, hero);
                            //设置按钮选中状态
                            Heros = GetNamesInHeroTable();
                            if (Heros != null && Heros.Length > 0)
                            {
                                HeroTableLeftSelectIndex = Heros.Length - 1;
                            }
                            //将选中新的英雄
                            HeroTableLeftSelectHero = hero.ID;
                            //清除ID数据
                            HeroTableCreateIDText = "";
                        }
                    }
                }
                GUILayout.EndVertical();

                //右侧详细显示
                GUILayout.BeginVertical();
                {
                    if (string.IsNullOrEmpty(HeroTableLeftSelectHero))
                    {
                        GUILayout.Label("当前没有选中任何英雄");
                    }
                    else
                    {
                        Hero h = HeroTable.Instance.GetHeroByID(HeroTableLeftSelectHero);
                        if (h == null)
                        {
                            GUILayout.Label("当前选中的英雄ID" + HeroTableLeftSelectHero + " 的英雄不存在或者获取失败!");
                        }
                        else
                        {
                            if (CurrentHeroIco != h.ID)
                            {
                                CurrentHeroIco = h.ID;
                                //加载英雄头像
                                if (!string.IsNullOrEmpty(h.Ico.Trim()))
                                {
                                    HeroInfoIco = Resources.Load<Sprite>(h.Ico);
                                }
                                else
                                {
                                    HeroInfoIco = null;
                                }
                            }
                            #region 右侧英雄详细信息
                            HeroTableRightScrollBar = GUILayout.BeginScrollView(HeroTableRightScrollBar);
                            {
                                //一个Label提示
                                GUILayout.Label("当前英雄ID（不可更改，只能通过表格本身修改）: " + h.ID);
                                //分隔
                                GUILayout.Space(10f);
                                //英雄对应的图标
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("英雄的头像 : ", GUILayout.Width(HeroTableLabelWidth));
                                    GUILayout.BeginVertical();
                                    {
                                        HeroInfoIco = EditorGUILayout.ObjectField(HeroInfoIco, typeof(Sprite), true) as Sprite;
                                        if (HeroInfoIco != null)
                                        {
                                            if (!AssetDatabase.GetAssetPath(HeroInfoIco.GetInstanceID()).StartsWith("Assets/TurnBasedCombat/Resources/"))
                                            {
                                                Color color = GUI.color;
                                                GUI.color = Color.red;
                                                GUILayout.Label("请将头像文件放在Assets/TurnBasedCombat/Resources/ 目录下，否则将不会生效");
                                                GUI.color = color;
                                            }
                                            GUILayout.Label("头像路径:" + AssetDatabase.GetAssetPath(HeroInfoIco.GetInstanceID()).Replace("Assets/TurnBasedCombat/Resources/", "").Split(new string[] { "." }, System.StringSplitOptions.RemoveEmptyEntries)[0]);
                                            GUILayout.Label(HeroInfoIco.texture);
                                            h.Ico = AssetDatabase.GetAssetPath(HeroInfoIco.GetInstanceID()).Replace("Assets/TurnBasedCombat/Resources/", "").Split(new string[] { "." }, System.StringSplitOptions.RemoveEmptyEntries)[0];
                                        }
                                        else
                                        {
                                            GUILayout.Label("头像路径: ??? ");
                                            h.Ico = "";
                                        }
                                    }
                                    GUILayout.EndVertical();
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //英雄名字
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("英雄的名字 : ", GUILayout.Width(HeroTableLabelWidth));
                                    h.Name = EditorGUILayout.TextField(h.Name);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //英雄的真名
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("英雄的真名 : ", GUILayout.Width(HeroTableLabelWidth));
                                    h.RealName = EditorGUILayout.TextField(h.RealName);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //最初等级
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("英雄最初等级 : ", GUILayout.Width(HeroTableLabelWidth));
                                    h.Level = EditorGUILayout.IntField(h.Level);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //英雄星级
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("英雄最初星级 : ", GUILayout.Width(HeroTableLabelWidth));
                                    h.StarLevel = (Global.StarLevel)EditorGUILayout.EnumPopup(h.StarLevel);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //基础生命值
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("英雄生命值上限 : ", GUILayout.Width(HeroTableLabelWidth));
                                    h.BaseLife = EditorGUILayout.LongField(h.BaseLife);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //基础魔力值
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("英雄魔力值上限 : ", GUILayout.Width(HeroTableLabelWidth));
                                    h.BaseMagic = EditorGUILayout.LongField(h.BaseMagic);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //基础物理攻击
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("英雄物理攻击 : ", GUILayout.Width(HeroTableLabelWidth));
                                    h.BaseAttack = EditorGUILayout.LongField(h.BaseAttack);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //基础物理攻击
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("英雄物理防御 : ", GUILayout.Width(HeroTableLabelWidth));
                                    h.BaseDefense = EditorGUILayout.LongField(h.BaseDefense);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //基础魔法攻击
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("英雄魔法攻击 : ", GUILayout.Width(HeroTableLabelWidth));
                                    h.BaseMagicAttack = EditorGUILayout.LongField(h.BaseMagicAttack);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //基础魔法攻击
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("英雄魔法防御 : ", GUILayout.Width(HeroTableLabelWidth));
                                    h.BaseMagicDefense = EditorGUILayout.LongField(h.BaseMagicDefense);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //基础速度
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("英雄速度 : ", GUILayout.Width(HeroTableLabelWidth));
                                    h.BaseSpeed = EditorGUILayout.LongField(h.BaseSpeed);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //技能列表
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("英雄初始技能 : ", GUILayout.Width(HeroTableLabelWidth));
                                    GUILayout.BeginVertical();
                                    {
                                        //TODO:添加一个技能,打开一个技能选择窗口,然后直接在里面添加
                                        if (GUILayout.Button("技能修改器"))
                                        {
                                            SkillChooseWindow.InitWindow(h);
                                        }
                                        GUILayout.Space(10f);
                                        GetHeroSkillsInHeroTable(h.ID);
                                    }
                                    GUILayout.EndVertical();
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //英雄描述
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("英雄描述 : ", GUILayout.Width(HeroTableLabelWidth));
                                    h.Description = EditorGUILayout.TextArea(h.Description, GUILayout.MinHeight(HeroTableLabelWidth));
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //添加保存和删除的按钮
                                GUILayout.BeginHorizontal();
                                {
                                    if (GUILayout.Button("保存英雄", GUILayout.Width(HeroTableLabelWidth)))
                                    {
                                        //保存数据
                                        HeroTable.Instance.list[h.ID] = h;
                                        //还有一步保存到表格中去
                                        SaveHeroTable();
                                    }
                                    if (GUILayout.Button("删除英雄", GUILayout.Width(HeroTableLabelWidth)))
                                    {
                                        //删除数据
                                        HeroTable.Instance.list.Remove(h.ID);
                                        //重新获取名字列表
                                        string[] Heros = GetNamesInHeroTable();
                                        if (Heros != null && Heros.Length > 0)
                                        {
                                            //重新找一个
                                            HeroTableLeftSelectHero = Heros[0].Split('|')[0].Trim();
                                        }
                                        //还有一步保存到表格中去
                                        SaveHeroTable();
                                    }
                                }
                                GUILayout.EndHorizontal();
                            }
                            GUILayout.EndScrollView();
                            #endregion
                        }
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 技能表显示
        /// </summary>
        void SkillTableGUI()
        {
            GUILayout.BeginHorizontal();
            {
                #region 左侧列表
                //左侧的列表
                GUILayout.BeginVertical(GUILayout.MinWidth(100f), GUILayout.MaxWidth(300f));
                {
                    //显示一个Label
                    GUILayout.Label("技能列表");
                    //获取将要显示的技能按钮信息 
                    string[] skills_names = GetNamesInSkillTable();
                    //技能列表
                    SkillTableLeftScrollBar = GUILayout.BeginScrollView(SkillTableLeftScrollBar);
                    {
                        if (skills_names != null && skills_names.Length > 0)
                        {
                            SkillTableLeftSelectIndex = GUILayout.SelectionGrid(SkillTableLeftSelectIndex, skills_names, 1);
                            //强制让其不会访问越界
                            if (SkillTableLeftSelectIndex >= skills_names.Length)
                            {
                                SkillTableLeftSelectIndex = skills_names.Length - 1;
                            }
                            //记录选择的英雄的ID和等级
                            string[] skills_names_spilt = skills_names[SkillTableLeftSelectIndex].Split('|');
                            SkillTableLeftSelectSkillID = skills_names_spilt[0].Trim();
                            SkillTableLeftSelectSkillLevel = int.Parse(skills_names_spilt[1].Trim());
                        }
                    }
                    GUILayout.EndScrollView();
                    //分隔
                    GUILayout.FlexibleSpace();
                    //增加技能的ID和等级
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("新技能的ID", GUILayout.Width(100f));
                        SkillTableCreateIDText = GUILayout.TextField(SkillTableCreateIDText);
                    }
                    GUILayout.EndHorizontal();
                    //增加技能的ID和等级
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("新技能的等级", GUILayout.Width(100f));
                        SkillTableCreateLevelText = EditorGUILayout.IntField(SkillTableCreateLevelText);
                    }
                    GUILayout.EndHorizontal();
                    //增加新英雄按钮
                    if (GUILayout.Button(new GUIContent("添加新技能")))
                    {
                        if (SkillTable.Instance.list.ContainsKey(SkillTableCreateIDText) && SkillTable.Instance.list[SkillTableCreateIDText].ContainsKey(SkillTableCreateLevelText))
                        {
                            if (EditorUtility.DisplayDialog("提示", SkillTableCreateIDText + " " + SkillTableCreateLevelText + "级已经存在，不能创建技能!", "确定"))
                            {
                                SkillTableCreateIDText = "";
                                SkillTableCreateLevelText = 0;
                            }
                        }
                        else
                        {
                            //创建新的技能数据,并添加进入数据列表中
                            Skill skill = new Skill(SkillTableCreateIDText, SkillTableCreateLevelText);
                            if (SkillTable.Instance.list.ContainsKey(skill.ID))
                            {
                                SkillTable.Instance.list[skill.ID].Add(skill.Level, skill);
                            }
                            else
                            {
                                SkillTable.Instance.list.Add(skill.ID, new Dictionary<int, Skill>() { { skill.Level, skill } });
                            }
                            //设置按钮选中状态
                            skills_names = GetNamesInSkillTable();
                            if (skills_names != null && skills_names.Length > 0)
                            {
                                SkillTableLeftSelectIndex = skills_names.Length - 1;
                            }
                            //将选中新的英雄
                            SkillTableLeftSelectSkillID = skill.ID;
                            SkillTableLeftSelectSkillLevel = skill.Level;
                            //清除ID数据
                            SkillTableLeftSelectSkillID = "";
                            SkillTableLeftSelectSkillLevel = 0;
                        }
                    }
                }
                GUILayout.EndVertical();
                #endregion

                #region 右侧显示
                //右侧的列表
                GUILayout.BeginVertical();
                {
                    if (string.IsNullOrEmpty(SkillTableLeftSelectSkillID))
                    {
                        GUILayout.Label("当前没有选择任何技能");
                    }
                    else
                    {
                        Skill skill = SkillTable.Instance.GetSkillByIDAndLevel(SkillTableLeftSelectSkillID, SkillTableLeftSelectSkillLevel);
                        if (skill == null)
                        {
                            GUILayout.Label("没有加载到相关技能信息，或者加载失败");
                        }
                        else
                        {
                            if (CurrentSkillIco != skill.ID + "|" + skill.Level)
                            {
                                CurrentSkillIco = skill.ID + "|" + skill.Level;
                                //加载图标
                                if (!string.IsNullOrEmpty(skill.Ico))
                                {
                                    SkillInfoIco = Resources.Load<Sprite>(skill.Ico);
                                }
                                else
                                {
                                    SkillInfoIco = null;
                                }
                                //加载脚本
                                if (!string.IsNullOrEmpty(skill.SkillMono) && skill.SkillMono != "BaseSkill")
                                {
                                    SkillInfoMono = AssetDatabase.LoadAssetAtPath("Assets/TurnBasedCombat/Skills/" + skill.SkillMono + ".cs", typeof(TextAsset)) as TextAsset;
                                }
                                else
                                {
                                    SkillInfoMono = null;
                                }
                            }
                            //详细信息列表
                            SkillTableRightScrollBar = GUILayout.BeginScrollView(SkillTableRightScrollBar);
                            {
                                //一个Label提示
                                GUILayout.Label("当前技能ID（不可更改，只能通过表格本身修改）: " + skill.ID);
                                //分隔
                                GUILayout.Space(10f);
                                //技能等级
                                GUILayout.Label("当前技能等级(不可更改，只能通过表格本身修改) : " + skill.Level);
                                //分隔
                                GUILayout.Space(10f);
                                //技能对应的图标
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("技能的图标 : ", GUILayout.Width(SkillTableLabelWidth));
                                    GUILayout.BeginVertical();
                                    {
                                        SkillInfoIco = EditorGUILayout.ObjectField(SkillInfoIco, typeof(Sprite), true) as Sprite;
                                        if (SkillInfoIco != null)
                                        {
                                            if (!AssetDatabase.GetAssetPath(SkillInfoIco.GetInstanceID()).StartsWith("Assets/TurnBasedCombat/Resources/"))
                                            {
                                                Color color = GUI.color;
                                                GUI.color = Color.red;
                                                GUILayout.Label("请将图标文件放在Assets/TurnBasedCombat/Resources/ 目录下，否则将不会生效");
                                                GUI.color = color;
                                            }
                                            GUILayout.Label("图标路径:" + AssetDatabase.GetAssetPath(SkillInfoIco.GetInstanceID()).Replace("Assets/TurnBasedCombat/Resources/", "").Split(new string[] { "." }, System.StringSplitOptions.RemoveEmptyEntries)[0]);
                                            GUILayout.Label(SkillInfoIco.texture);
                                            skill.Ico = AssetDatabase.GetAssetPath(SkillInfoIco.GetInstanceID()).Replace("Assets/TurnBasedCombat/Resources/", "").Split(new string[] { "." }, System.StringSplitOptions.RemoveEmptyEntries)[0];
                                        }
                                        else
                                        {
                                            GUILayout.Label("图标路径: ??? ");
                                            skill.Ico = "";
                                        }
                                    }
                                    GUILayout.EndVertical();
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //技能名字
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("技能名称 : ", GUILayout.Width(SkillTableLabelWidth));
                                    skill.Name = EditorGUILayout.TextField(skill.Name);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //技能脚本
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("技能脚本(默认使用BaseSkill) : ", GUILayout.Width(SkillTableLabelWidth));
                                    GUILayout.BeginVertical();
                                    {
                                        SkillInfoMono = EditorGUILayout.ObjectField(SkillInfoMono, typeof(TextAsset), true) as TextAsset;
                                        if (SkillInfoMono != null)
                                        {
                                            if (!AssetDatabase.GetAssetPath(SkillInfoMono.GetInstanceID()).StartsWith("Assets/TurnBasedCombat/Skills/"))
                                            {
                                                Color color = GUI.color;
                                                GUI.color = Color.red;
                                                GUILayout.Label("请将文件放在Assets/TurnBasedCombat/Skills/ 目录下，否则将可能出现问题");
                                                GUI.color = color;
                                            }
                                            skill.SkillMono = SkillInfoMono.name;
                                        }
                                        else
                                        {
                                            skill.SkillMono = "BaseSkill";
                                        }
                                        if (GUILayout.Button("选择技能脚本"))
                                        {
                                            string path = EditorUtility.OpenFilePanel("Choose Skill Mono", "Assets/TurnBasedCombat/Skills/", "cs");
                                            if (!string.IsNullOrEmpty(path))
                                            {
                                                SkillInfoMono = AssetDatabase.LoadAssetAtPath("Assets/TurnBasedCombat/Skills/" + path.Split(new string[] { "Assets/TurnBasedCombat/Skills/" }, System.StringSplitOptions.RemoveEmptyEntries)[1], typeof(TextAsset)) as TextAsset;
                                            }
                                        }
                                        if (SkillInfoMono != null)
                                        {
                                            int start = SkillInfoMono.text.IndexOf("[SkillDescription-Start]");
                                            int end = SkillInfoMono.text.IndexOf("[SkillDescription-End]");
                                            if (start == -1 || end == -1)
                                            {
                                                GUILayout.Label("此技能没有技能内容描述\n可以在技能脚本中使用注释的方法添加\n在注释中使用[SkillDescription-Start][SkillDescription-End]标记包裹\n具体参考BaseSkill脚本");
                                            }
                                            else
                                            {
                                                Color color = GUI.color;
                                                GUI.color = Color.green;
                                                char[] str = new char[end - start];
                                                SkillInfoMono.text.CopyTo(start, str, 0, end - start);
                                                GUILayout.Label("技能内容:" + new string(str).Replace("[SkillDescription-Start]", "").Replace("[SkillDescription-End]", ""));
                                                GUI.color = color;
                                            }
                                        }
                                    }
                                    GUILayout.EndVertical();
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //技能类型
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("技能类型 : ", GUILayout.Width(SkillTableLabelWidth));
                                    skill.SkillType = (Global.SkillType)EditorGUILayout.EnumPopup(skill.SkillType);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //技能特效
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("技能特效路径 : ", GUILayout.Width(SkillTableLabelWidth));
                                    GUILayout.BeginVertical();
                                    {
                                        SkillEffect = EditorGUILayout.ObjectField(SkillEffect, typeof(Object), false);
                                        if (GUILayout.Button("添加新的特效"))
                                        {
                                            if (SkillEffect == null)
                                            {
                                                EditorUtility.DisplayDialog("错误提示", "没有指定特效对象，不能使用添加功能", "确定");
                                            }
                                            else
                                            {
                                                if (AssetDatabase.GetAssetPath(SkillEffect.GetInstanceID()).StartsWith("Assets/TurnBasedCombat/Resources/"))
                                                {
                                                    skill.SkillEffectPaths.Add(AssetDatabase.GetAssetPath(SkillEffect.GetInstanceID()).Replace("Assets/TurnBasedCombat/Resources/", "").Split('.')[0]);
                                                }
                                                else
                                                {
                                                    EditorUtility.DisplayDialog("错误提示", "请将特效对象放在Assets/TurnBasedCombat/Resources/ 后再尝试使用添加功能", "确定");
                                                }
                                            }
                                        }
                                        for (int i = 0; i < skill.SkillEffectPaths.Count; i++)
                                        {
                                            GUILayout.BeginHorizontal();
                                            {
                                                if (GUILayout.Button("高亮目标"))
                                                {
                                                    Selection.activeObject = AssetDatabase.LoadAssetAtPath("Assets/TurnBasedCombat/Resources/" + skill.SkillEffectPaths[i] + ".prefab", typeof(Object));
                                                }
                                                if (GUILayout.Button("向上移动"))
                                                {
                                                    string str = skill.SkillEffectPaths[i];
                                                    if (i - 1 < 0)
                                                    {
                                                        skill.SkillEffectPaths[i] = skill.SkillEffectPaths[skill.SkillEffectPaths.Count - 1];
                                                        skill.SkillEffectPaths[skill.SkillEffectPaths.Count - 1] = str;
                                                    }
                                                    else
                                                    {
                                                        skill.SkillEffectPaths[i] = skill.SkillEffectPaths[i - 1];
                                                        skill.SkillEffectPaths[i - 1] = str;
                                                    }
                                                }
                                                if (GUILayout.Button("删除特效"))
                                                {
                                                    skill.SkillEffectPaths.RemoveAt(i);
                                                    return;
                                                }
                                                GUILayout.Label(skill.SkillEffectPaths[i]);
                                            }
                                            GUILayout.EndHorizontal();
                                        }
                                    }
                                    GUILayout.EndVertical();
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //技能结束时候延时时间
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("技能结束时候延时时间 : ", GUILayout.Width(SkillTableLabelWidth));
                                    skill.SkillEndDelay = EditorGUILayout.FloatField(skill.SkillEndDelay);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //是否是被动技能
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("是否是被动技能 : ", GUILayout.Width(SkillTableLabelWidth));
                                    skill.IsPassiveSkill = EditorGUILayout.Toggle(new GUIContent("被动技能"), skill.IsPassiveSkill);
                                }
                                GUILayout.EndHorizontal();
                                //如果是被动技能，则选择激活类型
                                if (skill.IsPassiveSkill)
                                {
                                    //分隔
                                    GUILayout.Space(10f);
                                    //技能激活状态
                                    GUILayout.BeginHorizontal();
                                    {
                                        GUILayout.Label("被动技能激活时机 : ", GUILayout.Width(SkillTableLabelWidth));
                                        skill.ActiveState = (Global.BuffActiveState)EditorGUILayout.EnumPopup(skill.ActiveState);
                                    }
                                    GUILayout.EndHorizontal();
                                }
                                //分隔
                                GUILayout.Space(10f);
                                //技能目标
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("技能目标 : ", GUILayout.Width(SkillTableLabelWidth));
                                    skill.TargetType = (Global.SkillTargetType)EditorGUILayout.EnumPopup(skill.TargetType);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //技能蓄力回合
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("技能蓄力回合数 : ", GUILayout.Width(SkillTableLabelWidth));
                                    skill.DelayTurn = EditorGUILayout.IntField(skill.DelayTurn);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //技能消耗
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("释放技能消耗的属性 : ", GUILayout.Width(SkillTableLabelWidth));
                                    SkillTableCostPropertyFold = HeroPropertyGUI("正数表示增长，负数表示消耗", skill.CostHeroProperty, SkillTableCostPropertyFold);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //技能造成
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("技能造成的属性变化 : ", GUILayout.Width(SkillTableLabelWidth));
                                    SkillTableCausePropertyFold = HeroPropertyGUI("正数表示增长，负数表示减少", skill.CauseHeroProperty, SkillTableCausePropertyFold);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //技能造成
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("技能伤害 : ", GUILayout.Width(SkillTableLabelWidth));
                                    SkillTableHurtPropertyFold = SkillHurtPropertyGUI("正数表示伤害，负数表示增长", skill.HurtHeroProperty, SkillTableHurtPropertyFold);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //技能暴击率
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("技能暴击率(单位为百分比) : ", GUILayout.Width(SkillTableLabelWidth));
                                    skill.CriticalChance = EditorGUILayout.IntSlider(skill.CriticalChance,0,100);
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //技能附带的buff/debuff
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("技能Buff/Debuff : ", GUILayout.Width(SkillTableLabelWidth));
                                    GUILayout.BeginVertical();
                                    {
                                        if (GUILayout.Button("Buff/Debuff编辑器"))
                                        {
                                            BuffChooseWindow.Init(skill);
                                        }
                                        GetSkillBuffInSkillTable(skill.ID, skill.Level);
                                    }
                                    GUILayout.EndVertical();
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //技能描述
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("技能描述 : ", GUILayout.Width(SkillTableLabelWidth));
                                    skill.Description = EditorGUILayout.TextArea(skill.Description, GUILayout.MinHeight(SkillTableLabelWidth));
                                }
                                GUILayout.EndHorizontal();
                                //分隔
                                GUILayout.Space(10f);
                                //添加保存和删除的按钮
                                GUILayout.BeginHorizontal();
                                {
                                    if (GUILayout.Button("保存技能", GUILayout.Width(SkillTableLabelWidth)))
                                    {
                                        //保存数据
                                        SkillTable.Instance.list[skill.ID][skill.Level] = skill;
                                        //还有一步保存到表格中去
                                        SaveSkilTable();
                                    }
                                    if (GUILayout.Button("删除技能", GUILayout.Width(SkillTableLabelWidth)))
                                    {
                                        //删除数据
                                        SkillTable.Instance.list[skill.ID].Remove(skill.Level);
                                        if (SkillTable.Instance.list[skill.ID].Count == 0)
                                        {
                                            SkillTable.Instance.list.Remove(skill.ID);
                                        }
                                        //重新获取名字列表
                                        string[] skills_names = GetNamesInSkillTable();
                                        if (skills_names != null && skills_names.Length > 0)
                                        {
                                            //重新找一个
                                            SkillTableLeftSelectSkillID = skills_names[0].Split('|')[0].Trim();
                                            SkillTableLeftSelectSkillLevel = int.Parse(skills_names[0].Split('|')[1].Trim());
                                        }
                                        //还有一步保存到表格中去
                                        SaveSkilTable();
                                    }
                                }
                                GUILayout.EndHorizontal();
                            }
                            GUILayout.EndScrollView();
                        }
                    }
                }
                GUILayout.EndVertical();
                #endregion
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// buff/debuff
        /// </summary>
        void BuffTableGUI()
        {
            GUILayout.BeginHorizontal();
            {
                //左侧的buff列表
                GUILayout.BeginVertical(GUILayout.MinWidth(100f), GUILayout.MaxWidth(300f));
                {
                    string[] buff_names = GetNamesInBuffTable();
                    //buff列表
                    BuffTableLeftScrollBar = GUILayout.BeginScrollView(BuffTableLeftScrollBar);
                    {
                        if (buff_names != null && buff_names.Length > 0)
                        {
                            BuffTableLeftSelectIndex = GUILayout.SelectionGrid(BuffTableLeftSelectIndex, buff_names, 1);
                            //强制不会访问越界
                            if (BuffTableLeftSelectIndex >= buff_names.Length)
                            {
                                BuffTableLeftSelectIndex = buff_names.Length - 1;
                            }
                            //保存当前选中的对象
                            BuffTableLeftSelectBuff = buff_names[BuffTableLeftSelectIndex].Split('|')[0].Trim();
                        }
                    }
                    GUILayout.EndScrollView();
                    //分隔
                    GUILayout.Space(10f);
                    //创建ID
                    GUILayout.BeginHorizontal();
                    {
                        BuffTableCreateIDText = EditorGUILayout.TextField(new GUIContent("Buff/Debuff ID"), BuffTableCreateIDText);
                    }
                    GUILayout.EndHorizontal();
                    //创建按钮
                    if (GUILayout.Button("创建新的Buff/Debuff"))
                    {
                        if (BuffTable.Instance.list.ContainsKey(BuffTableCreateIDText))
                        {
                            if (EditorUtility.DisplayDialog("提示", BuffTableCreateIDText + " 已经存在，不能同时存在多个相同ID的Buff或者Debuff", "确定"))
                            {
                                BuffTableCreateIDText = "";
                            }
                        }
                        else
                        {
                            //创建新的buff数据,并添加进入数据列表中
                            Buff buff = new Buff(BuffTableCreateIDText);
                            BuffTable.Instance.list.Add(buff.ID, buff);
                            //设置按钮选中状态
                            buff_names = GetNamesInBuffTable();
                            if (buff_names != null && buff_names.Length > 0)
                            {
                                BuffTableLeftSelectIndex = buff_names.Length - 1;
                            }
                            //将选中新的英雄
                            BuffTableLeftSelectBuff = buff.ID;
                            //清除ID数据
                            BuffTableCreateIDText = "";
                        }
                    }
                }
                GUILayout.EndVertical();

                //右侧详细信息显示
                GUILayout.BeginVertical();
                {
                    Buff h = BuffTable.Instance.GetBuffByID(BuffTableLeftSelectBuff);
                    if (h == null)
                    {
                        GUILayout.Label("当前选中ID" + BuffTableLeftSelectBuff + " 的Buff/Debuff不存在或者获取失败!");
                    }
                    else
                    {
                        #region 右侧Buff详细信息
                        BuffTableRightScrollBar = GUILayout.BeginScrollView(BuffTableRightScrollBar);
                        {
                            //一个Label提示
                            GUILayout.Label("当前状态ID（不可更改，只能通过表格本身修改）: " + h.ID);
                            //分隔
                            GUILayout.Space(10f);
                            //Buff对应的图标
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label("Buff/Debuff的图标 : ", GUILayout.Width(BuffTableLabelWidth));
                                Texture tex = Resources.Load<Texture>("Buff/" + h.ID);
                                if (tex == null)
                                {
                                    GUILayout.Label("没有加载到当前状态的图标");
                                }
                                else
                                {
                                    GUILayout.Label(tex);
                                }
                            }
                            GUILayout.EndHorizontal();
                            //分隔
                            GUILayout.Space(10f);
                            //Buff名字
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label("Buff/Debuff的名字 : ", GUILayout.Width(BuffTableLabelWidth));
                                h.Name = EditorGUILayout.TextField(h.Name);
                            }
                            GUILayout.EndHorizontal();
                            //分隔
                            GUILayout.Space(10f);
                            //类型
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label("Buff/Debuff类型 : ", GUILayout.Width(BuffTableLabelWidth));
                                h.BuffType = (Global.BuffType)EditorGUILayout.EnumPopup(h.BuffType);
                            }
                            GUILayout.EndHorizontal();
                            //分隔
                            GUILayout.Space(10f);
                            //目标类型
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label("Buff/Debuff目标类型 : ", GUILayout.Width(BuffTableLabelWidth));
                                h.TargetType = (Global.BuffTargetType)EditorGUILayout.EnumPopup(h.TargetType);
                            }
                            GUILayout.EndHorizontal();
                            //分隔
                            GUILayout.Space(10f);
                            //目标类型
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label("Buff/Debuff激活阶段 : ", GUILayout.Width(BuffTableLabelWidth));
                                h.BuffActiveState = (Global.BuffActiveState)EditorGUILayout.EnumPopup(h.BuffActiveState);
                            }
                            GUILayout.EndHorizontal();
                            //分隔
                            GUILayout.Space(10f);
                            //是否是增益buff
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label("是否是增益buff : ", GUILayout.Width(BuffTableLabelWidth));
                                h.IsBuff = EditorGUILayout.Toggle(new GUIContent("是否是增益buff"), h.IsBuff);
                            }
                            GUILayout.EndHorizontal();
                            //分隔
                            GUILayout.Space(10f);
                            //buff造成的属性变化
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label("Buff/Debuff造成的属性值变化", GUILayout.Width(BuffTableLabelWidth));
                                BuffTableFoldShow = HeroPropertyGUI("增益状态使用正数，负面状态使用负数", h.ChangeHeroProperty, BuffTableFoldShow);
                            }
                            GUILayout.EndHorizontal();
                            //分隔
                            GUILayout.Space(10f);
                            //技能暴击率
                             GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label("buff成功率(单位为百分比) : ", GUILayout.Width(BuffTableLabelWidth));
                                h.SuccessChance = EditorGUILayout.IntSlider(h.SuccessChance,0,100);
                            }
                             GUILayout.EndHorizontal();
                            //分隔
                            GUILayout.Space(10f);
                            //英雄描述
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label("Buff/Debuff描述 : ", GUILayout.Width(BuffTableLabelWidth));
                                h.Description = EditorGUILayout.TextArea(h.Description, GUILayout.MinHeight(BuffTableLabelWidth));
                            }
                            GUILayout.EndHorizontal();
                            //分隔
                            GUILayout.Space(10f);
                            //添加保存和删除的按钮
                            GUILayout.BeginHorizontal();
                            {
                                if (GUILayout.Button("保存Buff/Debuff", GUILayout.Width(BuffTableLabelWidth)))
                                {
                                    //保存数据
                                    BuffTable.Instance.list[h.ID] = h;
                                    //还有一步保存到表格中去
                                    SaveBuffTable();
                                }
                                if (GUILayout.Button("删除Buff/Debuff", GUILayout.Width(BuffTableLabelWidth)))
                                {
                                    //删除数据
                                    BuffTable.Instance.list.Remove(h.ID);
                                    //重新获取名字列表
                                    string[] buff_names = GetNamesInBuffTable();
                                    if (buff_names != null && buff_names.Length > 0)
                                    {
                                        //重新找一个
                                        BuffTableLeftSelectBuff = buff_names[0].Split('|')[0].Trim();
                                    }
                                    //还有一步保存到表格中去
                                    SaveBuffTable();
                                }
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndScrollView();
                        #endregion
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }


        #region 各个表格的辅助方法
        /// <summary>
        /// 渲染一个技能GUI
        /// </summary>
        void SkillGUI(Skill skill)
        {
            if (skill == null)
                return;
            GUILayout.BeginVertical();
            {
                if (GUILayout.Button("技能ID：" + skill.ID))
                {
                    //跳转进入这个技能列表中
                    //切换到技能面板
                    CurrentSelectIndex = 1;
                    //获取将要显示的技能按钮信息 
                    string[] skills_names = GetNamesInSkillTable();
                    for (int i = 0; i < skills_names.Length; i++)
                    {
                        if (skills_names[i] == skill.ID + " | " + skill.Level + " | " + skill.Name)
                        {
                            SkillTableLeftSelectIndex = i;
                        }
                    }
                }
                GUILayout.Label("技能名字：" + skill.Name);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("技能等级: " + skill.Level);
                }
                GUILayout.EndHorizontal();
                GUILayout.Label("技能描述:" + skill.Description);
            }
            GUILayout.EndVertical();
            GUILayout.Space(10f);
        }

        /// <summary>
        /// 获取英雄列表按钮显示的文字列表
        /// </summary>
        /// <returns></returns>
        string[] GetNamesInHeroTable()
        {
            List<string> names = new List<string>();
            foreach (Hero hero in HeroTable.Instance.list.Values)
            {
                names.Add(hero.ID + " | " + hero.Name);
            }
            return names.ToArray();
        }

        /// <summary>
        /// 渲染一个英雄的当前拥有的技能GUI
        /// </summary>
        /// <param name="ID">英雄ID</param>
        void GetHeroSkillsInHeroTable(string ID)
        {
            Hero h = HeroTable.Instance.GetHeroByID(ID);
            for (int i = 0; i < h.Skills.Count; i++)
            {
                SkillGUI(h.Skills[i]);
            }
        }


        public static void FoucsOn()
        {
            if (window != null)
            {
                window.Focus();
            }
        }

        /// <summary>
        /// 保存英雄表数据
        /// </summary>
        void SaveHeroTable()
        {
            string[] titles = new string[] { "ID", "英雄名字", "英雄真名", "头像路径", "英雄等级", "英雄星级（Global查询）", "生命值", "魔力值", "物理攻击力", "物理防御力", "魔法攻击力", "魔法防御力", "速度", "拥有技能", "英雄描述" };
            string result = "";
            //添加标题
            for (int i = 0; i < titles.Length; i++)
            {
                if (i == titles.Length - 1)
                {
                    result += titles[i] + "\r\n";
                }
                else
                {
                    result += titles[i] + "\t";
                }
            }
            //添加数据
            foreach (Hero hero in HeroTable.Instance.list.Values)
            {
                result += hero.ToString() + "\r\n";
            }
            //写入数据
            System.IO.File.WriteAllText(Global.TablePath + "HeroTable.txt", result);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 获取BuffName列表
        /// </summary>
        /// <returns></returns>
        string[] GetNamesInBuffTable()
        {
            List<string> names = new List<string>();
            foreach (Buff buff in BuffTable.Instance.list.Values)
            {
                names.Add(buff.ID + " | " + buff.Name);
            }
            return names.ToArray();
        }

        /// <summary>
        /// 用于显示一个PropertyGUI
        /// </summary>
        /// <param name="title">折叠标题</param>
        /// <param name="property">传入数据</param>
        /// <param name="show">是否打开折叠</param>
        bool HeroPropertyGUI(string title, HeroProperty property, bool show)
        {
            GUILayout.BeginVertical();
            {
                show = EditorGUILayout.Foldout(show, new GUIContent(title));
                if (show)
                {
                    GUILayout.BeginVertical();
                    {
                        //最大生命值
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("最大生命值");
                            property.MaxLife.Value = EditorGUILayout.LongField(property.MaxLife.Value);
                            property.MaxLife.Unit = (Global.UnitType)EditorGUILayout.EnumPopup(property.MaxLife.Unit);
                        }
                        GUILayout.EndHorizontal();
                        //最大魔力值
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("最大魔力值");
                            property.MaxMagic.Value = EditorGUILayout.LongField(property.MaxMagic.Value);
                            property.MaxMagic.Unit = (Global.UnitType)EditorGUILayout.EnumPopup(property.MaxMagic.Unit);
                        }
                        GUILayout.EndHorizontal();
                        //当前生命值
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("当前生命值");
                            property.CurrentLife.Value = EditorGUILayout.LongField(property.CurrentLife.Value);
                            property.CurrentLife.Unit = (Global.UnitType)EditorGUILayout.EnumPopup(property.CurrentLife.Unit);
                        }
                        GUILayout.EndHorizontal();
                        //当前魔力值
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("当前魔力值");
                            property.CurrentMagic.Value = EditorGUILayout.LongField(property.CurrentMagic.Value);
                            property.CurrentMagic.Unit = (Global.UnitType)EditorGUILayout.EnumPopup(property.CurrentMagic.Unit);
                        }
                        GUILayout.EndHorizontal();
                        //物理攻击力
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("物理攻击力");
                            property.Attack.Value = EditorGUILayout.LongField(property.Attack.Value);
                            property.Attack.Unit = (Global.UnitType)EditorGUILayout.EnumPopup(property.Attack.Unit);
                        }
                        GUILayout.EndHorizontal();
                        //物理防御力
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("物理防御力");
                            property.Defense.Value = EditorGUILayout.LongField(property.Defense.Value);
                            property.Defense.Unit = (Global.UnitType)EditorGUILayout.EnumPopup(property.Defense.Unit);
                        }
                        GUILayout.EndHorizontal();
                        //魔法攻击力
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("魔法攻击力");
                            property.MagicAttack.Value = EditorGUILayout.LongField(property.MagicAttack.Value);
                            property.MagicAttack.Unit = (Global.UnitType)EditorGUILayout.EnumPopup(property.MagicAttack.Unit);
                        }
                        GUILayout.EndHorizontal();
                        //魔法防御力
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("魔法防御力");
                            property.MagicDefense.Value = EditorGUILayout.LongField(property.MagicDefense.Value);
                            property.MagicDefense.Unit = (Global.UnitType)EditorGUILayout.EnumPopup(property.MagicDefense.Unit);
                        }
                        GUILayout.EndHorizontal();
                        //速度
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("速度");
                            property.Speed.Value = EditorGUILayout.LongField(property.Speed.Value);
                            property.Speed.Unit = (Global.UnitType)EditorGUILayout.EnumPopup(property.Speed.Unit);
                        }
                        GUILayout.EndHorizontal();
                        //回合数
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("回合数");
                            property.Turn = EditorGUILayout.IntField(property.Turn);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();
            return show;
        }

        bool SkillHurtPropertyGUI(string title, HeroProperty property, bool show)
        {
            GUILayout.BeginVertical();
            {
                show = EditorGUILayout.Foldout(show, new GUIContent(title));
                if (show)
                {
                    GUILayout.BeginVertical();
                    {
                        //物理攻击力
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("物理伤害");
                            property.Attack.Value = EditorGUILayout.LongField(property.Attack.Value);
                            property.Attack.Unit = (Global.UnitType)EditorGUILayout.EnumPopup(property.Attack.Unit);
                        }
                        GUILayout.EndHorizontal();
                        //魔法攻击力
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("魔法伤害");
                            property.MagicAttack.Value = EditorGUILayout.LongField(property.MagicAttack.Value);
                            property.MagicAttack.Unit = (Global.UnitType)EditorGUILayout.EnumPopup(property.MagicAttack.Unit);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();
            return show;
        }

        /// <summary>
        /// 保存Buff表数据
        /// </summary>
        void SaveBuffTable()
        {
            string[] titles = new string[] { "ID", "buff名字", "buff类型（Global查询）", "目标类型（Global查询）", "buff激活的阶段", "buff造成的属性变化", "是否是增益buff","成功率", "buff描述" };
            string result = "";
            //添加标题
            for (int i = 0; i < titles.Length; i++)
            {
                if (i == titles.Length - 1)
                {
                    result += titles[i] + "\r\n";
                }
                else
                {
                    result += titles[i] + "\t";
                }
            }
            //添加数据
            foreach (Buff buff in BuffTable.Instance.list.Values)
            {
                result += buff.ID + "\t";
                result += buff.Name + "\t";
                result += (int)buff.BuffType + "\t";
                result += (int)buff.TargetType + "\t";
                result += (int)buff.BuffActiveState + "\t";
                result += buff.ChangeHeroProperty.ToString() + "\t";
                result += (buff.IsBuff ? "1" : "0") + "\t";
                result += (int)buff.SuccessChance + "\t";
                result += buff.Description + "\r\n";
            }
            //写入数据
            System.IO.File.WriteAllText(Global.TablePath + "BuffTable.txt", result);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 获取技能按钮列表对象
        /// </summary>
        /// <returns></returns>
        string[] GetNamesInSkillTable()
        {
            List<string> result = new List<string>();
            foreach (string skill_id in SkillTable.Instance.list.Keys)
            {
                Dictionary<int, Skill> Skills = SkillTable.Instance.GetSkillsByID(skill_id);
                foreach (Skill skill in Skills.Values)
                {
                    result.Add(skill.ID + " | " + skill.Level + " | " + skill.Name);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// 保存技能表格
        /// </summary>
        void SaveSkilTable()
        {
            string[] titles = new string[] { "ID", "技能名字", "技能图标", "技能类型（Global查询）", "技能控制脚本", "是否是被动技能", "技能特效路径", "如果是被动技能的激活状态", "技能目标", "技能等级", "技能消耗使用者的属性值", "技能蓄力回合数", "技能对于除了伤害的属性值变化", "技能造成的攻击伤害", "技能暴击几率", "技能的buff或者debuff", "技能描述" };
            string result = "";
            //添加标题
            for (int i = 0; i < titles.Length; i++)
            {
                if (i == titles.Length - 1)
                {
                    result += titles[i] + "\r\n";
                }
                else
                {
                    result += titles[i] + "\t";
                }
            }
            //添加数据
            foreach (string skill_id in SkillTable.Instance.list.Keys)
            {
                Dictionary<int, Skill> Skills = SkillTable.Instance.GetSkillsByID(skill_id);
                foreach (Skill skill in Skills.Values)
                {
                    result += skill.ToString() + "\r\n";
                }
            }
            //写入数据
            System.IO.File.WriteAllText(Global.TablePath + "SkillTable.txt", result);
            AssetDatabase.SaveAssets();
        }


        void GetSkillBuffInSkillTable(string id, int level)
        {
            Skill skill = SkillTable.Instance.GetSkillByIDAndLevel(id, level);
            for (int i = 0; i < skill.SkillBuff.Count; i++)
            {
                SkillBuffGUI(skill.SkillBuff[i]);
            }
        }


        void SkillBuffGUI(Buff buff)
        {
            if (buff == null)
                return;
            GUILayout.BeginVertical();
            {
                if (GUILayout.Button("Buff/Debuff ID：" + buff.ID))
                {
                    //跳转进入这个技能列表中
                    //切换到技能面板
                    CurrentSelectIndex = 2;
                    //获取将要显示的技能按钮信息 
                    string[] buffs = GetNamesInBuffTable();
                    for (int i = 0; i < buffs.Length; i++)
                    {
                        if (buffs[i] == buff.ID + " | " + buff.Name)
                        {
                            BuffTableLeftSelectIndex = i;
                        }
                    }
                }
                GUILayout.Label("Buff/Debuff名字：" + buff.Name + (buff.IsBuff ? "(增益buff)" : "(负面Debuff)"));
                GUILayout.Label("Buff/Debuff成功概率:" + buff.SuccessChance + "%");
                GUILayout.Label("Buff/Debuff描述:" + buff.Description);
            }
            GUILayout.EndVertical();
            GUILayout.Space(10f);
        }
        #endregion
    }
}