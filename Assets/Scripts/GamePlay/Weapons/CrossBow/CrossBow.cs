using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class CrossBow : Missile_GunType
{
    //private Transform missile;


    void OnSpawned()
    {
        Missile_GunType_OnSpawned();
    }

    protected override void Shoot()
    {
        //Debug.Log("Shoot derived");

        Quaternion q = Quaternion.AngleAxis(targetHeading, Vector3.forward);
        Missile_pos = (Vector2)c_transform.position + new Vector2(Missile_pos_dist * Mathf.Cos(targetHeading * Mathf.Deg2Rad), Missile_pos_dist * Mathf.Sin(targetHeading * Mathf.Deg2Rad));
        try
        { 
        missile = PoolBoss.SpawnInPool(missilePrefab.transform, Missile_pos, Quaternion.Slerp(turret.rotation, q, 1f));        
        Rigidbody2D rb = missile.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(MissileSpeed * Mathf.Cos(targetHeading * Mathf.Deg2Rad), MissileSpeed * Mathf.Sin(targetHeading * Mathf.Deg2Rad));

        }
        catch
        {
        }
    }


}