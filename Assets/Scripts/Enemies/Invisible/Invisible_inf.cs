using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Invisible_inf : Enemy_infantry
{

    void OnSpawned()
    {
        Health = 10;
        this.enabled = true;
        Alive = true;
        NextTarget = navigation.GetStartPoint();
        Trajectory = navigation.GetPath(NextTarget);
        gameObject.tag = "Inv_enemy";
    }

}