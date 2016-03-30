using UnityEngine;
using System.Collections;
using System.Collections.Generic;


abstract class Missile_GunType : WeaponType
{

    Collider2D[] hitColliders;
    public GameObject missilePrefab;
    public Transform TurretPrefab;
    public Transform Turret_base_Prefab;
    public float Base_angle;  //Допустимы значения 0,90,-90,180
    public LayerMask myLayerMask;
    //public int Damage; //устанавливается в снаряде

    protected Transform missile;
    protected Transform turret;
    protected Transform turret_base;
    protected string turret_pr_name= "Turret_prefab";
    protected string turret_base_pr_name= "Turret_base_prefab";

    //------- Поворот
    protected float MinTurnAngle;
    protected float MaxTurnAngle;
    protected float targetHeading;
    //protected Vector2 targetPos;
    protected float TurnSpeed = 300;

    //--------Detect
    protected GameObject currentTarget;
    protected Transform currentTargetTransform;
    protected EnemyType currentTargetState;
    protected bool TargetLock=false;

    //--------Стрельба
    public float MissileSpeed ;
    public float Missile_pos_dist;

    protected Vector2 Missile_pos;
    protected Vector2 RCScanner_pos;  // точка на конце ствола, из нее строим рейкасты до цели 
    protected bool TakeToAim =false;  // вроде используется только в гравипушке, там мы не обновляли скрипты
    protected bool ReadyToShoot = true;  
    //-----Debug 
    protected DebugManager debugmanager;

    //------Spawning
    protected bool isReused = false;


    void Start()
    {
        //Debug.Log("Start");
        c_transform = transform;
        uimanager = GameObject.Find("UIManager").GetComponent<UIManager>();
        debugmanager = GameObject.Find("DebugManager").GetComponent<DebugManager>();

        foreach (Transform t in transform)
        {
            if (t.name == turret_pr_name)
            {
                turret = t;
            }
            else if (t.name == turret_base_pr_name)
            {
                turret_base = t;
            }
        }

        turret.position = c_transform.position;
        turret.rotation = Quaternion.AngleAxis(Base_angle - 90, new Vector3(0, 0, 1));  // -90 чтобы ствол всегда смотерл в направлении противоположном крпелению базы
        turret_base.position = c_transform.position;
        turret_base.rotation = Quaternion.AngleAxis(Base_angle, new Vector3(0, 0, 1)); // 0 0 1 это ось по которой идет вращение , то есть z

        MinTurnAngle = Base_angle - 180;
        MaxTurnAngle = Base_angle ;
        RCScanner_pos = (Vector2)turret.position + new Vector2(Missile_pos_dist * Mathf.Cos((Base_angle - 90) * Mathf.Deg2Rad), Missile_pos_dist * Mathf.Sin((Base_angle - 90) * Mathf.Deg2Rad));


        targetHeading = 0f;
        StartCoroutine(ScanArea());
        isReused = true; //Тут мы запускаем ScanArea при старте , когда спауним башню в первый раз. А в методе Onspawn будем запускать эту корутину только если она используется повторно
    }

    protected void Missile_GunType_OnSpawned()
    {
        //Debug.Log("Missile_GunType_OnSpawned");
        if (isReused)
        { 
        StartCoroutine(ScanArea());
        ReadyToShoot = true;
        }
    }

