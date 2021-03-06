﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AirEnemy : EnemyType
{

    private static System.Random random = new System.Random();

    //-----Конфигурация

    //public LayerMask myLayerMask;
    public float LinerVel;
    protected float BalanceTorque = 5f;
    //protected EnemyManager enemymanager;

    //----Состояние объекта  
    protected Rigidbody2D rb;
    //protected Vector2 vel;
    //protected int Global_direction;
    public bool isActive = true;
    public bool UnderAttack=false;
    //----Расчет траектории 
    protected Vector2 TargetPoint = new Vector2(0, 0);
    protected float RouteTimer = 0f;
    protected float StuckTimer = 100f;

    //-------Обработка проблем при движении 
    protected float CriticalRouteTimer = 17f;
    protected int VelProblemCounter = 0;
    protected int WarnVelProblem = 2;
    protected int CritVelProblem = 4;

    protected void Awake()
    {
        //Debug.Log("AirEnemy awake");
        EnemyType_Awake();
        this.enabled = true;
        rb = GetComponent<Rigidbody2D>();
        _transform = transform;
        //StartCoroutine(StuckCoroutine()); //работает ужасно
    }

    protected void AirEnemyOnSpawned()
    {
        EnemyTypeOnSpawned();
        NextTarget = navigation.GetStartPoint(_transform.position, "air");
        Trajectory = navigation.GetPath(NextTarget, "air");
    }
    protected void Update()
    {
        if (!isActive)
            return;
        
       // Debug.Log("Update " + _transform.position);
        //bool H_Button = false;
        //H_Button = (bool)(Input.GetKey("h"));
        //if (H_Button)
        //  H_ButtonMethod();
        //Debug.Log("health" + Health);
        if (Health <= 0)
        {
            StartCoroutine(Die());
        }
        vel = rb.velocity;  // Нужна для корректного движения по плоскости

        //ProblemsDetector();
        Balance();
        TrajectoryDirecting();
    }

    protected void TrajectoryDirecting()
    {
        //Переключаемся на след целевую точку
        if (Mathf.Abs(rb.position.x - NextTarget.position.x) < 0.5 && Mathf.Abs(rb.position.y - NextTarget.position.y) < 0.5)
        {
            if (NextTarget.isFinalTarget == true)
            {
                Debug.Log("reached ");
                eventmanager.CallOnTargetReached(_transform);
                return;
            }
            int i = 0;
            foreach (TargetPoint item in Trajectory.PointsList)
            {
                if (item.position == NextTarget.position)
                {
                    NextTarget = Trajectory.PointsList[i - 1];
                    RouteTimer = 0f;
                    //rb.velocity = ((Vector2)NextTarget.position - (Vector2)_transform.position).normalized * LinerVel; // сразу изменяем вектор скорости по направлению к след точке, неудобно тем что при попаданиях его носит по карте как лист на ветру
                    break;
                }
                i++;
            }
        }
        if (UnderAttack)
            return;

        MoveForward();
    }

    //-------------------Управление

    protected void MoveForward()
    {

        //Debug.Log("MoveForward rb.velocity " + rb.velocity);
        Debug.DrawLine(rb.position, NextTarget.position, Color.white);

        //if (vel.sqrMagnitude < LinerVel)
        //{
            Vector2 heading = ((Vector2)NextTarget.position - (Vector2)_transform.position).normalized;
            //rb.velocity = rb.velocity + heading * LinerVel; 
            rb.velocity = heading * LinerVel;
            
            Debug.DrawLine(_transform.position, (Vector2)_transform.position + (Vector2) heading * LinerVel, Color.green);
            //rb.AddForce(heading * LinerVel);
            //Vector2.MoveTowards(_transform.position, TargetPoint, 2f);
            //float step = 2 * Time.deltaTime;
            //_transform.position = Vector3.MoveTowards(_transform.position, NextTarget.position, step);
        //}

    }

    //-----------------------------------------------------------
    public void SettoSleep()
    {
        isActive = false;
    }

    public void SettoAwake()
    {
        isActive = true;
    }

    protected void ProblemsDetector()
    {
        // Debug.Log("VelProblemTimer " + VelProblemCounter);
        //Функция следит сколько времени прошло с достижения последней контрольной точки, и если много , то ищет новую контролькую точку
        RouteTimer += 1 / 30f;
        if (RouteTimer > CriticalRouteTimer)
        {
            //Debug.Log("PROBLEMS ---");
            TargetPoint newpoint = navigation.GetTarget(transform.position, NextTarget, "air");
            NextTarget = newpoint;
            Trajectory = navigation.GetPath(NextTarget, "air");
            RouteTimer = 0f;
        }
    }

    protected IEnumerator StuckCoroutine()
    {
        //Функция записывает координаты объекта через интервалы времени и если координаты не меняются , то происходит прыжок вверх под случайным углом
        while (true)
        {
            if (Mathf.Abs(vel.x) < 1 && Mathf.Abs(vel.y) < 1)
            {
                VelProblemCounter += 1;

                if (VelProblemCounter >= WarnVelProblem)
                {
                    if (VelProblemCounter >= CritVelProblem)
                    {
                        Debug.Log("--------------------------------------------------destroy----");
                        Destroy(gameObject);
                    }
                    Debug.Log("---------------------------------------------------STUCK-----------------------------------------------");
                    //JumpAngle = random.Next(80, 100);
                    //JumpV0 = 4f; 
                }

            }
            else
            {
                VelProblemCounter = 0;
            }
            yield return new WaitForSeconds(2f);
        }
    }

    protected void Balance()
    {
        //Debug.Log(rb.rotation);
        if (Mathf.Abs(rb.rotation % 360) > 15)
        {
            rb.AddTorque(-BalanceTorque * rb.rotation / Mathf.Abs(rb.rotation));
        }
    }

    new protected IEnumerator Die()
    {
        //Debug.Log("start die");
        eventmanager.CallOnDestroyEnemy_cost(cost);
        this.enabled = false;
        SettoSleep();
        gameObject.tag = "Dead";
        rb.gravityScale = 1f;
         
        yield return new WaitForSeconds(4f);
        eventmanager.CallOnDestroyEnemy(_transform);
    }

    //-------------------------------------------------------Weapons affect
    new public void ApplyDamage(int points)
    {
        Debug.Log("ApplyDamage " + points);
        Health -= points;
        //if (Health>0)
        UnderAttack = true;
        StartCoroutine(SlowDown(0.5f));

    }

    public void SlowDown_message(float slow_effect_length)
    {
        //float RecoverLinerVel = LinerVel;
        //LinerVel = 0.0f;
        this.enabled = false;
        rb.gravityScale = 1;

        if (Health>0)
        StartCoroutine(SlowDown(slow_effect_length));
    }

    protected IEnumerator SlowDown(float slow_effect_length)
    {
        yield return new WaitForSeconds(slow_effect_length);
        UnderAttack = false;
        this.enabled = true;

        yield return null;
    }



    //-------------------------------------------------------------------------
}
