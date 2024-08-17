using System.Collections.Generic;

namespace King.TurnBasedCombat
{
    /// <summary>
    /// 英雄阵容的分类
    /// </summary>
    public enum HeroTeamType
    {
        /// <summary>
        /// 自己的阵容
        /// </summary>
        Mine,
        /// <summary>
        /// 其他玩家的这阵容
        /// </summary>
        Player,
        /// <summary>
        /// NPC的阵容
        /// </summary>
        NPC
    }

    /// <summary>
    /// 阵营组分类
    /// </summary>
    public enum TeamGroupType
    {
        /// <summary>
        /// 相同阵营
        /// </summary>
        SameTeamGroup,
        /// <summary>
        /// 不同阵营
        /// </summary>
        NotSameTeamGroup,
        /// <summary>
        /// 所有阵营
        /// </summary>
        AllTeamGroup,
    }

    /// <summary>
    /// 英雄阵容信息
    /// </summary>
    public class HeroTeamMono
    {
        /// <summary>
        /// 阵营类型
        /// </summary>
        public HeroTeamType TeamType = HeroTeamType.Mine;
        /// <summary>
        /// 阵营组
        /// </summary>
        public string TeamGroup = "";
        /// <summary>
        /// 阵营所在位置索引
        /// </summary>
        public int TeamIndex = 0;
        /// <summary>
        /// 一个队伍中的所有英雄Mono
        /// </summary>
        public List<HeroMono> Heros = new List<HeroMono>();
        /// <summary>
        /// 当前可以操控的其他阵容里面的英雄对象
        /// </summary>
        public List<HeroMono> OtherHeros = new List<HeroMono>();

        public HeroTeamMono(HeroTeam team,List<HeroMono> heros)
        {
            this.TeamIndex = team.TeamIndex;
            this.TeamType = team.TeamType;
            this.TeamGroup = team.TeamGroup;
            this.Heros.Clear();
            this.Heros.AddRange(heros);
            this.OtherHeros.Clear();
        }

        /// <summary>
        /// 传入的英雄数据是否是当前阵营的所属英雄
        /// </summary>
        /// <param name="hero"></param>
        /// <returns></returns>
        public bool IsTeamHero(HeroMono hero)
        {
            return Heros.Contains(hero);
        }

        /// <summary>
        /// 是否被当前的阵容操控的英雄
        /// </summary>
        /// <param name="hero"></param>
        /// <returns></returns>
        public bool IsControledHero(HeroMono hero)
        {
            if(IsTeamHero(hero) && (hero.ControledTeam == null || hero.ControledTeam == this))
            {
                return true;
            }
            return OtherHeros.Contains(hero);
        }

        /// <summary>
        /// 操控一个其他指定英雄
        /// </summary>
        /// <param name="hero"></param>
        public void ControlHero(HeroMono hero)
        {
            if(IsControledHero(hero))
            {
                return;
            }
            hero.ControlledByHeroTeam(this);
            OtherHeros.Add(hero);
        }

        /// <summary>
        /// 释放一个其他英雄操作空
        /// </summary>
        /// <param name="hero"></param>
        public void UncontrolHero(HeroMono hero)
        {
            if(OtherHeros.Contains(hero))
            {
                hero.ControledTeam = hero.OrignalTeam;
                OtherHeros.Remove(hero);
            }
        }

        /// <summary>
        /// 获取当前控制的活着的英雄(一个参数是否包含其他阵营中被操控的对象,默认是包含的)
        /// </summary>
        /// <returns></returns>
        public List<HeroMono> GetTeamAlive(bool containOther = true)
        {
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < Heros.Count; i++)
            {
                if (!Heros[i].IsDead())
                {
                    result.Add(Heros[i]);
                }
            }
            if(containOther)
            {
                for(int i = 0; i < OtherHeros.Count; i++)
                {
                    if (!OtherHeros[i].IsDead())
                    {
                        result.Add(OtherHeros[i]);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取阵营中所有死亡的英雄
        /// </summary>
        /// <param name="containOther">是否包含其他阵营被控制的英雄，默认包含</param>
        /// <returns></returns>
        public List<HeroMono> GetTeamDead(bool containOther = true)
        {
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < Heros.Count; i++)
            {
                if (Heros[i].IsDead())
                {
                    result.Add(Heros[i]);
                }
            }
            if(containOther)
            {
                for(int i = 0; i < OtherHeros.Count; i++)
                {
                    if (OtherHeros[i].IsDead())
                    {
                        result.Add(OtherHeros[i]);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取阵营中不论生死的英雄
        /// </summary>
        /// <param name="containOther">是否包含其他阵营被控制的英雄，默认包含</param>
        /// <returns></returns>
        public List<HeroMono> GetTeamHero(bool containOther = true)
        {
            List<HeroMono> result = new List<HeroMono>();
            for (int i = 0; i < Heros.Count; i++)
            {
                result.Add(Heros[i]);
            }
            if(containOther)
            {
                for(int i = 0; i < OtherHeros.Count; i++)
                {
                    result.Add(OtherHeros[i]);
                }
            }
            return result;
        } 

        /// <summary>
        /// 判断这个阵营是否已经失败了
        /// </summary>
        /// <returns></returns>
        public bool CheckIsFailed()
        {
            for (int i = 0; i < Heros.Count; i++)
            {
                if (!Heros[i].IsDead() || !Heros[i].IsControlledBySelf())
                    return false;
            }
            return true;
        }
    }
}