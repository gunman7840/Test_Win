using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemyType : MonoBehaviour
{
    public int id;
    public float Health;
    public string type; //air/land
    protected EnemyManager enemymanager;
    protected LayerMask myLayerMask;
    protected Transform _transform;


    //Рассчет траектории
    public TargetPoint NextTarget;  //Устанавливает из вне при спауне
    protected Path Trajectory;
    protected Navigation navigation;

    //------События 
    protected EventManager eventmanager;

    //-----Debug 
    protected DebugManager debugmanager;


    protected void EnemyType_Awake()
    {
        //Debug.Log("enemy type awake");
        _transform = transform;
        enemymanager = GameObject.Find("Main Camera").GetComponent<EnemyManager>();
        debugmanager = GameObject.Find("DebugManager").GetComponent<DebugManager>();
        eventmanager = GameObject.Find("Main Camera").GetComponent<EventManager>();

        //Получаем первую траекторию
        navigation = GameObject.Find("Main Camera").GetComponent<Navigation>(); //Получаем доступ к классу

        myLayerMask = enemymanager.myLayerMask;

    }

    public void EnemyTypeOnSpawned()
    {
        //Debug.Log("EnemytypeAfterSpawn NextTarget  " + NextTarget.position); // Nexttarget 
        //NextTarget = navigation.GetStartPoint(_transform.position);
        //Trajectory = navigation.GetPath(NextTarget);
     }

}