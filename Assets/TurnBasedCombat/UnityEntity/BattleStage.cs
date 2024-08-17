using System.Collections.Generic;
using System.Threading.Tasks;
using AUIFramework;
using King.Tools;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace King.TurnBasedCombat
{
    public class BattleStage : MonoBehaviour
    {
        public SpriteRenderer BackgroundSpriteRenderer;
        public List<Transform> Team1Slot;
        public List<Transform> Team2Slot;
        public List<Transform> Team3Slot;
        public List<Transform> Team4Slot;
        public GameObject DefaultPrefab;

        private AsyncOperationHandle[,] heroHandles = new AsyncOperationHandle[4,6]
        {
            {default, default, default, default, default, default},
            {default,default,default,default,default,default},
            {default,default,default,default,default,default},
            {default,default,default,default,default,default}
        };
        

        public void SetBackground(Sprite background)
        {
            BackgroundSpriteRenderer.sprite = background;
        }

        public Transform GetHeroRoot(int teamSlot, int posIndex)
        {
            switch (teamSlot)
            {
                case 0:
                    return Team1Slot[posIndex];
                case 1:
                    return Team2Slot[posIndex];
                case 2:
                    return Team3Slot[posIndex];
                case 3:
                    return Team4Slot[posIndex];
                default:
                    return null;
            }
        }

        /// <summary>
        /// 加载一个阵容的英雄资源，并返回生成的HeroMono实力列表
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public async Task<HeroTeamMono> LoadHeroes(HeroTeam team)
        {
            List<HeroMono> heroes = new List<HeroMono>();
            HeroTeamMono heroTeamMono = new HeroTeamMono(team, heroes);
            for (int j = 0; j < team.Heroes.Count; j++)
            {
                Hero hero = team.Heroes[j];
                HeroMono hero_temp = null;
                AsyncOperationHandle handle = await ResourcesManager.LoadAsync<GameObject>("Hero/" + hero.ID);
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    ResourcesManager.ReleaseHandle(heroHandles[team.TeamIndex, j]);
                    heroHandles[team.TeamIndex, j] = handle;
                    GameObject prefab = handle.Result as GameObject;
                    if (UnityStaticTool.CreateObjectReletiveTo<HeroMono>(prefab == null ? DefaultPrefab : prefab, this.transform, out hero_temp))
                    {
                        hero_temp.Init(hero, heroTeamMono);
                        hero_temp.transform.position = GetHeroRoot(team.TeamIndex, j).position;
                        //记录英雄坐标
                        hero_temp.HeroPosition = GetHeroRoot(team.TeamIndex, j).position;
                        heroes.Add(hero_temp);
                    }
                }
                else
                {
                    if (UnityStaticTool.CreateObjectReletiveTo<HeroMono>(DefaultPrefab, this.transform, out hero_temp))
                    {
                        hero_temp.Init(hero, heroTeamMono);
                        hero_temp.transform.position = GetHeroRoot(team.TeamIndex, j).position;
                        //记录英雄坐标
                        hero_temp.HeroPosition = GetHeroRoot(team.TeamIndex, j).position;
                        heroes.Add(hero_temp);
                    }
                }
            }
            //添加阵营的英雄数据
            heroTeamMono.Heros.AddRange(heroes);
            return heroTeamMono;
        }

        public void Clear()
        {
            for(int i = 0;i<Team1Slot.Count;i++)
            {
                UnityStaticTool.DestoryChilds(Team1Slot[i].gameObject);
            }
            for(int i = 0;i<Team2Slot.Count;i++)
            {
                UnityStaticTool.DestoryChilds(Team2Slot[i].gameObject);
            }
            for(int i = 0;i<Team3Slot.Count;i++)
            {
                UnityStaticTool.DestoryChilds(Team3Slot[i].gameObject);
            }
            for(int i = 0;i<Team4Slot.Count;i++)
            {
                UnityStaticTool.DestoryChilds(Team4Slot[i].gameObject);
            }
            for (int i = 0; i < heroHandles.GetLength(0); i++)
            {
                for(int j = 0; j <heroHandles.GetLength(1); j++)
                {
                    ResourcesManager.ReleaseHandle(heroHandles[i,j]);
                }
            }
        }
    }
}