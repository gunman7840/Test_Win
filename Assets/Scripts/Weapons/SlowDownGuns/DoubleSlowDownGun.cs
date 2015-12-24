using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class DoubleSlowDownGun : Missile_GunType
{
    //private Transform missile;

    public Transform Missile_pos_l;
    public Transform Missile_pos_r;

    private Transform missile2;
    private Transform missile3;

    void Awake()
    {
        MissileSpeed = 15;
        Missile_pos_dist = 2;
        DetectRadius = 6;
        turret_pr_name = "Turret_prefab_SlowDown";
        turret_base_pr_name = "Turret_base_prefab";


    }



    protected override void Shoot()
    {
        //Debug.Log("Shoot derived");

        Quaternion q = Quaternion.AngleAxis(targetHeading, Vector3.forward);
        Missile_pos = (Vector2)transform.position + new Vector2(Missile_pos_dist * Mathf.Cos(targetHeading * Mathf.Deg2Rad), Missile_pos_dist * Mathf.Sin(targetHeading * Mathf.Deg2Rad));

        //missile = PoolBoss.SpawnInPool(missilePrefab.transform, Missile_pos, Quaternion.Slerp(transform.rotation, q, 1f));
        //Rigidbody2D rb = missile.GetComponent<Rigidbody2D>();
        //rb.velocity = new Vector2(MissileSpeed * Mathf.Cos(targetHeading * Mathf.Deg2Rad), MissileSpeed * Mathf.Sin(targetHeading * Mathf.Deg2Rad));

        missile2 = PoolBoss.SpawnInPool(missilePrefab.transform, Missile_pos_l.position, Quaternion.Slerp(turret.rotation, q, 1f));
        Rigidbody2D rb2 = missile2.GetComponent<Rigidbody2D>();
        rb2.velocity = new Vector2(MissileSpeed * Mathf.Cos(targetHeading * Mathf.Deg2Rad), MissileSpeed * Mathf.Sin(targetHeading * Mathf.Deg2Rad));
        Debug.Log("vel2" + rb2.velocity);

        missile3 = PoolBoss.SpawnInPool(missilePrefab.transform, Missile_pos_r.position, Quaternion.Slerp(turret.rotation, q, 1f));
        Rigidbody2D rb3 = missile3.GetComponent<Rigidbody2D>();
        rb3.velocity = new Vector2(MissileSpeed * Mathf.Cos(targetHeading * Mathf.Deg2Rad), MissileSpeed * Mathf.Sin(targetHeading * Mathf.Deg2Rad));
        Debug.Log("vel3" + rb3.velocity);

    }


}