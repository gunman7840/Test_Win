﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


abstract class Missile_GunType : MonoBehaviour
{
    Collider2D[] hitColliders;
    public GameObject missilePrefab;
    public GameObject TurretPrefab;
    public GameObject Turret_base_Prefab;
    public float Base_angle;
    public float ShootFrequency;
    public LayerMask myLayerMask;

    protected GameObject missile;
    protected GameObject turret;
    protected GameObject turret_base;
    protected Transform c_transform;

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
    protected float RechargeTime = 1.5f;
    protected int DetectRadius = 10;
    protected bool TakeToAim =false;
    protected bool ReadyToShoot = true;
    
    //-----Debug 
    DebugManager debugmanager;


    void Start()
    {
        
        debugmanager = GameObject.Find("DebugManager").GetComponent<DebugManager>();
        turret = (GameObject)Instantiate(TurretPrefab, transform.position, Quaternion.AngleAxis(Base_angle - 90, new Vector3(0, 0, 1))); // -90 чтобы ствол всегда смотерл в направлении противоположном крпелению базы
        c_transform=turret.transform;
        turret_base = (GameObject)Instantiate(Turret_base_Prefab, c_transform.position, Quaternion.AngleAxis(Base_angle, new Vector3(0, 0, 1)));  // 0 0 1 это ось по которой идет вращение , то есть z

        MinTurnAngle = Base_angle;
        MaxTurnAngle = Base_angle + 180;
        RCScanner_pos = (Vector2)c_transform.position + new Vector2(Missile_pos_dist * Mathf.Cos((Base_angle - 90) * Mathf.Deg2Rad), Missile_pos_dist * Mathf.Sin((Base_angle - 90) * Mathf.Deg2Rad));


        targetHeading = 0f;
        StartCoroutine(ScanArea());
       
    }

    void Update()
    {
        //Debug.Log("-------------------------------------------Update ");

        if (TakeToAim)
        {
            if (c_transform.eulerAngles.z != targetHeading)
            {
                //Debug.DrawLine(RCScanner_pos, );
                Quaternion q = Quaternion.AngleAxis(targetHeading, Vector3.forward); // то же что Vector3(0, 0, 1)
                c_transform.rotation = Quaternion.RotateTowards(c_transform.rotation, q, Time.deltaTime * TurnSpeed);
            }
            if (ReadyToShoot && ((c_transform.eulerAngles.z - targetHeading < 0.3 && c_transform.eulerAngles.z - targetHeading > -0.3) || (c_transform.eulerAngles.z - 360f - targetHeading < 0.3 && c_transform.eulerAngles.z - 360f - targetHeading > -0.3)))
            {
                //Debug.Log("------------------------------------------------------------------------Shoot---------------------------");
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
            //Debug.Log("-------------------------------------------ScanArea ");
            Vector2 enemyposition = DetectEnemy(DetectRadius);
                if (enemyposition.x != 0) //По умолчанию всегда 0
                {
                    TakeToAim = true;
                    Vector3 vectorToTarget = (Vector3)enemyposition - (Vector3)c_transform.position;
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
        //Debug.Log("-------------------------------------------DetectEnemy ");
        int layerMask = 1 << 9;
        hitColliders = Physics2D.OverlapCircleAll(c_transform.position, radius, layerMask);
        //List<Collider2D> hitCollidersList = new List<Collider2D>(hitColliders);

        float distance = Mathf.Infinity;
        Vector2 position = c_transform.position;
        Vector2 enemypos = new Vector2(0, 0);


        //Находим ближайшего 
        foreach (Collider2D hitCollider in hitColliders)
        {
            float _angle = Mathf.Atan2(c_transform.position.y - hitCollider.attachedRigidbody.position.y, c_transform.position.x - hitCollider.attachedRigidbody.position.x);

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