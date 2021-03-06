﻿using UnityEngine;
using System.Collections;

public class DebugManager : MonoBehaviour {

    public GameObject delayline;
    public GameObject whitesquare;
    private float gravity = 9.8f;


   
    public void DrawDebugLine(Vector2 x1, Vector2 x2, Color _color)
    {
      ((GameObject)Instantiate(delayline, new Vector2(-1, -1), Quaternion.identity)).GetComponent<DrawDelayLine>().Initialize(x1, x2, _color);

    }

    public void DrawParabola(Vector2 startposition, float StartV, float angle)
    {
        //Рисуем предполагаемую траекторию прыжка 
        float x =0f;
        float y =0f;
        for (float t = 0; t < 2; t=t+0.1f)
        {
            x = startposition.x + (StartV * t * Mathf.Cos(angle));
            y = startposition.y + (StartV * t * Mathf.Sin(angle) - (gravity * t * t) / 2);

            Instantiate(whitesquare, new Vector2(x, y), Quaternion.identity);
        }
    }

    public void DrawDebugRay(Vector2 x1, float angle,int length, Color _color)
    {

        Vector2 x2 = new Vector2(length * Mathf.Cos(angle*Mathf.Deg2Rad) + x1.x, length * Mathf.Sin(angle*Mathf.Deg2Rad) + x1.y);

        ((GameObject)Instantiate(delayline, new Vector2(-1, -1), Quaternion.identity)).GetComponent<DrawDelayLine>().Initialize(x1, x2, _color);

    }

}
