using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace King.TurnBasedCombat
{
    public class TableController
    {
        private static TableController m_self = null;
        public static TableController Instance
        {
            get
            {
                if (m_self == null)
                {
                    m_self = new TableController();
                }
                return m_self;
            }
        }

        private TableController() { }

        private string path = Application.dataPath + "/TurnBasedCombat/Table/";

        public void LoadAllTable()
        {
            List<string> files = new List<string>();
            files.Add("BuffTable.txt");
            files.Add("SkillTable.txt");
            files.Add("HeroTable.txt");

            for (int i = 0; i < files.Count; i++)
            {
                string content = File.ReadAllText(path + files[i]);
                switch (files[i])
                {
                    case "BuffTable.txt":
                        BuffTable.Instance.LoadTable(content);
                        break;
                    case "SkillTable.txt":
                        SkillTable.Instance.LoadTable(content);
                        break;
                    case "HeroTable.txt":
                        HeroTable.Instance.LoadTable(content);
                        break;
                }
            }
        }
    }

    #region BuffTable
    public class BuffTable
    {
        private static BuffTable m_self = null;
        public static BuffTable Instance
        {
            get
            {
                if (m_self == null)
                {
                    m_self = new BuffTable();
                }
                return m_self;
            }
        }

        private BuffTable() { }

        public Dictionary<string, Buff> list;

        public void LoadTable(string content)
        {
            list = new Dictionary<string, Buff>();
            string[] rows = content.Trim().Replace("\r", "").Split('\n');
            for (int i = 0; i < rows.Length; i++)
            {
                if (i == 0)
                {
                    continue;
                }
                string[] colums = rows[i].Split('\t');
                Buff buff = new Buff(colums);
                list.Add(buff.ID, buff);
            }
        }

        public Buff GetBuffByID(string id)
        {
            if (list.ContainsKey(id))
            {
                return list[id];
            }
            return null;
        }
    }
    #endregion


    #region SkillTable
    public class SkillTable
    {
        private static SkillTable m_self = null;
        public static SkillTable Instance
        {
            get
            {
                if (m_self == null)
                {
                    m_self = new SkillTable();
                }
                return m_self;
            }
        }

        private SkillTable() { }

        public Dictionary<string,Dictionary<int,Skill>> list;

        public void LoadTable(string content)
        {
            list = new Dictionary<string, Dictionary<int,Skill>>();
            string[] rows = content.Trim().Replace("\r", "").Split('\n');
            for (int i = 0; i < rows.Length; i++)
            {
                if (i == 0)
                {
                    continue;
                }
                string[] colums = rows[i].Split('\t');
                Skill skill = new Skill(colums);
                if (list.ContainsKey(skill.ID))
                {
                    list[skill.ID].Add(skill.Level,skill);
                }
                else
                {
                    list.Add(skill.ID, new Dictionary<int,Skill>() { {skill.Level,skill}});
                }
            }
        }

        public Dictionary<int,Skill> GetSkillsByID(string id)
        {
            if (list.ContainsKey(id))
            {
                return list[id];
            }
            return null;
        }

        public Skill GetSkillByIDAndLevel(string id, int level)
        {
            if (list.ContainsKey(id))
            {
                if (list[id].ContainsKey(level))
                {
                    return list[id][level];
                }
            }
            return null;
        }
    }
    #endregion

    #region HeroTable
    public class HeroTable
    {
        private static HeroTable m_self = null;
        public static HeroTable Instance
        {
            get
            {
                if (m_self == null)
                {
                    m_self = new HeroTable();
                }
                return m_self;
            }
        }

        private HeroTable() { }

        public Dictionary<string, Hero> list;

        public void LoadTable(string content)
        {
            list = new Dictionary<string, Hero>();
            string[] rows = content.Trim().Replace("\r", "").Split('\n');
            for (int i = 0; i < rows.Length; i++)
            {
                if (i == 0)
                    continue;
                string[] colums = rows[i].Split('\t');
                Hero hero = new Hero(colums);
                list.Add(hero.ID, hero);
            }
        }

        public Hero GetHeroByID(string id)
        {
            if (list.ContainsKey(id))
            {
                return list[id];
            }
            return null;
        }
    }
    #endregion
}