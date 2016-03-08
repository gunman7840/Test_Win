using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Level3_1 : MonoBehaviour, ILevel
{
    //это уровень 1 на самом деле
    public GameObject[] path1_points;
    //public GameObject[] air_path1_points;

    protected Path path1;
    //protected Path air_path1;

    //--здесь ничего менять не нужно
    protected List<Path> _pathlist = new List<Path>(); //Лист содержащий все пути
    protected List<Path> _pathlist_air = new List<Path>(); //Лист содержащий все пути по воздуху
    protected List<TargetPoint> _tp_Array = new List<TargetPoint>(); //Лист содержащий все точки всех траекторий
    protected List<TargetPoint> _tp_Array_air = new List<TargetPoint>(); //Лист содержащий все точки всех траекторий по воздуху

    void Awake()
    {
        //Debug.Log("level awake");
        // awake чтобы инициализировать траектории были готовы к моменту старта Player

        List<TargetPoint> _pointslist_1 = new List<TargetPoint>();
        FillPointslist(path1_points, _pointslist_1);
        path1 = new Path("Path1", _pointslist_1, 10);

        /*
        List<TargetPoint> air_pointslist_1 = new List<TargetPoint>();
        FillPointslist(air_path1_points, air_pointslist_1);
        air_path1 = new Path("AirPath1", air_pointslist_1, 10);
        */


        _pathlist.Add(path1);

       // _pathlist_air.Add(air_path1);


        //менять не нужно
        foreach (Path _path in _pathlist)
        {
            foreach (TargetPoint _point in _path.PointsList)
            {
                if (_tp_Array.Contains(_point) == false)
                {
                    _tp_Array.Add(_point);
                }
            }
        }

        foreach (Path _path in _pathlist_air)
        {
            foreach (TargetPoint _point in _path.PointsList)
            {
                if (_tp_Array_air.Contains(_point) == false)
                {
                    _tp_Array_air.Add(_point);
                }
            }
        }

        DisablepointObjects();
    }

    private void FillPointslist(GameObject[] points_prefabs, List<TargetPoint> pointsList)
    {
        foreach (GameObject point in points_prefabs)
        {
            TargetPoint tp = new TargetPoint(point);
            if (point.tag == "point_land" || point.tag == "point_air")
                tp.edgeType = "land"; // точки траектории по воздуху ставим в ленд, там все равно не используется edgeType
            else if (point.tag == "point_hollow")
                tp.edgeType = "hollow";
            else
            {
                tp.edgeType = "land";
                tp.isFinalTarget = true;
            }

            pointsList.Add(tp);
        }
    }

    public List<Path> GetPathList(string _pathtype)
    {
        if (_pathtype == "air")
            return _pathlist_air;
        else
            return _pathlist;
    }

    public List<TargetPoint> Get_tp_Array(string _pathtype)
    {
        if (_pathtype == "air")
            return _tp_Array_air;
        else
            return _tp_Array;
    }

    private void DisablepointObjects()
    {
        foreach (Transform t in transform)
        {
            if (t.name.Contains("Point_") == true)
            {
                t.gameObject.SetActive(false);
            }
        }
    }

}
