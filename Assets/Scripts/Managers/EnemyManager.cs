using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

	// Use this for initialization
	void Start () {

        EventManager.OnTargetReached += GameOver; //Подписываемся на событие(Добавляем метод который будет исполнятся по этому событию)

    }

    // Update is called once per frame
    void Update () {
	
	}


    void GameOver(GameObject enemy)
    {
        Debug.Log("--------------GameOver---------------");
        Destroy(enemy);
    }



}
