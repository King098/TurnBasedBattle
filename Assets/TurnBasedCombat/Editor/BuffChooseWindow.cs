using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace King.TurnBasedCombat
{
    /// <summary>
    /// Buff/Debuff选择窗口
    /// </summary>
    public class BuffChooseWindow : EditorWindow
    {
        private static Skill _Skill;

        private static bool[] SkillBuffFold;

        private static Dictionary<string, bool> AllBuffFold;

        private static BuffChooseWindow window;

        private Vector2 skillscrollbar;

        private Vector2 scrollbar;

        public static void Init(Skill skill)
        {
            _Skill = skill;
            window = EditorWindow.GetWindow<BuffChooseWindow>("Buff/Debuff编辑器");
            ResetSkillBuffFold(_Skill.SkillBuff.Count);
            AllBuffFold = new Dictionary<string, bool>();
            foreach (string id in BuffTable.Instance.list.Keys)
            {
                AllBuffFold.Add(id, false);
            }
        }


        void OnDestroy()
        {
            _Skill = null;
            SkillBuffFold = null;
            TurnBasedCombatEditor.FoucsOn();
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
                #region 当前技能显示
                SkillGUI();
                #endregion
                #region 当前技能已经有的Buff/Debuff
                SkillBuffGUI();
                #endregion
                #region 所有的Buff/Debuff
                AllBuff();
                #endregion
            }
            GUILayout.EndVertical();
        }

        void SkillGUI()
        {
            GUILayout.Label("当前技能ID : " + _Skill.ID);
            GUILayout.Label("当前技能等级 : " + _Skill.Level);
            GUILayout.Label("当前技能名称 : " + _Skill.Name);
            GUILayout.Label("当前技能描述 : " + _Skill.Description);
        }

        void SkillBuffGUI()
        {
            GUILayout.Space(10f);
            GUILayout.Label("当前技能拥有的Buff/Debuff : ");
            skillscrollbar = GUILayout.BeginScrollView(skillscrollbar);
            {
                for (int i = 0; i < _Skill.SkillBuff.Count; i++)
                {
                    BuffGUI(_Skill.SkillBuff[i]);
                }
            }
            GUILayout.EndScrollView();
        }


        void AllBuff()
        {
            GUILayout.Space(10f);
            GUILayout.Label("当前所有可以选择的Buff/Debuff : ");
            scrollbar = GUILayout.BeginScrollView(scrollbar);
            {
                foreach (string buff in BuffTable.Instance.list.Keys)
                {
                    Buff(buff);
                }
            }
            GUILayout.EndScrollView();
        }


        static void ResetSkillBuffFold(int count)
        {
            SkillBuffFold = new bool[count];
            for (int i = 0; i < SkillBuffFold.Length; i++)
            {
                SkillBuffFold[i] = false;
            }
        }

        void BuffGUI(Buff buff)
        {
            if (buff == null)
                return;
            GUILayout.BeginHorizontal();
            {
                //删除按钮
                if (GUILayout.Button("X", GUILayout.Width(30f)))
                {
                    _Skill.SkillBuff.Remove(buff);
                    //重置技能显示
                    ResetSkillBuffFold(_Skill.SkillBuff.Count);
                    return;
                }
                //展开技能信息
                SkillBuffFold[_Skill.SkillBuff.IndexOf(buff)] = EditorGUILayout.Foldout(SkillBuffFold[_Skill.SkillBuff.IndexOf(buff)], new GUIContent(buff.Name));
                if (SkillBuffFold[_Skill.SkillBuff.IndexOf(buff)])
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label("当前Buff/Debuff ID : " + buff.ID);
                        GUILayout.Label("当前Buff/Debuff名字 : " + buff.Name);
                        GUILayout.BeginHorizontal();
                        {
                            buff.SuccessChance = EditorGUILayout.IntField(new GUIContent("Buff/Debuff成功概率 : "), buff.SuccessChance);
                            GUILayout.Label("%");
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Label("是否是Buff : " + buff.IsBuff);
                        GUILayout.Label(buff.Description);
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndHorizontal();
        }


        bool HasBuffInSkill(Buff buff)
        {
            for (int i = 0;i< _Skill.SkillBuff.Count; i++)
            {
                if (_Skill.SkillBuff[i].ID == buff.ID)
                {
                    return true;
                }
            }
            return false;
        }

        void Buff(string buff_id)
        {
            Buff buff = BuffTable.Instance.GetBuffByID(buff_id);
            if (buff == null)
                return;
            GUILayout.BeginHorizontal();
            {
                //左侧分隔
                GUILayout.Space(10f);
                //添加按钮
                if (GUILayout.Button("添加此Buff", GUILayout.Width(100f)))
                {
                    if (HasBuffInSkill(buff))
                    {
                        EditorUtility.DisplayDialog("提示", "当前技能已经拥有这个Buff " + buff.Name, "确定");
                    }
                    else
                    {
                        _Skill.SkillBuff.Add(buff);
                        //添加导数据列表中
                        SkillTable.Instance.list[_Skill.ID][_Skill.Level] = _Skill;
                        //刷新Fold
                        ResetSkillBuffFold(_Skill.SkillBuff.Count);
                        return;
                    }
                }
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(10f);
                    //然后是折叠列表
                    AllBuffFold[buff_id] = EditorGUILayout.Foldout(AllBuffFold[buff_id], new GUIContent(buff.ID + " | " + buff.Name));
                    if (AllBuffFold[buff_id])
                    {
                        GUILayout.BeginVertical();
                        {
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Space(20f);
                                GUILayout.Label("Buff/Debuff ID:" + buff.ID);
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Space(20f);
                                GUILayout.Label("Buff/Debuff 名称:" + buff.Name);
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Space(20f);
                                GUILayout.Label("是否是增益Buff :" + buff.IsBuff);
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Space(20f);
                                GUILayout.Label("Buff/Debuff 描述:" + buff.Description);
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
    }
}
