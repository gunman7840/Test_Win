using UnityEngine;
using System.Collections;

public class TargetPoint
{
    public Vector2 position;
    public float distance;
    public string edgeType;
    public bool isFinalTarget;
    /*
    public TargetPoint(Vector2 _position)
    {
        position = _position;
    }
    */

    public TargetPoint(GameObject pointGO)
    {
        position = pointGO.transform.position;

        if (pointGO.tag == "point_land" || pointGO.tag == "point_air")
            edgeType = "land"; // точки траектории по воздуху ставим в ленд, там все равно не используется edgeType
        else if (pointGO.tag == "point_hollow")
            edgeType = "hollow";
        else
        {
            edgeType = "land";
            isFinalTarget = true;
        }
    }
}