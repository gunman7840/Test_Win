using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class GraviGun : Missile_GunType
{
    private bool ActiveState=false;
    private BoxCollider2D GravityArea;
    //private Transform GravityPoint;
    GraviSpaceTurret _graviSpaceTurret;

    public int GravityRadius;
    Collider2D[] AffectedEnemies;
    public int ActiveStateTime;


    void Awake()
    {
        MissileSpeed = 15;
        Missile_pos_dist = 2;
        DetectRadius = 6;
        turret_pr_name = "GrviGun_turret";
        turret_base_pr_name = "Turret_base_prefab";

        Missile_pos_dist = 3;
    }

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
                GravityArea=turret.GetComponent<BoxCollider2D>();
                _graviSpaceTurret= turret.GetComponent<GraviSpaceTurret>();
                GravityArea.enabled = false;

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
        MaxTurnAngle = Base_angle;
        RCScanner_pos = (Vector2)turret.position + new Vector2(Missile_pos_dist * Mathf.Cos((Base_angle - 90) * Mathf.Deg2Rad), Missile_pos_dist * Mathf.Sin((Base_angle - 90) * Mathf.Deg2Rad));


        targetHeading = 0f;
        StartCoroutine(ScanArea());
        isReused = true;
    }

    void Update()
    {
        //Debug.Log("ActiveState " + ActiveState);

        if (TakeToAim && !ActiveState)
        {
            GravityArea.enabled = false;
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
                
            }

        }
        else if (ActiveState)
        {
            GravityArea.enabled = true;

            //StartCoroutine(ScanGravityArea());
        }
        else
        {
            //random move
        }

    }


    protected override void Shoot()
    {
        //Debug.Log("Shoot derived");
        _graviSpaceTurret.OnActivate();
        ActiveState = true;
        StartCoroutine(StopShoot());
        
    }

    IEnumerator StopShoot()
    {
        //Видимо здесь будет анимация перезарядки
        //Debug.Log("StopShoot");
        yield return new WaitForSeconds(ActiveStateTime);
        //_graviSpaceTurret
        ActiveState = false;
        _graviSpaceTurret.OnDeActivate();
        StartCoroutine(Recharging());
    }

}