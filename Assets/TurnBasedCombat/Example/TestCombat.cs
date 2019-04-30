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
    public List<Hero> Enemys;

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
            BattleController.Instance.StartBattle(Players, Enemys);
        }
    }
}
