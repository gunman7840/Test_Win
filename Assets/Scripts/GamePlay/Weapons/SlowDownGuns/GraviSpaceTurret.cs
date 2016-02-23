using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class GraviSpaceTurret : MonoBehaviour
{
    private List<GameObject> EnemiesList=new List<GameObject>();

    public void OnActivate()
    {
        //if(EnemiesList.Count >0)
        EnemiesList.Clear();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        //Debug.Log("TrigerEnter");

        if (coll.gameObject.tag == "Enemy")
        {
            //coll.gameObject.SendMessage("SlowDown_message", 2);
            coll.gameObject.GetComponent<Enemy_infantry>().isActive = false;
            Rigidbody2D rb = coll.gameObject.GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x * 0.4f, 1);
            EnemiesList.Add(coll.gameObject);
            //rb.velocity = new Vector2(0,0.1f);
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        coll.gameObject.GetComponent<Enemy_infantry>().isActive = true;
        Rigidbody2D rb = coll.gameObject.GetComponent<Rigidbody2D>();
        rb.gravityScale = 1;

        if (EnemiesList.Contains(coll.gameObject))
        {
            //remove it from the list
            EnemiesList.Remove(coll.gameObject);
        }
    }

    public void OnDeActivate()
    {
        foreach(GameObject _enemy in EnemiesList)
        {
            _enemy.GetComponent<Enemy_infantry>().isActive = true;
            Rigidbody2D rb = _enemy.GetComponent<Rigidbody2D>();
            rb.gravityScale = 1;
        }
    }


    //Нужно как-то вызвать метод OnTriggerExit2D перед отключением поля
}