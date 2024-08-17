using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using King.TurnBasedCombat;

public class TestCombat : MonoBehaviour 
{
    public bool UseTableData;
    public List<string> PlayerIds;
    public List<string> EnemyIds;
    public List<Hero> Players;
    public List<Hero> Firendly;
    public List<Hero> Enemys1;
    public List<Hero> Enemys2;   

    void Start()
    {
        if(UseTableData)
        {
            TableController.Instance.LoadAllTable();
            //开始加载数据
            List<Hero> players = new List<Hero>();
            List<Hero> enemy = new List<Hero>();
            for(int i = 0;i<PlayerIds.Count;i++)
            {
                Hero hero = HeroTable.Instance.GetHeroByID(PlayerIds[i]);
                if(hero != null)
                {
                    players.Add(hero);
                }
            }
            for(int i = 0;i<EnemyIds.Count;i++)
            {
                Hero hero = HeroTable.Instance.GetHeroByID(EnemyIds[i]);
                if(hero != null)
                {
                    enemy.Add(hero);
                }
            }
            BattleController.Instance.StartBattle(players, enemy);
        }
        else
        {
            List<HeroTeam> teams = new List<HeroTeam>(){
                new HeroTeam(Players,HeroTeamType.Mine,0,HeroTeam.MineTeamGroup),
                new HeroTeam(Enemys1,HeroTeamType.NPC,1,"Enemy1"),
                new HeroTeam(Enemys2,HeroTeamType.NPC,2,"Enemy2"),
                new HeroTeam(Firendly,HeroTeamType.NPC,3,HeroTeam.MineTeamGroup)
            };
            BattleController.Instance.StartBattle(teams);
        }
    }
}
