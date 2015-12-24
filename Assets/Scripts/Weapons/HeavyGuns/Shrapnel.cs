using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class Shrapnel : MonoBehaviour
{
    public Rigidbody2D rb;
    private BoxCollider2D col_comp;
    public Transform c_tr;
    public int Damage;

    void Awake()
    {
        c_tr = transform;
    }

    void OnSpawned()
    {
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Enemy")
        {
            coll.gameObject.SendMessage("ApplyDamage", Damage);
        }

        PoolBoss.Despawn(c_tr);
    }



 


}

