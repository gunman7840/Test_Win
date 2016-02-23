using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Invisible_inf : Enemy_infantry
{

    void OnSpawned()
    {
        EnemyInfantryOnSpawned();
        //Health = 10;
        this.enabled = true;
        Alive = true;
        gameObject.tag = "Inv_enemy";
    }

}