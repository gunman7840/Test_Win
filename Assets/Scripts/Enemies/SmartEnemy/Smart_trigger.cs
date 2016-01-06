using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Smart_trigger : MonoBehaviour
{
    private Smart_m parent;
    void Awake()
    {
        parent = transform.parent.GetComponent<Smart_m>();
    }


    void OnTriggerEnter2D(Collider2D coll)
    {
        Rigidbody2D rb = coll.GetComponent<Rigidbody2D>();
        Vector2 pos = rb.position;
        //Vector2 vel = rb.velocity; //либо через угол
        float angle = rb.rotation;
        //coll.gameObject.SendMessage("ApplyDamage", Damage);

        parent.Dodge(pos,angle);
        
    }



}