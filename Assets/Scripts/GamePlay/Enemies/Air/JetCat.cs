using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class JetCat : AirEnemy
{
    void OnSpawned()
    {
        AirEnemyOnSpawned();
        //Health = 40;
        this.enabled = true;
        gameObject.tag = "Enemy";
    }
}