using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace King.TurnBasedCombat
{
    /// <summary>
    /// 技能选择窗口
    /// </summary>
    public class SkillChooseWindow : EditorWindow
    {
        private static bool[] HeroSkillFold;
        /// <summary>
        /// 技能一级展开
        /// </summary>
        private static Dictionary<string, bool> SkillFoldShos;
        /// <summary>
        /// 用于展开技能的bool值
        /// </summary>
        private static Dictionary<string, Dictionary<int, bool>> FoldShows;
        /// <summary>
        /// 当前的英雄
        /// </summary>
        private static Hero _Hero;

        private static SkillChooseWindow window;

        private Vector2 scrollbar;

        private Vector2 SkillScrollBar;

        public static void InitWindow(Hero hero)
        {
            _Hero = hero;
            window = EditorWindow.GetWindow<SkillChooseWindow>("选择技能窗口");
            SkillFoldShos = new Dictionary<string, bool>();
            FoldShows = new Dictionary<string, Dictionary<int, bool>>();
            foreach (string skill_id in SkillTable.Instance.list.Keys)
            {
                Dictionary<int, Skill> ss = SkillTable.Instance.GetSkillsByID(skill_id);
                Dictionary<int, bool> s_bool = new Dictionary<int, bool>();
                foreach (Skill s in ss.Values)
                {
                    s_bool.Add(s.Level, false);
                }
                FoldShows.Add(skill_id, s_bool);
                SkillFoldShos.Add(skill_id, false);
            }
            ResetHeroSkillFold(hero.Skills.Count);
        }

        public static void CloseWindow()
        {
            if (window != null)
            {
                window.Close();
            }
        }

        void OnGUI()
        {
            GUILayout.BeginVertical();
            {
                #region 首先是显示当前英雄
                GUILayout.Label("当前英雄ID : " + _Hero.ID);
                GUILayout.Label("英雄名字 : " + _Hero.Name);
                GUILayout.Label("英雄真名 : " + _Hero.RealName);
                #endregion
                #region 其次是显示当前英雄技能
                GUILayout.Label("当前英雄技能 : ");
                scrollbar = GUILayout.BeginScrollView(scrollbar);
                {
                    for (int i = 0; i < _Hero.Skills.Count; i++)
                    {
                        HeroSkillGUI(_Hero.Skills[i]);
                    }
                }
                GUILayout.EndScrollView();
                #endregion
                #region 接着显示技能筛选
                #endregion
                #region 显示符合条件的技能
                GUILayout.Label("所有可配置的技能");
                SkillScrollBar = GUILayout.BeginScrollView(SkillScrollBar);
                {
                    foreach (string id in SkillTable.Instance.list.Keys)
                    {
                        SkillGUI(id);
                    }
                }
                GUILayout.EndScrollView();
                #endregion
            }
            GUILayout.EndVertical();
        }

        void OnDestroy()
        {
            _Hero = null;
            FoldShows = null;
            TurnBasedCombatEditor.FoucsOn();
        }


        static void ResetHeroSkillFold(int count)
        {
            HeroSkillFold = new bool[count];
            for (int i = 0; i < HeroSkillFold.Length; i++)
            {
                HeroSkillFold[i] = false;
            }
        }

        void HeroSkillGUI(Skill skill)
        {
            if (skill == null)
                return;
            GUILayout.BeginHorizontal();
            {
                //删除按钮
                if (GUILayout.Button("X", GUILayout.Width(30f)))
                {
                    _Hero.Skills.Remove(skill);
                    //重置技能显示
                    ResetHeroSkillFold(_Hero.Skills.Count);
                    return;
                }
                //展开技能信息
                HeroSkillFold[_Hero.Skills.IndexOf(skill)] = EditorGUILayout.Foldout(HeroSkillFold[_Hero.Skills.IndexOf(skill)], new GUIContent(skill.Name));
                if (HeroSkillFold[_Hero.Skills.IndexOf(skill)])
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label("当前技能ID : " + skill.ID);
                        GUILayout.Label("当前技能名字 : " + skill.Name);
                        GUILayout.Label("当前技能等级 : " + skill.Level);
                        GUILayout.Label(skill.Description);
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndHorizontal();
        }

        void SkillGUI(string skill_id)
        {
            Dictionary<int, Skill> Skills = SkillTable.Instance.GetSkillsByID(skill_id);
            if (Skills == null || Skills.Count <= 0)
                return;
            GUILayout.BeginVertical();
            {
                //展开技能信息
                SkillFoldShos[skill_id] = EditorGUILayout.Foldout(SkillFoldShos[skill_id], new GUIContent(skill_id));
                if (SkillFoldShos[skill_id])
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(20f);
                        GUILayout.BeginVertical();
                        {
                            foreach (Skill skill in Skills.Values)
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    //添加技能按钮
                                    if (GUILayout.Button("添加此技能"))
                                    {
                                        if (_Hero.Skills.Contains(skill))
                                        {
                                            EditorUtility.DisplayDialog("提示", "当前英雄已经拥有技能 " + skill.ID + " | " + skill.Level + " | " + skill.Name, "确定");
                                        }
                                        else
                                        {
                                            //添加技能
                                            _Hero.Skills.Add(skill);
                                            //并将当前英雄数据添加到列表中
                                            HeroTable.Instance.list[_Hero.ID] = _Hero;
                                            //刷新fold
                                            ResetHeroSkillFold(_Hero.Skills.Count);
                                            //返回重新刷新
                                            return;
                                        }
                                    }
                                    GUILayout.BeginVertical();
                                    {
                                        //展开二级目录
                                        FoldShows[skill.ID][skill.Level] = EditorGUILayout.Foldout(FoldShows[skill.ID][skill.Level], new GUIContent(skill.Name + "(" + skill.Level + ")"));
                                        if (FoldShows[skill.ID][skill.Level])
                                        {
                                            GUILayout.BeginHorizontal();
                                            {
                                                //分隔
                                                GUILayout.Space(20f);
                                                GUILayout.BeginVertical();
                                                GUILayout.Label("当前技能ID : " + skill.ID);
                                                GUILayout.Label("当前技能名字 : " + skill.Name);
                                                GUILayout.Label("当前技能等级 : " + skill.Level);
                                                GUILayout.Label(skill.Description);
                                                GUILayout.EndVertical();
                                            }
                                            GUILayout.EndHorizontal();
                                        }
                                    }
                                    GUILayout.EndVertical();
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }
    }
}