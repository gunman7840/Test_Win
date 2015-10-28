using UnityEngine;
using System.Collections;
using System.Collections.Generic;



class Arrow : MonoBehaviour
{
    void Update()
    {

    }

    void Start()
    {
        Destroy(gameObject, 1f);
    }

    

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Enemy")
        {
            Destroy(coll.gameObject);
            Destroy(gameObject);
            //coll.gameObject.SendMessage("ApplyDamage", 10);
        }   
    }

}

