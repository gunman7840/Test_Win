using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Smart_m : Enemy_infantry
{
    BoxCollider2D _box_col;
    float BodyRadius;
    public float DodgeSpeed;

    void Start()
    {
        //Health = 30;
        //LinerVel = 1;
        //JumpVel_min = 2f;
        //JumpVel_max = 10f;
        //JumpVel_med = 12f;
        //BalanceTorque = 10f;

        _box_col = GetComponent<BoxCollider2D>();
        BodyRadius = Mathf.Sqrt(Mathf.Abs(_box_col.size.x) + Mathf.Abs(_box_col.size.y))/2;

    }

    void OnSpawned()
    {
        Health = 30;
        this.enabled = true;
        Alive = true;
        NextTarget = navigation.GetStartPoint();
        Trajectory = navigation.GetPath(NextTarget);
        gameObject.tag = "Enemy";
    }

    new void Update ()
    {
        if (!isActive)
            return;
        if (Health <= 0)
        {
            StartCoroutine(Die());
        }
        //Debug.DrawLine(rb.position, NextTarget.position, Color.white);
        vel = rb.velocity;  // Нужна для корректного движения по плоскости

        if (JumpAngle != 0f)
        {
            //Приходится назначать скорость прямо отсюда. Если сделать это из метода Jump, то при следующем же апдейте скорость изменяется непредсказуемо
            rb.velocity = new Vector2(JumpV0 * Mathf.Cos(JumpAngle), JumpV0 * Mathf.Sin(JumpAngle));
            JumpAngle = 0f;
            JumpV0 = 0f;
        }



        /*
        //отрисовывает круг безопасности
        for(float i=0;i<=360;i+=45f)
        {
            //Debug.Log("---" + i);
            ///Debug.Log("Mathf.Cos(i)" + Mathf.Cos(i*Mathf.Deg2Rad));
            //Debug.Log("  Mathf.Sin(i)" + Mathf.Sin(i * Mathf.Deg2Rad));

            Vector2 point = (Vector2)_transform.position + new Vector2(BodyRadius* Mathf.Cos(i * Mathf.Deg2Rad), BodyRadius* Mathf.Sin(i * Mathf.Deg2Rad));
            Debug.DrawLine(_transform.position, point, Color.red);
        }
        */

        
        ProblemsDetector();
        Balance();
        Raycasting();
        TrajectoryDirecting();
        
    }


    public void Dodge(Vector2 threadPosition, float threadRotation)
    {
        Vector2 _direction = (Vector2)threadPosition-(Vector2)_transform.position ;
        float h = _direction.sqrMagnitude;  
        float angle=Vector2.Angle(new Vector2(1, 0), _direction);
        //корректируем угол чтобы привести его в соответсвие с ротейшном стрелы
        Vector2 diff = (Vector2)threadPosition - (Vector2)_transform.position;
        if (diff.y>0)
            angle = angle - 180;
        else if(diff.y<0)
            angle = 180- angle;

        float _Criticalangle = Mathf.Atan(BodyRadius / h) * Mathf.Rad2Deg; // +10; // угол в пределах которого находится тело 

        //debugmanager.DrawDebugRay(threadPosition,  angle, 3, Color.yellow);
        //debugmanager.DrawDebugRay(threadPosition,  angle+ _Criticalangle, 3, Color.red);
        //debugmanager.DrawDebugRay(threadPosition,  angle- _Criticalangle, 3, Color.red);

        if (threadRotation < angle + _Criticalangle && threadRotation > angle - _Criticalangle)
        {
            //Debug.Log("Detect");
            float DodgeAngle = 90f;
            if (threadRotation < -60 && threadRotation > -120)
            {
                DodgeAngle = (Global_direction == 1 || Global_direction == 2) ? 0 : 180;
            }
            else
            {
                DodgeAngle = 90;
            }

            //debugmanager.DrawDebugRay(_transform.position, DodgeAngle, 3, Color.yellow);
            rb.velocity = new Vector2(DodgeSpeed * Mathf.Cos(DodgeAngle*Mathf.Deg2Rad), DodgeSpeed * Mathf.Sin(DodgeAngle)) ;
            StartCoroutine(StopDodge());
            //Debug.Log("rb.velocity " + rb.velocity);
        }
        else
        {
            //Debug.Log("NO_Detect");
        }

    }

    protected IEnumerator StopDodge()
    {
        yield return new WaitForSeconds(0.3f);
        rb.velocity = new Vector2(0, 0);
        if (!isOnGround && NextTarget.edgeType == "hollow")
        {
            //Debug.Log("2 jump");
            bool res = CalculateAngle(NextTarget.position, Global_direction);
        }
        yield return null;
    }

}