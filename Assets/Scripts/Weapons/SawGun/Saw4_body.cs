using UnityEngine;
using System.Collections;
using System.Collections.Generic;



class Saw4_body : MonoBehaviour
{
    public Rigidbody2D rb;
    private BoxCollider2D col_comp;
    public Transform c_tr;

    public int direction = 1;
    private bool FirstCollide;

    //protected DebugManager debugmanager;

    //Transform body_tr;
    public Rigidbody2D body_rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //col_comp = GetComponent<BoxCollider2D>();
        c_tr = transform;
         //debugmanager = GameObject.Find("DebugManager").GetComponent<DebugManager>();
    }

    void OnSpawned()
    {
        //BounceCounter = 0;
        //direction = direction * (-1);
        FirstCollide = false;
    }



    void Update()
    {
        //Debug.Log("saw speed " + rb.velocity);
        rb.angularVelocity = 4500 * direction;
    }


    void OnCollisionEnter2D(Collision2D coll)
    {
        if(!FirstCollide)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.3f); // чтобы он не отскакивал очень сильно при первом ударе о землю
        }
       
        foreach (ContactPoint2D contact in coll.contacts)
            {
            //debugmanager.DrawDebugLine(new Vector2(0, 0), contact.point, Color.red);
            float diff = contact.point.x - c_tr.position.x;
            if((Mathf.Abs(diff) > 0.15f))   //   diff /Mathf.Abs(diff) == direction) это необязательно
                {
                //Debug.Log("direction " + direction);
                direction = direction * (-1);
                }


            }

        FirstCollide = true;


    }



    





}