    void Update()
    {
        //Debug.Log("desc " + description);

        if (currentTarget == null)
            TargetLock = false; //если цель исчезла из памяти , например по итогам StuckCourutine

        if (TargetLock)
        {
            if (turret.eulerAngles.z != targetHeading)
            {
                Quaternion q = Quaternion.AngleAxis(targetHeading, Vector3.forward); // то же что Vector3(0, 0, 1)
                turret.rotation = Quaternion.RotateTowards(turret.rotation, q, Time.deltaTime * TurnSpeed);
            }
          
            if (ReadyToShoot && ((turret.eulerAngles.z - targetHeading < 0.3 && turret.eulerAngles.z - targetHeading > -0.3) || (turret.eulerAngles.z - 360f - targetHeading < 0.3 && turret.eulerAngles.z - 360f - targetHeading > -0.3))) 
            {
                RaycastHit2D hit = Physics2D.Linecast(RCScanner_pos, currentTargetTransform.position, myLayerMask); //Если он захватил цель , то продолжает вести ее и после того как она скрылась за углом, поэтому нужен рэйкаст
                //debugmanager.DrawDebugLine(turret.position, currentTargetTransform.position, Color.cyan);
                //debugmanager.DrawDebugRay(turret.position, targetHeading, 10, Color.red);
                if (hit.rigidbody != null && hit.rigidbody.tag == "Enemy")
                {
                    Shoot();
                    ReadyToShoot = false;
                    StartCoroutine(Recharging());
                }
                else
                {
                    TargetLock = false;// цель скрылась за преградой, ищем новую
                }   
            }
        }
        else
        {
            //random move
        }
        
    }


    
    protected IEnumerator ScanArea()
    {
        while (true)
        {
            //Debug.Log("ScanArea");
            if (!TargetLock) //Ищем только если мы не ведем цель в данный момент
            {
                DetectEnemy(DetectRadius);
            }
            else
            {
                if (currentTargetTransform != null)
                {
                    //Debug.DrawLine(turret.position, currentTargetTransform.position, Color.white);
                }
                //эта строка ломается если враг исчез например через stuckcourutine
                float distance=Vector2.Distance(currentTargetTransform.position,turret.position);
                //debugmanager.DrawDebugLine(RCScanner_pos, currentTargetTransform.position, Color.red);
                if (distance < DetectRadius && currentTarget.tag == "Enemy") //оставить проверку тэга иначе ломается
                {
                    Vector2 vel = currentTargetState.vel;
                    Vector2 nextTargetposition = (Vector2)currentTargetTransform.position + (Vector2)vel * Time.deltaTime * 10; //Стреляем на опережение по времени deltatime*5 это примерно 0.25 секунды, взято от балды
                    Vector2 vectorToTarget;
                    if (currentTargetState.isOnGround == false)
                    { vectorToTarget = (Vector2)currentTargetTransform.position - (Vector2)turret.position; }
                    else
                    { vectorToTarget = (Vector2)nextTargetposition - (Vector2)turret.position; }

                    targetHeading = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;

                    //Если цель ушла за угол, то башня следит за ней до момента выстрела и толлько потом делает рэйкаст, сбрасывает цель и начинает искать новую. Поэтому проверяем угол здесь
                    if ((Base_angle != -90 && !(targetHeading >= MinTurnAngle && targetHeading <= MaxTurnAngle)) || (Base_angle == -90 && !(targetHeading >= MaxTurnAngle * (-1) || targetHeading <= MaxTurnAngle))) //-90 (ствол смотрит влево) является исключением из алгоритма, пришлось делать отдельное условие для него
                    {
                        TargetLock = false; //цель находится за башней 
                    }
                    //debugmanager.DrawDebugRay(turret.position, targetHeading, 10, Color.cyan);
                }
                else
                {
                    TargetLock = false; //цель вышла из зоны досягаемости
                }
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    protected void DetectEnemy(int radius)
    {
        //Debug.Log("DetectEnemy "+ DetectRadius);
        currentTarget = null; //зануляем текущую цель
        hitColliders = Physics2D.OverlapCircleAll(turret.position, radius, myLayerMask);
        float distance = Mathf.Infinity;
        Vector2 position = turret.position;

        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.attachedRigidbody.tag == "Enemy")
            {
                Rigidbody2D enemy_rb = hitCollider.attachedRigidbody;
                float _angle = Mathf.Atan2(turret.position.y - enemy_rb.position.y, turret.position.x - enemy_rb.position.x) * Mathf.Rad2Deg;
                //Корректируем угол , в зависимости кто над кем находится. Детектим только врагов в нашем секторе шириной 180 градусов
                if ((enemy_rb.position.y - c_transform.position.y) > 0)
                    _angle = _angle + 180;
                else if ((enemy_rb.position.y - c_transform.position.y) < 0)
                    _angle = _angle - 180;

                if ((Base_angle != -90 && (_angle >= MinTurnAngle && _angle <= MaxTurnAngle)) || (Base_angle == -90 && (_angle >= MaxTurnAngle * (-1) || _angle <= MaxTurnAngle))) //-90 (ствол смотрит влево) является исключением из алгоритма, пришлось делать отдельное условие для него
                {
                    RaycastHit2D hit = Physics2D.Linecast(turret.position, hitCollider.attachedRigidbody.position, myLayerMask);
                    //debugmanager.DrawDebugLine(RCScanner_pos, hit.point, Color.red);
                    if (hit.rigidbody != null)
                    {
                        if (hit.rigidbody.tag == "Enemy")
                        {
                            Vector3 diff = hitCollider.attachedRigidbody.position - position;
                            float curDistance = diff.sqrMagnitude;
                            if (curDistance < distance)
                            {
                                currentTarget = hitCollider.gameObject;
                                distance = curDistance;
                            }
                        }
                    }
                }
            }
        }
        if (currentTarget != null)
        {
            TargetLock = true;
            currentTargetTransform = currentTarget.transform;
            currentTargetState = currentTarget.GetComponent<EnemyType>();

            //Сразу же определяем targetHeading, чтобы не дожидаться следующего вызова ScanArea
            Vector2 vel = currentTargetState.vel;
            Vector2 nextTargetposition = (Vector2)currentTargetTransform.position + (Vector2)vel * Time.deltaTime * 7; //Стреляем на опережение по времени deltatime*5 это примерно 0.25 секунды, взято от балды
            Vector2 vectorToTarget = (Vector2)nextTargetposition - (Vector2)turret.position;
            targetHeading = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        }
        else
        {
            TargetLock = false;
        }
    }

    protected IEnumerator Recharging()
    {
        //Видимо здесь будет анимация перезарядки
        //Debug.Log("Recharging");
        yield return new WaitForSeconds(RechargeTime);
        ReadyToShoot = true;
    }

    protected virtual void Shoot()
    {
        //Debug.Log("Shoot base");
    }
    
}