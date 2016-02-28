using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Level2 : MonoBehaviour, ILevel
{

    public GameObject[] path1_points;
    public GameObject[] path2_points;
    public GameObject[] path3_points;
    public GameObject[] air_path1_points;
    public GameObject[] air_path2_points;

    protected Path path1;
    protected Path path2;
    protected Path path3;
    protected Path air_path1;
    protected Path air_path2;

    //--здесь ничего менять не нужно
    protected List<Path> _pathlist = new List<Path>(); //Лист содержащий все пути
    protected List<Path> _pathlist_air = new List<Path>(); //Лист содержащий все пути по воздуху
    protected List<TargetPoint> _tp_Array = new List<TargetPoint>(); //Лист содержащий все точки всех траекторий
    protected List<TargetPoint> _tp_Array_air = new List<TargetPoint>(); //Лист содержащий все точки всех траекторий по воздуху

    void Awake()
    {
        //Debug.Log("level awake");
        path1 = new Path("Path1", FillPointsList(path1_points, _tp_Array), 10);
        path2 = new Path("Path2", FillPointsList(path2_points, _tp_Array), 5);
        path3 = new Path("Path3", FillPointsList(path3_points, _tp_Array), 5);
        _pathlist.Add(path1);
        _pathlist.Add(path2);
        _pathlist.Add(path3);

        air_path1 = new Path("AirPath1", FillPointsList(air_path1_points, _tp_Array_air), 10);
        air_path2 = new Path("AirPath2", FillPointsList(air_path2_points, _tp_Array_air), 10);
        _pathlist_air.Add(air_path1);
        _pathlist_air.Add(air_path2);

        DisablepointObjects();
    }

    private List<TargetPoint> FillPointsList(GameObject[] points_prefabs, List<TargetPoint> Common_tp_Array)
    {
        List < TargetPoint > _pointslist=new List<TargetPoint>();
        foreach (GameObject pointGO in points_prefabs)
        {
            TargetPoint point = new TargetPoint(pointGO);
            if (Common_tp_Array.Count == 0) //для самой первой точки
            {
                Common_tp_Array.Add(point);
                _pointslist.Add(point);
            }
            else
            {
                TargetPoint _commontp = ArrayHasMatch_pos(pointGO, Common_tp_Array);
                if (_commontp != null)
                {
                    _pointslist.Add(_commontp);
                }
                else
                { 
                Common_tp_Array.Add(point);
                _pointslist.Add(point);
                }
            }
        }


        return _pointslist;
    }

    protected TargetPoint ArrayHasMatch_pos(GameObject pointGO, List<TargetPoint> Common_tp_Array) 
    {
        //Метод возвращает тру если точка с такой позицией уже добавлена в коммонарэй
        foreach (TargetPoint commontp in Common_tp_Array)
        {
            if (commontp.position == (Vector2)pointGO.transform.position)
            {
                return commontp;
            }

        }
        return null;
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
