using UnityEngine;
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

        //StartCoroutine(CreateEnemy());
    }

    // Update is called once per frame
    void Update () {
	
	}


    void GameOver(GameObject enemy)
    {
        TargetRiched++;
        Debug.Log("--------------GameOver-----------------------------------------------number" + TargetRiched);
        Destroy(enemy);
    }

    void DestroyEnemy(GameObject enemy)
    {
        enemiesdestroed++;
        Debug.Log("--------------Destroy------------------------------number" + enemiesdestroed);

        Destroy(enemy);
    }

    IEnumerator CreateEnemy()
    {

        while (true)
        {
            Instantiate(enemyPrefab, new Vector2(10,7), Quaternion.identity);
            Enemynumber++;
            //Debug.Log("Enemy № " + Enemynumber);
            yield return new WaitForSeconds(2f);
           
        }
    }

}
