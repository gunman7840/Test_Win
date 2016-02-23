using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class SawGun3 : Missile_GunType
{
    //private Transform missile;
    private int dir=1; //направление в котором будет крутится фреза

    void Awake()
    {
        MissileSpeed = 15;
        Missile_pos_dist = 2;
        DetectRadius = 6;
        turret_pr_name = "Turret_saw";
        turret_base_pr_name = "Turret_base_prefab";
    }



    protected override void Shoot()
    {
        //Debug.Log("Shoot derived");

        Quaternion q = Quaternion.AngleAxis(targetHeading, Vector3.forward);
        Missile_pos = (Vector2)transform.position + new Vector2(Missile_pos_dist * Mathf.Cos(targetHeading * Mathf.Deg2Rad), Missile_pos_dist * Mathf.Sin(targetHeading * Mathf.Deg2Rad));
        try
        {
            missile = PoolBoss.SpawnInPool(missilePrefab.transform, Missile_pos, Quaternion.Slerp(transform.rotation, q, 1f));
            missile.gameObject.SendMessage("SpeedUp", new Vector2(MissileSpeed * Mathf.Cos(targetHeading * Mathf.Deg2Rad), MissileSpeed * Mathf.Sin(targetHeading * Mathf.Deg2Rad)));

            //Debug.Log("targetHeading " + targetHeading);
            dir = (targetHeading < -90 || targetHeading > 90) ? 1 : -1; //Вращение фрезы в нужную сторону
           
            missile.gameObject.SendMessage("SetSpinDirection", dir);
        }
        catch
        {
        }
    }


}
