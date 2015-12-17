﻿using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

    public GameObject enemyPrefab;
    int Enemynumber = 0;
    int TargetRiched = 0;
    int enemiesdestroed = 0;

    // Use this for initialization
    void Start () {
        EventManager.OnTargetReached += GameOver; //Подписываемся на событие(Добавляем метод который будет исполнятся по этому событию)
        //EventManager.DestroyEnemy_event += DestroyEnemy; 

        StartCoroutine(CreateEnemy());
    }

    void GameOver(GameObject enemy)
    {
        TargetRiched++;
        PoolBoss.Despawn(enemy.transform);
    }

    public void  DestroyEnemy(Transform _enemytransform)
    {
        enemiesdestroed++;
        //Debug.Log("--------------Destroy------------------------------number" + enemiesdestroed);
        PoolBoss.Despawn(_enemytransform);
    }

    IEnumerator CreateEnemy()
    {

        while (true)
        {
            //Instantiate(enemyPrefab, new Vector2(2,5), Quaternion.identity);
            PoolBoss.SpawnInPool(enemyPrefab.transform, new Vector2(2, 5), Quaternion.identity);
            
            Enemynumber++;
            //Debug.Log("Enemy № " + Enemynumber);
            yield return new WaitForSeconds(1f);
           
        }
    }

}
