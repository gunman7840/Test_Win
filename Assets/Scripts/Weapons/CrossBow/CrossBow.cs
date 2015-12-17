﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class CrossBow : Missile_GunType
{
    //private Transform missile;

    void Awake()
    {
        MissileSpeed = 15;
        Missile_pos_dist = 2;
        DetectRadius = 6;
    }



    protected override void Shoot()
    {
        //Debug.Log("Shoot derived");
       
        Quaternion q = Quaternion.AngleAxis(targetHeading, Vector3.forward);
        Missile_pos = (Vector2)c_transform.position + new Vector2(Missile_pos_dist * Mathf.Cos(targetHeading * Mathf.Deg2Rad), Missile_pos_dist * Mathf.Sin(targetHeading * Mathf.Deg2Rad));

        missile= PoolBoss.SpawnInPool(missilePrefab.transform, Missile_pos, Quaternion.Slerp(c_transform.rotation, q, 1f));

        //GameObject missile = (GameObject)Instantiate(missilePrefab, Missile_pos, Quaternion.Slerp(transform.rotation, q, 1f));
        //Rigidbody2D ArrowRB = missile.GetComponent<Rigidbody2D>();
        //ArrowRB.velocity = new Vector2(MissileSpeed * Mathf.Cos(targetHeading * Mathf.Deg2Rad), MissileSpeed * Mathf.Sin(targetHeading * Mathf.Deg2Rad)); 
        Rigidbody2D rb = missile.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(MissileSpeed * Mathf.Cos(targetHeading * Mathf.Deg2Rad), MissileSpeed * Mathf.Sin(targetHeading * Mathf.Deg2Rad));

    }


}