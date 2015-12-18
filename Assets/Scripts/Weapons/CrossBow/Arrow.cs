using UnityEngine;
using System.Collections;
using System.Collections.Generic;



class Arrow : MonoBehaviour
{
    public Rigidbody2D rb;
    private BoxCollider2D col_comp;
    public Transform c_tr;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col_comp = GetComponent<BoxCollider2D>();
        c_tr = transform;
       
    }

    void OnSpawned()
    {
        Debug.Log("OnSpawned");
        rb.IsAwake();
        rb.isKinematic = false;
        c_tr.parent = null;
        col_comp.enabled = true;
       StartCoroutine(Disappear());
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        ArrowStick(coll);

        if (coll.gameObject.tag == "Enemy")
        {         
            coll.gameObject.SendMessage("ApplyDamage", 10);
        }   
    }

    void ArrowStick(Collision2D col)
    {

        Debug.Log("Stick");
        
        rb.isKinematic = true; // stop physics
        rb.Sleep();

        col_comp.enabled = false;
        c_tr.Translate(new Vector2(0.5f,0),Space.Self);
        c_tr.parent = col.transform;
        
    }

    IEnumerator Disappear()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            //Debug.Log("Time to die " );
            PoolBoss.Despawn(c_tr);
        }
    }

    

}

