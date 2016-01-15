using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

    public LayerMask myLayerMask; //маска по которой будут делать рэйкасты все враги
    int Enemynumber = 0;
    int TargetRiched = 0;
    int enemiesdestroed = 0;

    public GameObject[] EnemyPrefabs;
    Hashtable EnemyHT = new Hashtable();
    private int[] WavesMap = {1,1,1,1,1005,2,2,2,3,3,20,1010,30,30,20,30,1005,40,20,40,20}; 


    // Use this for initialization
    void Awake () {
        //Debug.Log("enmey manager awake");
        EventManager.OnTargetReached += GameOver; //Подписываемся на событие(Добавляем метод который будет исполнятся по этому событию)
        //EventManager.DestroyEnemy_event += DestroyEnemy; 

        foreach(GameObject enemy in EnemyPrefabs)
        {
            EnemyHT.Add(enemy.GetComponent<EnemyType>().id, enemy.transform);
        }

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
        Debug.Log("--------------Destroy------------------------------number" + _enemytransform);
        PoolBoss.Despawn(_enemytransform);
    }

    IEnumerator CreateEnemy()
    {

        foreach(int unit in WavesMap)
        {
            yield return new WaitForSeconds(2f);
            if(unit >1000)
            {
                float pause = unit - 1000;
                yield return new WaitForSeconds(pause);
            }
            else
            PoolBoss.SpawnInPool((Transform)EnemyHT[unit], new Vector2(2, 5), Quaternion.identity);
        }

    }

}
