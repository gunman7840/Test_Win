using UnityEngine;
using System.Collections;
using System.Collections.Generic;



class Saw4 : MonoBehaviour
{
    public Rigidbody2D rb;
    private BoxCollider2D col_comp;
    public Transform c_tr;
    public int Damage;

    private int startdirection=1;

    //Transform body_tr;
    public Rigidbody2D body_rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //col_comp = GetComponent<BoxCollider2D>();
        c_tr = transform;

        foreach (Transform t in c_tr)
        {
            if (t.name == "Saw_Body")
            {
                body_rb = t.GetComponent<Rigidbody2D>();
            }
        }
       
    }

    void OnSpawned()
    {
        //BounceCounter = 0;
        startdirection = startdirection * (-1);
        StartCoroutine(Disappear());
    }


    void SpeedUp(Vector2 vel)
    {
        body_rb.velocity = vel;
        rb.velocity = vel;


    }

    
    void Update()
    {
        //Debug.Log("saw speed " + rb.velocity);

        //body_rb.angularVelocity = 2500 * startdirection;
    }
    



    void OnTriggerEnter2D(Collider2D coll)
    {

        if (coll.gameObject.tag == "Enemy" || coll.gameObject.tag == "Inv_enemy")
        {
           // Debug.Log("enemy");
            coll.gameObject.SendMessage("ApplyDamage", Damage);
        }
        else if (coll.gameObject.tag == "units")
        {
           


        }
    }


    IEnumerator Disappear()
    {
        while (true)
        {
            Debug.Log("Time to die 1 ");
            yield return new WaitForSeconds(7);
            Debug.Log("Time to die " );
            PoolBoss.Despawn(c_tr);
        }
    }

    void SetSpinDirection(int dir)
    {
        body_rb.gameObject.GetComponent<Saw4_body>().direction = dir;
    }


}

