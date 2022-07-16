using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : MonoBehaviour
{

    MapManager mapManager;

    // Start is called before the first frame update
    void Start()
    {
        mapManager = FindObjectOfType<MapManager>();
    }

    public void NextMove()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in enemies)
        {
            var enemyController = enemy.GetComponent<EnemyBase>();
            enemyController.TakeTurn();
        }
    }

}
