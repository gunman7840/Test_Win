using UnityEngine;
using System.Collections;
using System.Collections.Generic;



class ShrapnelMissile : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject _shrapnelPrefab;
    public float _shrapnelSpeed;
    private BoxCollider2D col_comp;
    private Transform c_tr;
    private bool isDetonate = false;
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
        //Debug.Log("OnSpawned");
        //rb.IsAwake();
        rb.isKinematic = false;
        //c_tr.parent = null;
        col_comp.enabled = true;
        anim.SetBool("isDetonate", false);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Enemy")
        {
            coll.gameObject.SendMessage("ApplyDamage", 30);
        }
        rb.isKinematic = true;
        Detonate();

    }

    void Detonate()
    {
        anim.SetBool("isDetonate", true);
        /*
        // Уберем это здесь, оставим только осколки
        hitColliders = Physics2D.OverlapCircleAll(c_tr.position, radius, myLayerMask);
        foreach (Collider2D hitCollider in hitColliders)
        {
            
            if (hitCollider.gameObject.tag == "Enemy")
            {
                Rigidbody2D rb = hitCollider.attachedRigidbody;
                //debugmanager.DrawDebugLine(c_tr.position, rb.position, Color.red);
                Vector2 direction = 140 * ((Vector2)rb.position - (Vector2)c_tr.position);
                rb.AddForce(direction);
                hitCollider.gameObject.SendMessage("ApplyDamage", 5);
            }
        }
        */
        //----------------осколки
        //Здесь могут быть проблемы с происзводительностью
        col_comp.enabled = false;
        for (int i =0; i<360; i+=10)
        {
            //Debug.Log("shrapnel");
            Transform _sh = PoolBoss.SpawnInPool(_shrapnelPrefab.transform, c_tr.position, Quaternion.identity);
            Rigidbody2D rb = _sh.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(_shrapnelSpeed * Mathf.Cos(i * Mathf.Deg2Rad), _shrapnelSpeed * Mathf.Sin(i * Mathf.Deg2Rad));
        }
    }
    void Dissapear()
    {
        //вызывается из анимации
        PoolBoss.Despawn(c_tr);
    }


}

