using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

    public LayerMask myLayerMask; //маска по которой будут делать рэйкасты все враги
    int Enemynumber = 0;

    

    //public GameObject[] EnemyPrefabs;
    //Hashtable EnemyHT = new Hashtable();


    // Use this for initialization
    void Awake () {
        //Debug.Log("enmey manager awake");
        EventManager.OnTargetReached += DespawnEnemy;
        EventManager.OnDestroyEnemy += DespawnEnemy;
    }

    void DespawnEnemy(Transform enemy_tr)
    {
        PoolBoss.Despawn(enemy_tr);
    }


}
