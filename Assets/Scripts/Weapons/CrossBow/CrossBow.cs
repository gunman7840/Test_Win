using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class CrossBow : Missile_GunType
{




    protected override void Shoot()
    {
        //Debug.Log("Shoot derived");
       
        Quaternion q = Quaternion.AngleAxis(targetHeading, Vector3.forward);
        Missile_pos = (Vector2)transform.position + new Vector2(Missile_pos_dist * Mathf.Cos(targetHeading * Mathf.Deg2Rad), Missile_pos_dist * Mathf.Sin(targetHeading * Mathf.Deg2Rad));
        GameObject missile = (GameObject)Instantiate(missilePrefab, Missile_pos, Quaternion.Slerp(transform.rotation, q, 1f));
        Rigidbody2D ArrowRB = missile.GetComponent<Rigidbody2D>();
        ArrowRB.velocity = new Vector2(MissileSpeed * Mathf.Cos(targetHeading * Mathf.Deg2Rad), MissileSpeed * Mathf.Sin(targetHeading * Mathf.Deg2Rad)); ;
        
    }


}