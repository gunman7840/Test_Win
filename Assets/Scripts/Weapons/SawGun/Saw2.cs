using UnityEngine;
using System.Collections;
using System.Collections.Generic;



class Saw2 : MonoBehaviour
{
    public Rigidbody2D rb;
    private BoxCollider2D col_comp;
    public Transform c_tr;
    public int Damage;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //col_comp = GetComponent<BoxCollider2D>();
        c_tr = transform;

    }

    void OnSpawned()
    {
        //   Debug.Log("OnSpawned");
        //rb.IsAwake();
        //rb.isKinematic = false;
        //c_tr.parent = null;
        //col_comp.enabled = true;
        //StartCoroutine(Disappear());
    }

    void OnTriggerEnter2D(Collider2D coll)
    {

        if (coll.gameObject.tag == "Enemy")
        {
            Debug.Log("enemy");
            coll.gameObject.SendMessage("ApplyDamage", Damage);
        }
        else
        {
            //Здесь нужно будет запустить анимацию разрушения фрезы
            Debug.Log("ground");
            PoolBoss.Despawn(c_tr);
        }
    }



}

