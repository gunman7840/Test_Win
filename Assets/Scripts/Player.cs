using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Player : MonoBehaviour {

    static System.Random random = new System.Random();

    //----Состояние объекта  2
    public Transform g0, gLeft, gRight, bleft0, bleft1, bright0, bright1,leftCast,rightCast;
    private Rigidbody2D rb;
    private Vector2 vel;
    private bool isOnGround = false;
    private int isOnGroundJumpCounter = 0;
    private bool isOnMovingBody = false;
    private Vector2 MovingBodySpeed;
    private Vector2 RelativeVel;
    private bool afterstart = true;
    private float pos0; //Используем для случаев когда объект застрял 
    private float pos1;

    //Vector2 _myposition;

    //---------Прыжки
    private bool Jumpbool = false; //готовы ли мы прыгать
    Vector2 PointToJump=new Vector2(-3,-3);
    private float JumpAngle =0f;
    private float JumpV0=0f;

    //----Параметры  объекта
    private float gravity = 9.8f;
    private int LinerVel = 6;
    private int LinerForce = 20;
    private float JumpVel_min = 6f;
    private float JumpVel_max = 14f;

    //----Расчет траектории 
    public Navigation navigation;
    public Vector2 TargetPoint = new Vector2(0, 0);  
    public Path Trajectory;
    public Vector2 NextTarget;
    public float NextTarget_angle;
    public float RouteTimer = 0f;
    public float StuckTimer = 100f;

    //------События 
    public EventManager eventmanager;

    //-----Debug 
    DebugManager debugmanager; 

	void Start () 
    {    
        rb=GetComponent<Rigidbody2D>();
        debugmanager = GameObject.Find("DebugManager").GetComponent<DebugManager>();
        eventmanager = GameObject.Find("Main Camera").GetComponent<EventManager>();

        //Получаем первую траекторию
        navigation = GameObject.Find("Main Camera").GetComponent<Navigation>(); //Получаем доступ к классу
        NextTarget = navigation.GetStartPoint();
        Trajectory = navigation.GetPath(NextTarget);

        pos0 = transform.position.x;
        StartCoroutine (StuckCoroutine());
    }

	void Update () {
        //Debug.Log("Update ");
        //bool H_Button = false;
        //H_Button = (bool)(Input.GetKey("h"));
        //if (H_Button)
          //  H_ButtonMethod();




        Debug.DrawLine(transform.position, TargetPoint, Color.yellow);
        Debug.DrawLine(transform.position, NextTarget, Color.white);

        
        vel = rb.velocity;  // Нужна для корректного движения по плоскости
        
        if (JumpAngle != 0f)
        {
            //Приходится назначать скорость прямо отсюда. Если сделать это из метода Jump, то при следующем же апдейте скорость изменяется непредсказуемо
            rb.velocity = new Vector2(JumpV0 * Mathf.Cos(JumpAngle), JumpV0 * Mathf.Sin(JumpAngle));
            //Debug.Log("Jump from update " + rb.velocity);
            JumpAngle = 0f;
            JumpV0 = 0f;
        }

        ProblemsDetector();
        Balance();
        Raycasting();
        TrajectoryDirecting();
        

        //

        //Debug.Log("Stuck timer " + StuckTimer);
       
        //---------------------------------------------------Ручное управление
        
        int horizontal = 0;
        int vertical = 0;
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));


        if (horizontal == 1  && isOnGround == true)
        {
            MoveForward(1);
        }
        else if (horizontal == -1 && isOnGround == true)
        {
            MoveForward(-1);
        }   

        if (vertical == 1 && isOnGround==true)
        {
            Debug.Log("-----------------------------------------------------------------jump");           
            //rb.velocity = new Vector2(3.88f,9.21f);
            CalculateAngle(new Vector2(35.3f, 17f), 1);
           
        }
         
        //---------------------------------------------------Ручное управление
    }

    IEnumerator StuckCoroutine()
    {

        while (true)
        {
            if (!afterstart)
            {
                //Debug.Log("run cor");

                pos1 = transform.position.x;
                float dif = Mathf.Abs(pos1 - pos0);
                //Debug.Log("pos0 " + pos0 + " pos1 " + pos1 + " Diff " + dif);
                if (dif < 1)
                {
                    //Debug.Log("---------------------------------------------------STUCK-----------------------------------------------");
                    JumpAngle = random.Next(80, 100);
                    JumpV0 = 15f; //Иногда он так крепко застревает что прыжок не срабатывает, нужно учесть такие случае и применять прыжок в каждом апдейте пока он не выпрыгнет
                }
                pos0 = transform.position.x;
                yield return new WaitForSeconds(3f);
            }
            else
            {
                afterstart = false;
                yield return new WaitForSeconds(5f);
            }
        }    
    }

    void ProblemsDetector()
    {
        RouteTimer += 1 / 30f;  
        if (RouteTimer > 10)
        {
            //Debug.Log("PROBLEMS ---");
            Vector2 newpoint = navigation.GetTarget(transform.position, Trajectory, NextTarget);
            NextTarget = newpoint;
            Trajectory = navigation.GetPath(NextTarget);
            RouteTimer = 0f;
        }
            
    }

    void TrajectoryDirecting()
    {
        //Переключаемся на след целевую точку
        if (Mathf.Abs(transform.position.x - NextTarget.x) < 1 && Mathf.Abs(transform.position.y - NextTarget.y) < 1)
        {
            //rb.velocity = new Vector2(0, 0); // останавливаемся
            if (NextTarget == navigation.GetFinalTarget())
            {
                eventmanager.TargetReached(gameObject);
            }
            int i = 0;
            foreach (Vector2 item in Trajectory.PointsList)
            {
                if (item == NextTarget)
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
        NextTarget_angle = Mathf.Atan2(NextTarget.y - transform.position.y, NextTarget.x - transform.position.x) * Mathf.Rad2Deg;

        //Debug.Log(NextTarget_angle);

        if (NextTarget_angle > -30 && NextTarget_angle < 30)
        {
            MoveForward(1);
        }
        else if (NextTarget_angle > 30 && NextTarget_angle < 90)
        {
            MoveForward(2);
        }
        else if (NextTarget_angle > 90 && NextTarget_angle < 150)
        {
            MoveForward(-2);
        }
        else if (NextTarget_angle > 150 || NextTarget_angle < -150)
        {
            MoveForward(-1);
        }
        else if (NextTarget_angle > -150 && NextTarget_angle < -90)
        {
            MoveForward(-3);
        }
        else if (NextTarget_angle > -90 && NextTarget_angle < -30)
        {
            MoveForward(3);
        }

    }

    void Raycasting()
    {
        //Debug.Log("RayCasting");
        
        //Если мы только что прыгнули то считаем себя в воздухе даже не делая рейкастов      
        if (isOnGroundJumpCounter > 0)
        {
            isOnGroundJumpCounter -= 1;
            isOnGround = false;
            return;
        }
        
        RaycastHit2D hit01 =Physics2D.Linecast(g0.position,gRight.position);
        RaycastHit2D hit02 =Physics2D.Linecast(g0.position,gLeft.position);
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
   
    void MoveForward(int direction) 
    {

        //Debug.Log("MoveForward");
        switch (direction)
        {
            case 1:
            ScanLandscape(rightCast.position, 1);
            break;
            case 2:
            ScanAir(rightCast.position, 1);
            break;
            case -1:
            ScanLandscape(leftCast.position, -1);
            break;
            case -2:
            ScanAir(leftCast.position, -1);
            break;    

        }

        if (direction >= 2)
            direction = 1;
        else if (direction <= -2)
            direction = -1;

        if (isOnMovingBody) 
            RelativeVel = new Vector2(vel.x - MovingBodySpeed.x, vel.y - MovingBodySpeed.y); //Двмижение по телу которое тоже движется
        else
            RelativeVel = new Vector2(vel.x, vel.y);


        if (Mathf.Sqrt(RelativeVel.x * RelativeVel.x + RelativeVel.y * RelativeVel.y) < LinerVel)
        {
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
                rb.velocity = new Vector2(vel.x + 3 * direction, rb.velocity.y);  //если он движется в другую сторону со скоростью выше модуля, то соответственно не может остановиться
        }
          
    }

    void ScanAir(Vector2 scanpoint, int direction)
    {
        //Debug.Log("Scan Air");
        Vector2 PointToJump = new Vector2(-3, -3); //Структуру Vector2 необходимо инициализировать иначе будут ошибки компиляции

        RaycastHit2D hit1 = Physics2D.Raycast(scanpoint, new Vector2(direction, 1));
        RaycastHit2D hit2 = Physics2D.Raycast(scanpoint, new Vector2(5f * direction, 2));
        Debug.DrawLine(scanpoint, hit1.point, Color.cyan);
        Debug.DrawLine(scanpoint, hit2.point, Color.cyan);

        if (isOnGround == true && (Mathf.Abs(hit1.point.x - transform.position.x) < 5 || Mathf.Abs(hit2.point.x - transform.position.x) < 5))
        {
            //Debug.Log("Platform detected");

            RaycastHit2D hit_jump = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y+2), new Vector2(0, 1));
            if (hit_jump.point.y - transform.position.y < 4)  //Мы в туннеле
                return;

            RaycastHit2D hit_jump0 = Physics2D.Raycast(new Vector2((scanpoint.x + 0.2f * direction), transform.position.y+1), new Vector2(direction, 0));

            for (float i = 2f; i < 7; i = i + 1f)
            {
                RaycastHit2D hit_jump1 = Physics2D.Raycast(new Vector2((scanpoint.x + 0.2f * direction), transform.position.y + i), new Vector2(direction, 0));
                Debug.DrawLine(new Vector2(transform.position.x, hit_jump0.point.y), hit_jump0.point, Color.white);
                Debug.DrawLine(new Vector2(transform.position.x, hit_jump1.point.y), hit_jump1.point, Color.white);

                if (((hit_jump1.point.x - hit_jump0.point.x) * direction > 2) && Mathf.Abs(transform.position.x - hit_jump0.point.x) < 10) //Проверям что площадка достаточно широкая, что на нее можно запругнуть, проверям что HillEdge не слишком далеко
                {

                    Debug.DrawLine(new Vector2(transform.position.x, hit_jump1.point.y), hit_jump1.point, Color.red);
                    PointToJump = new Vector2(hit_jump0.point.x + 0.5f * direction, hit_jump1.point.y + 0.5f);
                    break;
                }
                hit_jump0 = hit_jump1;
            }
        }
        
        if (PointToJump.x != -3 && PointToJump.y != -3) //Можно прыгать
        {
            TargetPoint = PointToJump;
            CalculateAngle(PointToJump, direction);
        }


    }

    void ScanLandscape(Vector2 scanpoint, int direction)
    {
        //Debug.Log("ScanLandscape");
        Jumpbool = false;
        RaycastHit2D hit1 = Physics2D.Raycast(scanpoint, new Vector2(direction, -1));
        RaycastHit2D hit2 = Physics2D.Raycast(scanpoint, new Vector2(5f * direction, -2));
        RaycastHit2D hit3 = Physics2D.Raycast(scanpoint, new Vector2(5f * direction, -1));
        Debug.DrawLine(scanpoint, hit1.point, Color.cyan);
        Debug.DrawLine(scanpoint, hit2.point, Color.cyan);
        Debug.DrawLine(scanpoint, hit3.point, Color.cyan);
        //float tg = direction*  (hit2.point.y - hit1.point.y) / (hit2.point.x - hit1.point.x);
        float tg2 = direction * (hit3.point.y - hit2.point.y) / (hit3.point.x - hit2.point.x);

        //float angle = Mathf.Atan(tg);
        float angle2 = Mathf.Atan(tg2);
        //Debug.Log("angle2 " + angle2*Mathf.Rad2Deg);
        if ((hit3.point.x - hit2.point.x) * direction < 0) //Следовательно уклон грунта больше 90 градусов и нависает
        {
            angle2 = Mathf.PI + angle2;           
        }
        //---------------------------------------------------------------------------------------------------------------------
        //Vector2 PointToJump = new Vector2(-3, -3); //Структуру Vector2 необходимо инициализировать иначе будут ошибки компиляции

        if (isOnGround == true && (Mathf.Abs(transform.position.y - hit1.point.y) > 2 || Mathf.Abs(transform.position.y - hit2.point.y) > 2))  //Если есть подозрение на яму
        {
            if (preScanHollow(scanpoint, direction) == false) //Проверяем рельеф прямо перед собой
            {
                Jumpbool = ScanJump(scanpoint, direction);  //Сначала пытаемся прыгнуть ( на тот случай если сразу после впадины идет отвесный склон) и если не получается, тогда уже сканируем впадину и пытаемся спрыгнуть в нее
                if (Jumpbool)
                    return; //значит мы уже прыгаем
                Jumpbool = ScanHollow(scanpoint, direction);
                if (Jumpbool)
                    return; //значит мы уже прыгаем
            }
        }
        else if (isOnGround == true && (angle2 * Mathf.Rad2Deg > 50) && Mathf.Abs(transform.position.x - hit2.point.x)<2) //Если есть подозрение на возвышенность и она находится ближе 2 метров
        {
            Jumpbool = ScanJump(scanpoint, direction);
            if (Jumpbool)
                return; //значит мы уже прыгаем
        }

    }

    bool preScanHollow(Vector2 scanpoint, int direction)
    {
        //Если после приемлемой впадины сразу идет возвышенность то мы попадаем в тупик. Но и прыгать сразу же на нее нет смыслка потому что заранее неизвестно что это тупик. Будем решулировать через точки ориентации
        //Debug.Log("preScanHollow");  
        bool _freeway = false;
        RaycastHit2D hit_hollow = Physics2D.Raycast(scanpoint, new Vector2(direction, 0));
            //debugmanager.DrawDebugLine(scanpoint, hit_hollow.point, Color.white);
       
            RaycastHit2D hit_depth0 = Physics2D.Raycast(new Vector2(transform.position.x +2f* direction, scanpoint.y), new Vector2(0, -1));
            for (float i = 3f; i < 6f; i = i + 1f)
            {
                RaycastHit2D hit_depth1 = Physics2D.Raycast(new Vector2(transform.position.x + i * direction, scanpoint.y), new Vector2(0, -1));
                //debugmanager.DrawDebugLine(new Vector2(hit_depth0.point.x, scanpoint.y), hit_depth0.point, Color.white);
                //debugmanager.DrawDebugLine(new Vector2(hit_depth1.point.x, scanpoint.y), hit_depth1.point, Color.white);
                if ((transform.position.y - hit_depth1.point.y < 4) && (transform.position.y - hit_depth0.point.y < 4)) //Если в процессе сканирования видно, что яма не глубокая, то выходим из функции и продолжаем движение вперед
                {
                    //Debug.Log("landscape is acceptable");
                    _freeway = true;
                    return _freeway;
                }
            }
        return _freeway;
    }

    bool ScanHollow(Vector2 scanpoint, int direction)
    {
        //Debug.Log("ScanHollow");
        Vector2 HollowEdge=new Vector2(-3,-3);

            RaycastHit2D hit_hollow = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 2), new Vector2(direction, 0));
            //debugmanager.DrawDebugLine(new Vector2(transform.position.x, transform.position.y + 2), hit_hollow.point, Color.white); 
       
            RaycastHit2D hit_depth0 = Physics2D.Raycast(new Vector2(transform.position.x +3f* direction, transform.position.y + 2), new Vector2(0, -1));
            for (float i = 4.5f; i < Mathf.Abs(transform.position.x - hit_hollow.point.x); i = i + 1.5f)
            {
                RaycastHit2D hit_depth1 = Physics2D.Raycast(new Vector2(transform.position.x + i * direction, transform.position.y + 2), new Vector2(0, -1));
                //debugmanager.DrawDebugLine(new Vector2(hit_depth0.point.x, transform.position.y + 2), hit_depth0.point, Color.white);
                //debugmanager.DrawDebugLine(new Vector2(hit_depth1.point.x, transform.position.y + 2), hit_depth1.point, Color.white);
                if ((transform.position.y - hit_depth1.point.y < 4) && (hit_depth1.point.y - hit_depth0.point.y < 0.5)) //Если впадина не слишком глубока и есть ровный участок, то можно прыгать
                {
                   //Debug.DrawLine(new Vector2(hit_depth0.point.x, scanpoint.y), hit_depth0.point, Color.red);
                   //Debug.DrawLine(new Vector2(hit_depth1.point.x, scanpoint.y), hit_depth1.point, Color.red);
                    HollowEdge = new Vector2(hit_depth0.point.x + 0.5f * direction, hit_depth0.point.y + 0.5f);
                    break;
                }
                hit_depth0 = hit_depth1;
            }

        if (HollowEdge.x != -3) //Если мы нашли какую-то точку, то считаем угол
            {
                if (CalculateAngle(HollowEdge, direction)) //если CalculateAngle вернул true то прыжок возможен и будет выполнен в след апдейте, значения для прыжка обновлены. Выходим из всех функций через возврат true
                    return true;
                else
                    return false; //в противном случае прыжок невозможен, возвращаем false
            }
            else
                return false; //точка не найдена, возвращаем false
    }

    bool ScanJump(Vector2 scanpoint, int direction)  //не работает нихера
    {
        //Debug.Log("ScanJump");
        Vector2 HillEdge = new Vector2(-3, -3);
        RaycastHit2D hit_jump0 = Physics2D.Raycast(new Vector2(transform.position.x + direction, transform.position.y+1), new Vector2(direction, 0));
        for (float i = 2f; i < 10; i = i + 1f)
        {
            RaycastHit2D hit_jump1 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + i), new Vector2(direction, 0));

            debugmanager.DrawDebugLine(new Vector2(transform.position.x, hit_jump0.point.y), hit_jump0.point, Color.green);
            debugmanager.DrawDebugLine(new Vector2(transform.position.x, hit_jump1.point.y), hit_jump1.point, Color.white);

            if (((hit_jump1.point.x - hit_jump0.point.x) * direction > 1) && Mathf.Abs(transform.position.x - hit_jump0.point.x) < 10) //Проверям что площадка достаточно широкая, что на нее можно запругнуть, проверям что HillEdge не слишком далеко
            {
               debugmanager.DrawDebugLine(new Vector2(transform.position.x, hit_jump1.point.y + 0.5f), new Vector2(hit_jump0.point.x + 0.5f * direction, hit_jump1.point.y + 0.5f), Color.red);
                HillEdge = new Vector2(hit_jump0.point.x + 0.5f * direction, hit_jump1.point.y + 0.5f );
                break;
            }
            hit_jump0 = hit_jump1;
        }

        if (HillEdge.x != -3) //Если мы нашли какую-то точку, то считаем угол
        {
            if (CalculateAngle(HillEdge, direction)) //если CalculateAngle вернул true то прыжок возможен и будет выполнен в след апдейте, значения для прыжка обновлены. Выходим из всех функций через возврат true
                return true;
            else
                return false; //в противном случае прыжок невозможен, возвращаем false
        }
        else
            return false; //точка не найдена, возвращаем false
    }

    bool CalculateAngle(Vector2 point, int direction)
    {
        //Debug.Log("--------------------------------------------------------------------------CalculateAngle");
        Vector2 target = new Vector2(Mathf.Abs(transform.position.x - point.x), point.y); //Берем абсолютное значение по оси y, чтобы считать углы для точек которые находятся внизу
        float _angle=0f;
        float FinalJumpVel=0f;
        for (float V0 = JumpVel_min; V0 <= JumpVel_max; V0 = V0 + 2f) //Пытаемся расчитать угол для нескольких начальных скоростей, от минимальной к максимальной
        {
            float tg1 = (Mathf.Pow(V0, 2) + Mathf.Sqrt(Mathf.Pow(V0, 4) - gravity * (gravity * Mathf.Pow(target.x, 2) + 2 * Mathf.Pow(V0, 2) * (target.y - transform.position.y)))) / (gravity * target.x);
            //float tg2 = (Mathf.Pow(V0, 2) - Mathf.Sqrt(Mathf.Pow(V0, 4) - gravity * (gravity * Mathf.Pow(target.x, 2) + 2 * Mathf.Pow(V0, 2) * target.y - transform.position.y))) / (gravity * target.x);
            _angle= Mathf.Atan(tg1);
            if (_angle == _angle && _angle != 0)  //выбираем валидный угол с минимальной скоростью
            {
                //Debug.Log("Accepted Jump vel " + V0);
                FinalJumpVel = V0;
                break;
            }
        }
        
 
        if (_angle != _angle || _angle==0f)  //если угол не равен самому себе значит он неопределен (float.NaN), то точка недосягаема при исходной начальной скорости
        {
            Debug.Log("JUMP IMPOSSIBLE ");
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

    void Balance()
    {
        //Debug.Log(rb.rotation);
        if (Mathf.Abs(rb.rotation % 360) > 15)
        {
            rb.AddTorque(-20 * rb.rotation / Mathf.Abs(rb.rotation));
        }
    }

    
    
}
