using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using King.TurnBasedCombat;

public class TestCombat : MonoBehaviour 
{
    public List<Hero> Players;
    public List<Hero> Enemys;

    void Start()
    {
        BattleController.Instance.StartBattle(Players, Enemys);
    }
}
