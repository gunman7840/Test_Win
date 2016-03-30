using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemyType : MonoBehaviour
{
    //------Параметры
    public int id;
    public float Health;
    public string type; //air/land
    public int cost;
    protected LayerMask myLayerMask;


    //----Состояние
    protected Transform _transform;
    public Vector2 vel; //к ней нужен доступ из башен, чтобы стрелять на опережение
    public bool isOnGround = false; //нужен доступ из башен, чтобы не стрелять на опережение во время прыжков
    public bool Alive = true;
    protected float DeadBodytime = 5f;

    //-----Ссылки
    private Camera _camera;
    //protected ResourceManager resourceManager;
    protected EventManager eventmanager;
    protected DebugManager debugmanager;
    protected EnemyManager enemymanager;
    protected Navigation navigation;



    //Рассчет траектории
    public TargetPoint NextTarget;  //Устанавливает из вне при спауне
    protected Path Trajectory;



    protected void EnemyType_Awake()
    {
        //Debug.Log("enemy type awake");
        _transform = transform;
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        enemymanager = _camera.GetComponent<EnemyManager>();
        debugmanager = GameObject.Find("DebugManager").GetComponent<DebugManager>();
        eventmanager = _camera.GetComponent<EventManager>();
        //resourceManager = _camera.GetComponent<ResourceManager>();
        navigation = _camera.GetComponent<Navigation>(); //Получаем доступ к классу

        myLayerMask = enemymanager.myLayerMask;

    }

    public void EnemyTypeOnSpawned()
    {
        //Debug.Log("EnemytypeAfterSpawn NextTarget  " + NextTarget.position); // Nexttarget 
        //NextTarget = navigation.GetStartPoint(_transform.position);
        //Trajectory = navigation.GetPath(NextTarget);
     }

    public void ApplyDamage(int points)
    {
        //Debug.Log("ApplyDamage " + points);
        Health -= points;
    }

    protected IEnumerator Die()
    {
        eventmanager.CallOnDestroyEnemy_cost(cost);
        this.enabled = false;
        Alive = false;
        gameObject.tag = "Dead";

        yield return new WaitForSeconds(DeadBodytime);

        eventmanager.CallOnDestroyEnemy(_transform);

    }

}