
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WeaponRadius : MonoBehaviour
{
    private Camera _camera;
    private UIManager uimanager;
    private GameObject gm;
    private Transform tr;
    private int radius=5;

   // private int angle = 0;

    void Awake()
    {
        gm = gameObject;
        tr = transform;
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        uimanager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    void Update()
    {
        //Debug.Log("weaponradius");
        //Нужно сделать какую-то графику 
        /*
        if (radius!=0)
        {

            if (angle > 360)
                angle = 0;

            Debug.DrawRay(tr.position, Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right, Color.red, radius);
            angle+=40;
        }
        */
    }

    void Appear(int rad)
    {

    }

    void Disappear()
    {

    }
}