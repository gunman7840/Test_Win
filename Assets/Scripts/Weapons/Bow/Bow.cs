using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class Bow : MonoBehaviour
{
    Collider2D[] hitColliders;
    public GameObject arrowPrefab;
    public GameObject TurretPrefab;
    public GameObject Turret_base_Prefab;
    public float Base_angle;
    
    private GameObject arrow;
    private GameObject turret;
    private GameObject turret_base;

    //------- Поворот
    private float MinTurnAngle;
    private float MaxTurnAngle;

    private float targetHeading;
    private Vector2 targetPos;
    private bool LockTarget;
    private float TurnSpeed = 300;
    //private Vector2 TestTarget;

    //--------Стрельба
    private float Missile_pos_dist=3;
    private Vector2 Missile_pos;
    private Vector2 RCScanner_pos;
    float MissileSpeed = 40;

    //--------Debug
    DebugManager debugmanager;
    //------------------------------------------------------------------------------------------------------
    void Start()
    {
        debugmanager = GameObject.Find("DebugManager").GetComponent<DebugManager>();

        turret = (GameObject)Instantiate(TurretPrefab, transform.position, Quaternion.AngleAxis(Base_angle-90, new Vector3(0, 0, 1))); // -90 чтобы ствол всегда смотерл в направлении противоположном крпелению базы
        turret_base = (GameObject)Instantiate(Turret_base_Prefab, transform.position, Quaternion.AngleAxis(Base_angle,new Vector3(0,0,1)));  // 0 0 1 это ось по которой идет вращение , то есть z

        MinTurnAngle = Base_angle  ;
        MaxTurnAngle = Base_angle + 180;
        RCScanner_pos = (Vector2)transform.position + new Vector2(Missile_pos_dist * Mathf.Cos((Base_angle  - 90) * Mathf.Deg2Rad), Missile_pos_dist * Mathf.Sin((Base_angle  - 90) * Mathf.Deg2Rad));


        targetHeading = 0f;
        LockTarget = false;

        StartCoroutine(CustomUpdate());

    }
    void Update()
    {
        Debug.DrawLine(transform.position, (Vector2)transform.position+ new Vector2(10 * Mathf.Cos(MinTurnAngle * Mathf.Deg2Rad), 10 * Mathf.Sin(MinTurnAngle * Mathf.Deg2Rad)), Color.cyan);
        Debug.DrawLine(transform.position, (Vector2)transform.position +  new Vector2(10 * Mathf.Cos(MaxTurnAngle * Mathf.Deg2Rad), 10 * Mathf.Sin(MaxTurnAngle * Mathf.Deg2Rad)), Color.cyan);

        Debug.DrawLine(transform.position, RCScanner_pos, Color.red);


        //Debug.Log("LockTarget " + LockTarget);
        //Debug.Log("targetHeading " + targetHeading);
        //Debug.Log("turret " + turret.transform.eulerAngles.z);
        //Debug.Log("turret -360f " + (turret.transform.eulerAngles.z - 360f));

        if (LockTarget && turret.transform.eulerAngles.z != targetHeading)
        {
            //Debug.Log("------------------------------------moving....");
            //Debug.DrawLine(turret.transform.position, targetPos, Color.red);

            Quaternion q = Quaternion.AngleAxis(targetHeading, Vector3.forward);
            turret.transform.rotation = Quaternion.RotateTowards(turret.transform.rotation, q, Time.deltaTime * TurnSpeed);
        }
        if(LockTarget && ((turret.transform.eulerAngles.z - targetHeading < 0.5 && turret.transform.eulerAngles.z - targetHeading >- 0.5) || (turret.transform.eulerAngles.z -360f - targetHeading < 0.5 && turret.transform.eulerAngles.z - 360f - targetHeading > -0.5)))  
        {
            //Debug.Log("------------------------------------------------------------------------Shoot---------------------------");
            Shoot();
            LockTarget = false;
        }
    }

    void Shoot()
    {
        Quaternion q = Quaternion.AngleAxis(targetHeading, Vector3.forward);
        Missile_pos = (Vector2)transform.position + new Vector2(Missile_pos_dist * Mathf.Cos(targetHeading * Mathf.Deg2Rad), Missile_pos_dist* Mathf.Sin(targetHeading * Mathf.Deg2Rad));
        GameObject arrow = (GameObject)Instantiate(arrowPrefab, Missile_pos, Quaternion.Slerp(transform.rotation, q, 1f));
        Rigidbody2D ArrowRB = arrow.GetComponent<Rigidbody2D>();
        ArrowRB.velocity = new Vector2(MissileSpeed * Mathf.Cos(targetHeading * Mathf.Deg2Rad), MissileSpeed * Mathf.Sin(targetHeading * Mathf.Deg2Rad)); ;
    }

    IEnumerator CustomUpdate()
    {
        while (true)
        {
            if (!LockTarget)
            {
                Vector2 enemyposition = DetectEnemy(28);
                if (enemyposition.x != 0) //По умолчанию
                {
                    LockTarget = true;
                    Vector3 vectorToTarget = (Vector3)enemyposition - (Vector3)transform.position;
                    targetHeading = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
                }
                
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    Vector2 DetectEnemy (int radius)
    {
        //нужно ограничить угол и построить рейкасты до цели 
        Debug.Log("-------------------------------------------DetectEnemy ");
        int layerMask = 1 << 8;
        hitColliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        //List<Collider2D> hitCollidersList = new List<Collider2D>(hitColliders);

        float distance = Mathf.Infinity;
        Vector2 position = transform.position;
        Vector2 enemypos = new Vector2(0,0);


        //Находим ближайшего 
        foreach (Collider2D hitCollider in hitColliders)
        {
            float _angle = Mathf.Atan2(transform.position.y- hitCollider.attachedRigidbody.position.y, transform.position.x-hitCollider.attachedRigidbody.position.x);

            if (hitCollider.attachedRigidbody.tag == "Enemy"  && _angle * Mathf.Rad2Deg >= MinTurnAngle && _angle * Mathf.Rad2Deg <= MaxTurnAngle) 
            {
                RaycastHit2D hit = Physics2D.Linecast(RCScanner_pos, hitCollider.attachedRigidbody.position);
                //debugmanager.DrawDebugLine(RCScanner_pos, hit.point, Color.green);
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
        //------------------------------------------------------------------------------------
        targetPos = enemypos;
        return enemypos;

    }

   

    

}

