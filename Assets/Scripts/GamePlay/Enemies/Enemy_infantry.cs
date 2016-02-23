using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Enemy_infantry : EnemyType
{

    private static System.Random random = new System.Random();

    //-----Конфигурация

    //public LayerMask myLayerMask ;
    protected float gravity = 9.8f;
    public float LinerVel = 0f;
    protected int LinerForce = 10;
    protected float JumpVel_min = 4f;
    protected float JumpVel_max = 10f;
    //protected float JumpVel_med = 6f;
    protected float DeadBodytime = 5f;
    public float BalanceTorque;

    //----Состояние объекта  
    public Transform g0, gLeft, gRight, bleft0, bleft1, bright0, bright1; //, leftCast, rightCast;
    protected Rigidbody2D rb;
    protected Vector2 vel;
    protected int Global_direction;
    protected bool isOnGround = false;
    protected int isOnGroundJumpCounter = 0;
    protected bool isOnMovingBody = false;
    protected Vector2 MovingBodySpeed;
    protected Vector2 RelativeVel;
    public bool isActive = true;
    public bool Alive=true;


    //---------Прыжки
    protected float JumpAngle = 0f;
    protected float JumpV0 = 0f;

    //----Расчет траектории 
    protected Vector2 TargetPoint = new Vector2(0, 0);
    protected float NextTarget_angle;
    protected float RouteTimer = 0f;
    protected float StuckTimer = 100f;

    //-------Обработка проблем при движении 
    protected float CriticalRouteTimer = 17f;
    protected int VelProblemCounter=0;
    protected int WarnVelProblem = 2;
    protected int CritVelProblem = 4;




    protected void Awake()
    {
        //Debug.Log("Enemy inf awake");
        EnemyType_Awake();

        this.enabled = true;
        Alive = true;

        rb = GetComponent<Rigidbody2D>();

    }

    protected void EnemyInfantryOnSpawned()
    {
        EnemyTypeOnSpawned();
        NextTarget = navigation.GetStartPoint(_transform.position,"inf");
        Trajectory = navigation.GetPath(NextTarget, "inf");
        StartCoroutine(StuckCoroutine()); //работает ужасно
    }
    
    protected void Update()
    {
        if (!isActive)
            return;
        //Debug.Log("Update ");
        //bool H_Button = false;
        //H_Button = (bool)(Input.GetKey("h"));
        //if (H_Button)
        //  H_ButtonMethod();
        if (Health<=0)
        {
            StartCoroutine(Die()); 
        }
        Debug.DrawLine(rb.position, NextTarget.position, Color.white);
        vel = rb.velocity;  // Нужна для корректного движения по плоскости
        
        if (JumpAngle != 0f)
        {
            //Приходится назначать скорость прямо отсюда. Если сделать это из метода Jump, то при следующем же апдейте скорость изменяется непредсказуемо
            rb.velocity = new Vector2(JumpV0 * Mathf.Cos(JumpAngle), JumpV0 * Mathf.Sin(JumpAngle));
            JumpAngle = 0f;
            JumpV0 = 0f;
        }


        ProblemsDetector();
        Balance();
        Raycasting();
        TrajectoryDirecting();


        /*
         //---------------------------------------------------Ручное управление
        int horizontal = 0;
        int vertical = 0;
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));


        if (horizontal == 1 && isOnGround == true)
        {
            MoveForward(1);
        }
        else if (horizontal == -1 && isOnGround == true)
        {
            MoveForward(-1);
        }

        if (vertical == 1 && isOnGround == true)
        {
            Debug.Log("-----------------------------------------------------------------jump");
            //rb.velocity = new Vector2(3.88f,9.21f);
            CalculateAngle(new Vector2(35.3f, 17f), 1);

        }
         //---------------------------------------------------Ручное управление
        */

    }

    protected void TrajectoryDirecting()
    {
        //Debug.Log("TrajectoryDirecting");
        //Переключаемся на след целевую точку
        if (Mathf.Abs(rb.position.x - NextTarget.position.x) < 0.5 && Mathf.Abs(rb.position.y - NextTarget.position.y) < 0.5)
        {

            rb.velocity = new Vector2(rb.velocity.x* 0.2f , rb.velocity.y); // останавливаемся
            if (NextTarget.isFinalTarget==true)
            {
                eventmanager.TargetReached(gameObject);
            }
           
            int i = 0;
            foreach (TargetPoint item in Trajectory.PointsList)
            {
                if (item.position == NextTarget.position)
                {
                    NextTarget = Trajectory.PointsList[i - 1];
                    RouteTimer = 0f;
                    break;
                }
                i++;
            }
        }

        //Если мы в воздухе, выходим из функции
        if (!isOnGround)
            return;

        //Принимаем решение куда двигаться в зависимости от того под каким углом находится след целевая точка 
        NextTarget_angle = Mathf.Atan2(NextTarget.position.y - rb.position.y, NextTarget.position.x - rb.position.x) * Mathf.Rad2Deg;

        

        if (NextTarget_angle > -90 && NextTarget_angle <= 45)
        {
            MoveForward(1);
        }
        else if (NextTarget_angle > 45 && NextTarget_angle <= 90)
        {
            MoveForward(2);
        }
        else if (NextTarget_angle > 90 && NextTarget_angle <= 135)
        {
            MoveForward(-2);
        }
        else if (NextTarget_angle > 135 || NextTarget_angle <= -90)
        {
            MoveForward(-1);
        }
       

    }

    //-------------------Управление
    protected void Raycasting()
    {
        //Debug.Log("RayCasting");

        //Если мы только что прыгнули то считаем себя в воздухе даже не делая рейкастов      
        if (isOnGroundJumpCounter > 0)
        {
            isOnGroundJumpCounter -= 1;
            isOnGround = false;
            return;
        }

        RaycastHit2D hit01 = Physics2D.Linecast(g0.position, gRight.position, myLayerMask);
        RaycastHit2D hit02 = Physics2D.Linecast(g0.position, gLeft.position, myLayerMask);
        if (hit01 || hit02)
        {
            isOnGround = true;

            // Смотрим не касаемся ли мы движущегося кинематик тела. Пока откладываем этот функционал
            if (hit01.rigidbody != null && hit02.rigidbody != null)  //Нужен этот if , потому что у земли не определяется rigidbody и сыпятся ошибки
            {
                if (hit01.rigidbody.tag == "MovingBody" || hit02.rigidbody.tag == "MovingBody")
                {
                    isOnMovingBody = true;
                    MovingBodySpeed = hit01.rigidbody.velocity;
                }
                else
                    isOnMovingBody = false;
            }

        }
        else
            isOnGround = false;

    }

    protected void MoveForward(int direction)
    {
        Global_direction = direction;
        //Debug.Log("MoveForward");
        if (direction == 1 && NextTarget.edgeType == "hollow" )  //Прыгаем по параболе если: перед нами пропасть и точка приземления невысоко
            ScanLandscape( 1);
        else if (direction == -1 && NextTarget.edgeType == "hollow")
            ScanLandscape( -1);
        else if (direction == 2 && NextTarget.edgeType == "hollow" ) //добавил сюда hollow на случай если мы провалились в яму и пытаемся выбраться
           Jump(1);
        else if (direction == -2 && NextTarget.edgeType == "hollow" )
           Jump(-1);

        /*
        if (isOnMovingBody)
            RelativeVel = new Vector2(vel.x - MovingBodySpeed.x, vel.y - MovingBodySpeed.y); //Двмижение по телу которое тоже движется
        else
        */
        RelativeVel = new Vector2(vel.x, vel.y);

        //rb.velocity = new Vector2(Mathf.Lerp(0f, LinerVel * (direction / Mathf.Abs(direction)), LinerVel - vel.x), rb.velocity.y); //работает рывками

        
        if (Mathf.Sqrt(RelativeVel.x * RelativeVel.x + RelativeVel.y * RelativeVel.y) < LinerVel || (RelativeVel.x/ Mathf.Abs(RelativeVel.x) != direction / Mathf.Abs(direction)))
        {
            /*
            //Эта секция отвечает за движение по кинематическим телам

            RaycastHit2D hit01 = Physics2D.Linecast(bleft0.position, bleft1.position);
            RaycastHit2D hit02 = Physics2D.Linecast(bright0.position, bright1.position);
            if (hit01.rigidbody != null && hit02.rigidbody != null)  //Нужен этот if , потому что у земли не определяется rigidbody и сыпятся ошибки
            {
                if ((hit01.rigidbody.isKinematic == false && direction == 1) || (hit02.rigidbody.isKinematic == false && direction == -1))
                {
                    //Debug.Log("CONTACT");
                    rb.AddForce(new Vector2(LinerForce * direction, 0));
                }
            }
            else
            */
            rb.velocity = new Vector2(vel.x + 3 * direction, rb.velocity.y);  //если он движется в другую сторону со скоростью выше модуля, то соответственно не может остановиться
       }
        
    }

    protected void Jump(int direction)
    {
        
        if (CalculateAngle(NextTarget.position, direction))
        {
            return;
        }
    }

    protected void ScanLandscape(int direction)
    {
        //Debug.Log("ScanLandscape");
        RaycastHit2D hit1 = Physics2D.Raycast(_transform.position, new Vector2(direction, -1),100, myLayerMask);
        RaycastHit2D hit2 = Physics2D.Raycast(_transform.position, new Vector2(5f * direction, -2), 100, myLayerMask);
        RaycastHit2D hit3 = Physics2D.Raycast(_transform.position, new Vector2(5f * direction, -1), 100, myLayerMask);
        Debug.DrawLine(_transform.position, hit1.point, Color.cyan);
        Debug.DrawLine(_transform.position, hit2.point, Color.cyan);
        Debug.DrawLine(_transform.position, hit3.point, Color.cyan);
        //float tg = direction*  (hit2.point.y - hit1.point.y) / (hit2.point.x - hit1.point.x);
        float tg2 = direction * (hit3.point.y - hit2.point.y) / (hit3.point.x - hit2.point.x);
        //float angle = Mathf.Atan(tg);
        float angle2 = Mathf.Atan(tg2);
        if ((hit3.point.x - hit2.point.x) * direction < 0) //Следовательно уклон грунта больше 90 градусов и нависает
        {
            angle2 = Mathf.PI + angle2;
        }
        if (isOnGround == true && (Mathf.Abs(rb.position.y - hit1.point.y) > 2 || Mathf.Abs(rb.position.y - hit2.point.y) > 2))  //Если есть подозрение на яму
        {
            if(CalculateAngle(NextTarget.position, direction))
            {
                return;
            }
            else
            {
                Debug.Log("Cant jump");
            }  
        }
    }

    protected bool CalculateAngle(Vector2 point, int direction)
    {
        //Debug.Log("--------------------------------------------------------------------------CalculateAngle");
        Vector2 target = new Vector2(Mathf.Abs(rb.position.x - point.x), point.y); //Берем абсолютное значение по оси y, чтобы считать углы для точек которые находятся внизу
        float _angle = 0f;
        float FinalJumpVel = 0f;
        for (float V0 = JumpVel_min; V0 <= JumpVel_max; V0 = V0 + 0.5f) //Пытаемся расчитать угол для нескольких начальных скоростей, от минимальной к максимальной
        {
            float tg1 = (Mathf.Pow(V0, 2) + Mathf.Sqrt(Mathf.Pow(V0, 4) - gravity * (gravity * Mathf.Pow(target.x, 2) + 2 * Mathf.Pow(V0, 2) * (target.y - rb.position.y)))) / (gravity * target.x);
            //float tg2 = (Mathf.Pow(V0, 2) - Mathf.Sqrt(Mathf.Pow(V0, 4) - gravity * (gravity * Mathf.Pow(target.x, 2) + 2 * Mathf.Pow(V0, 2) * target.y - rb.position.y))) / (gravity * target.x);
            _angle = Mathf.Atan(tg1);
            if (_angle == _angle && _angle != 0)  //выбираем валидный угол с минимальной скоростью
            {
                //Debug.Log("Accepted Jump vel " + V0);
                FinalJumpVel = V0;
                break;
            }
        }


        if (_angle != _angle || _angle == 0f)  //если угол не равен самому себе значит он неопределен (float.NaN), то точка недосягаема при исходной начальной скорости
        {
            //Debug.Log("JUMP IMPOSSIBLE ");
            return false;
        }
        if (direction == -1)
        {
            _angle = Mathf.PI - _angle;
        }

        TargetPoint = point; //рисуем линию до этой точки в методе scnlandscape
        //------------------------------debug
        //debugmanager.DrawParabola(transform.position, FinalJumpVel, _angle);      
        //------------------------------

        isOnGroundJumpCounter = 20;
        rb.velocity = new Vector2(0, 0); //Останавливаем тело
        rb.angularVelocity = 0;
        JumpAngle = _angle;
        JumpV0 = FinalJumpVel;

        return true;
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
            NextTarget = navigation.GetTarget(transform.position, NextTarget, "inf");
            Trajectory = navigation.GetPath(NextTarget,"inf");
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
                VelProblemCounter+=1;

                if(VelProblemCounter>=WarnVelProblem)
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

    protected IEnumerator Die()
    {
        while (true)
        {
            if (Alive)
            {
                this.enabled = false;
                Alive = false;
                gameObject.tag = "Dead";
                yield return new WaitForSeconds(DeadBodytime);
            }
            else
            {
                enemymanager.DestroyEnemy(_transform);
                yield return null;
            }
        }
    }
    
    //-------------------------------------------------------Weapons affect
    public void ApplyDamage(int points)
    {
        //Debug.Log("ApplyDamage " + points);
        Health -= points;
    }

    public void SlowDown_message(int slow_effect_length)
    {

        //float RecoverLinerVel = LinerVel;
        //LinerVel = 0.0f;
        this.enabled = false;
        StartCoroutine(SlowDown(slow_effect_length));
    }

    protected IEnumerator SlowDown(int slow_effect_length)
    {
        while (true)
        {
            yield return new WaitForSeconds(slow_effect_length);
            //LinerVel = _vel;
            this.enabled = true;
            yield return null;   
        }
    }

    

    //-------------------------------------------------------------------------
}
