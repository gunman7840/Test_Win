using UnityEngine;
using System.Collections;
using System.Collections.Generic;



class Saw3 : MonoBehaviour
{
    public Rigidbody2D rb;
    private BoxCollider2D col_comp;
    public Transform c_tr;
    public int Damage;
    public int BounceCounterLimit;
    private int BounceCounter;

    //Transform body_tr;
    public Rigidbody2D body_rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //col_comp = GetComponent<BoxCollider2D>();
        c_tr = transform;

        foreach (Transform t in transform)
        {
            if (t.name == "Saw_Body")
            {
                body_rb = t.GetComponent<Rigidbody2D>();
            }
        }


    }

    void SpeedUp(Vector2 vel)
    {
        body_rb.velocity = vel;
        rb.velocity = vel;
    }

    /*
    void Update()
    {
        Debug.Log("saw speed " + rb.velocity);
    }
    */

    void OnSpawned()
    {
        BounceCounter = 0;
        //   Debug.Log("OnSpawned");
        //rb.IsAwake();
        //rb.isKinematic = false;
        //c_tr.parent = null;
        //col_comp.enabled = true;
        //StartCoroutine(Disappear());
    }

    void OnTriggerEnter2D(Collider2D coll)
    {

        if (coll.gameObject.tag == "Enemy" || coll.gameObject.tag == "Inv_enemy")
        {
            Debug.Log("enemy");
            coll.gameObject.SendMessage("ApplyDamage", Damage);
        }
        else
        {
            //Debug.Log("ground");
            BounceCounter++;
            if (BounceCounter == BounceCounterLimit)
            {
                PoolBoss.Despawn(c_tr);
                //Здесь нужно будет запустить анимацию разрушения фрезы
            }
        }
    }



}

