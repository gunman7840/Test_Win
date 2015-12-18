﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


abstract class Missile_GunType : WeaponType
{
    Collider2D[] hitColliders;
    public GameObject missilePrefab;
    public Transform TurretPrefab;
    public Transform Turret_base_Prefab;
    public float Base_angle;
    public float ShootFrequency;
    public LayerMask myLayerMask;

    protected Transform missile;
    protected Transform turret;
    protected Transform turret_base;
    protected string turret_pr_name= "Turret_prefab";
    protected string turret_base_pr_name= "Turret_base_prefab";

    //protected Transform c_transform;

    //------- Поворот
    protected float MinTurnAngle;
    protected float MaxTurnAngle;

    protected float targetHeading;
    //protected Vector2 targetPos;
    protected float TurnSpeed = 300;

    //--------Detect
    
    GameObject currentTarget;

    //--------Стрельба
    protected float Missile_pos_dist = 1;
    protected Vector2 Missile_pos;
    protected Vector2 RCScanner_pos;  // точка на конце ствола, из нее строим рейкасты до цели 
    protected float MissileSpeed = 0f;
    public float RechargeTime = 1.5f;
    protected int DetectRadius = 10;
    protected bool TakeToAim =false;
    protected bool ReadyToShoot = true;
    
    //-----Debug 
    DebugManager debugmanager;

    //------Spawning
    private bool isReused = false;


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

        MinTurnAngle = Base_angle;
        MaxTurnAngle = Base_angle + 180;
        RCScanner_pos = (Vector2)turret.position + new Vector2(Missile_pos_dist * Mathf.Cos((Base_angle - 90) * Mathf.Deg2Rad), Missile_pos_dist * Mathf.Sin((Base_angle - 90) * Mathf.Deg2Rad));


        targetHeading = 0f;
        StartCoroutine(ScanArea());
        isReused = true;
    }

    void OnSpawned()
    {
        //Debug.Log("onspawn");
        if (isReused)
        { 
        StartCoroutine(ScanArea());
        ReadyToShoot = true;
        }
    }

    void Update()
    {

        if (TakeToAim)
        {
            if (turret.eulerAngles.z != targetHeading)
            {
                Quaternion q = Quaternion.AngleAxis(targetHeading, Vector3.forward); // то же что Vector3(0, 0, 1)
                turret.rotation = Quaternion.RotateTowards(turret.rotation, q, Time.deltaTime * TurnSpeed);
            }
          
            if (ReadyToShoot && ((turret.eulerAngles.z - targetHeading < 0.3 && turret.eulerAngles.z - targetHeading > -0.3) || (turret.eulerAngles.z - 360f - targetHeading < 0.3 && turret.eulerAngles.z - 360f - targetHeading > -0.3)))
            {
                Shoot();
                ReadyToShoot = false;
                TakeToAim = false;
                StartCoroutine(Recharging());
            }
            
        }
        else
        {
            //random move
        }
        
    }

    IEnumerator Recharging()
    {
        //Видимо здесь будет анимация перезарядки
        yield return new WaitForSeconds(RechargeTime);
        ReadyToShoot = true; 
    }

    
    
    IEnumerator ScanArea()
    {
        while (true)
        {
            Vector2 enemyposition = DetectEnemy(DetectRadius);

            if (enemyposition.x != 0) //По умолчанию всегда 0
                {
                    TakeToAim = true;
                    Vector3 vectorToTarget = (Vector3)enemyposition - (Vector3)turret.position;
                    targetHeading = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
                }
                else
                {
                    TakeToAim = false;
                }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    Vector2 DetectEnemy(int radius)
    {
        int layerMask = 1 << 9;
        hitColliders = Physics2D.OverlapCircleAll(turret.position, radius, layerMask);

        float distance = Mathf.Infinity;
        Vector2 position = turret.position;
        Vector2 enemypos = new Vector2(0, 0);


        //Находим ближайшего 
        foreach (Collider2D hitCollider in hitColliders)
        {
            float _angle = Mathf.Atan2(turret.position.y - hitCollider.attachedRigidbody.position.y, turret.position.x - hitCollider.attachedRigidbody.position.x);

            if (hitCollider.attachedRigidbody.tag == "Enemy" &&  _angle * Mathf.Rad2Deg >= MinTurnAngle && _angle * Mathf.Rad2Deg <= MaxTurnAngle) //Возможно нужно избавиться от этих углов
            {
                RaycastHit2D hit = Physics2D.Linecast(RCScanner_pos, hitCollider.attachedRigidbody.position,myLayerMask);
                //debugmanager.DrawDebugLine(RCScanner_pos, hit.point, Color.red);

                if (hit.rigidbody != null)
                {
                    if (hit.rigidbody.tag == "Enemy")
                    {
                        Vector3 diff = hitCollider.attachedRigidbody.position - position;
                        float curDistance = diff.sqrMagnitude;
                        if (curDistance < distance)
                        {
                            enemypos = hitCollider.attachedRigidbody.position;
                            distance = curDistance;
                        }
                    }
                }
            }
        }
        //------------------------------------------------------------------------------------
       //targetPos = enemypos;
        return enemypos;

    }

    protected virtual void Shoot()
    {
        //Debug.Log("Shoot base");
    }
    
}