using UnityEngine;
using System.Collections;
using System.Collections.Generic;



class VacuumMissile : MonoBehaviour
{
    public Rigidbody2D rb;
    public int VacuumEffect;
    private BoxCollider2D col_comp;
    private Transform c_tr;
    private bool isDetonate = false;
    private bool ReadyToDetonate = true;  //Используем этот ключ метод detonate выполнялся только один раз
    Animator anim;



    //-----explosion
    private Collider2D[] hitColliders;
    public int radius;
    public LayerMask myLayerMask;

    //-----debug
    DebugManager debugmanager;


    void Awake()
    {
        debugmanager = GameObject.Find("DebugManager").GetComponent<DebugManager>();
        rb = GetComponent<Rigidbody2D>();
        col_comp = GetComponent<BoxCollider2D>();
        c_tr = transform;
        anim = GetComponent<Animator>();
    }

    void OnSpawned()
    {
        anim.SetBool("isDetonate", false);
        ReadyToDetonate = true;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        
        if (coll.gameObject.tag == "Enemy" || coll.gameObject.tag == "Inv_enemy")
        {
           coll.gameObject.SendMessage("ApplyDamage", 30); //Это достается не только тем в кого попал снаряд , но и тем кого засасало в него
        }
        
        //PoolBoss.Despawn(c_tr);
        //rb.isKinematic = true;
       
        Detonate();

    }




    void Detonate()
    {
        if (ReadyToDetonate)
        {
            Debug.Log("detonate");
            anim.SetBool("isDetonate", true);
            //Здесь могут быть проблемы с происзводительностью
            hitColliders = Physics2D.OverlapCircleAll(c_tr.position, radius, myLayerMask);
            foreach (Collider2D hitCollider in hitColliders)
            {

                if (hitCollider.gameObject.tag == "Enemy" || hitCollider.gameObject.tag == "Inv_enemy")
                {
                    Rigidbody2D rb = hitCollider.attachedRigidbody;
                    debugmanager.DrawDebugLine(c_tr.position, rb.position, Color.red);
                    Vector2 direction = VacuumEffect * ((Vector2)c_tr.position - (Vector2)rb.position);
                    rb.AddForce(direction);
                    hitCollider.gameObject.SendMessage("ApplyDamage", 10);
                }
            }
        }
        ReadyToDetonate = false;
    }
    void Dissapear()
    {
        //вызывается из анимации
        PoolBoss.Despawn(c_tr);
    }


}

