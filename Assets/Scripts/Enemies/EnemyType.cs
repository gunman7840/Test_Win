using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemyType : MonoBehaviour
{
    public int id;
    public float Health;
    protected EnemyManager enemymanager;
    protected LayerMask myLayerMask;


    protected void EnemyType_Awake()
    {
        //Debug.Log("enemy type awake");
        enemymanager = GameObject.Find("Main Camera").GetComponent<EnemyManager>();
        myLayerMask = enemymanager.myLayerMask;

    }

}