using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

    public LayerMask myLayerMask; //маска по которой будут делать рэйкасты все враги
    int Enemynumber = 0;
    int TargetRiched = 0;
    int enemiesdestroed = 0;

    //public GameObject[] EnemyPrefabs;
    //Hashtable EnemyHT = new Hashtable();


    // Use this for initialization
    void Awake () {
        //Debug.Log("enmey manager awake");
        EventManager.OnTargetReached += GameOver; //Подписываемся на событие(Добавляем метод который будет исполнятся по этому событию)
        //EventManager.DestroyEnemy_event += DestroyEnemy; 
    }

    void GameOver(GameObject enemy)
    {
        TargetRiched++;
        PoolBoss.Despawn(enemy.transform);
    }

    public void  DestroyEnemy(Transform _enemytransform)
    {
        enemiesdestroed++;
        Debug.Log("--------------Destroy------------------------------number" + _enemytransform);
        PoolBoss.Despawn(_enemytransform);
    }

   
}
