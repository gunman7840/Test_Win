using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Path
{
    //Класс конкретной траектории

    public string PathName;
    public List<TargetPoint> PointsList;
    public int PathPriority;
    public int PathFails;
    public int PathWins;

    public Path(string _PathName, List<TargetPoint> _PointsList, int _PathPriority)
    {
        PathName = _PathName;
        PointsList = _PointsList;
        PathPriority = _PathPriority;
    }

    /*
    public void DrawPath()
    {
        //Отрисовка траектории

        Gizmos.color = Color.white;
        // for every point (except for the last one), draw line to the next point
        for (int i = 0; i < PointsList.Count - 1; i++)
        {
            Gizmos.DrawLine(new Vector2(PointsList[i].x, PointsList[i].y), new Vector2(PointsList[i + 1].x, PointsList[i + 1].y));
        }
    }
    */
}