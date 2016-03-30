using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour {



    //--------------------------------------------------------------SCORE
    public delegate void ChangeResources(int quantity);
    public event ChangeResources OnChangeResources; //На это событие подписаны элеманты гуи, которые обновляют свои свойства, вызывает оно из ресурс мэнеджера
    public void CallOnChangeResources(int quantity)
    {
        if(OnChangeResources!=null)
        OnChangeResources(quantity);
    }


    public delegate void Setwafes_start(int wafes);
    public event Setwafes_start OnSetwafes_start; 
    public void CallOnSetwafes_start(int wafes)
    {
        if (OnSetwafes_start != null)
            OnSetwafes_start(wafes);
    }

    public delegate void EncreaseWafe();
    public event EncreaseWafe OnEncreaseWafe; 
    public void CallOnEncreaseWafe()
    {
        if (OnEncreaseWafe != null)
            OnEncreaseWafe();
    }

    public delegate void CallWafe();
    public event CallWafe OnCallWafe;
    public void CallOnCallWafe()
    {
        if (OnCallWafe != null)
            OnCallWafe();
    }

    //----------------------------------------------------------enemies
    public delegate void TargetReached(Transform enemy_tr);
    public static event TargetReached OnTargetReached;
    public void CallOnTargetReached(Transform enemy_tr)
    {
        OnTargetReached(enemy_tr);
    }

    public delegate void DestroyEnemy(Transform enemy_tr);
    public static event DestroyEnemy OnDestroyEnemy;
    public void CallOnDestroyEnemy(Transform enemy_tr)
    {
        OnDestroyEnemy(enemy_tr);
    }

    public delegate void DestroyEnemy_cost( int cost);
    public static event DestroyEnemy_cost OnDestroyEnemy_cost;
    public void CallOnDestroyEnemy_cost( int cost)
    {
        OnDestroyEnemy_cost( cost);
    }

    //------------------------------------------------------------GamePlay
    public delegate void GameOver();
    public  event GameOver OnGameOver;
    public void OnGameOver_meth()
    {
        OnGameOver();
    }


    void Start()
    {

    }

}
