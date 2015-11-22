using UnityEngine;
using System.Collections;
using System.Collections.Generic;



class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D col_comp;
    private Transform c_tr;

    void Start()
    {
        Destroy(gameObject, 5f);
        rb = GetComponent<Rigidbody2D>();
        col_comp = GetComponent<BoxCollider2D>();
        c_tr = transform;
    }

    

    void OnCollisionEnter2D(Collision2D coll)
    {
        ArrowStick(coll);
        

        if (coll.gameObject.tag == "Enemy")
        {
            //Destroy(coll.gameObject);
           
            coll.gameObject.SendMessage("ApplyDamage", 10);
            //Destroy(gameObject);
        }   
    }

    void ArrowStick(Collision2D col)
    {

        //Debug.Log("Stick");
        rb.isKinematic = true; // stop physics
        Destroy(rb);
       
        Destroy(col_comp);
        c_tr.Translate(new Vector2(0.5f,0),Space.Self);
        c_tr.parent = col.transform;
        
    }

}

