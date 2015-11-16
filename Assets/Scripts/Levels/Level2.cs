using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Level2 : MonoBehaviour {

    public GameObject target;
    public GameObject point_1;
    public GameObject point_2;
    public GameObject point_3;
    public GameObject point_4;
    public GameObject point_5;
    public GameObject point_6;
    public GameObject point_7;
    public GameObject point_8;
    public GameObject point_9; 
    public GameObject point_10;
    public GameObject point_11;
    public GameObject point_12;
    public GameObject point_13;
    public GameObject point_14;
    public GameObject point_15;
    public GameObject point_16;
    public GameObject point_17;

    public TargetPoint _target; 
    private TargetPoint _point_1;
    private TargetPoint _point_2;
    private TargetPoint _point_3;
    private TargetPoint _point_4;
    private TargetPoint _point_5;
    private TargetPoint _point_6;
    private TargetPoint _point_7; // hollowedge
    private TargetPoint _point_8;
    private TargetPoint _point_9;  
    private TargetPoint _point_10;
    private TargetPoint _point_11;
    private TargetPoint _point_12;
    private TargetPoint _point_13;
    private TargetPoint _point_14;
    private TargetPoint _point_15;
    private TargetPoint _point_16;
    private TargetPoint _point_17;


    protected List<TargetPoint> _tp_Array = new List<TargetPoint>(); //Лист содержащий все точки всех траекторий


    protected List<Path> _pathlist = new List<Path>(); //Лист содержащий все пути
    protected Path path1;
    protected Path path2;
    

    void Awake()
    {

        // awake чтобы инициализировать траектории были готовы к моменту старта Player
        _target = new TargetPoint(target.transform.position, "land");
        _point_1= new TargetPoint(point_1.transform.position, "land");
        _point_2 = new TargetPoint(point_2.transform.position, "land");
        _point_3 = new TargetPoint(point_3.transform.position, "hollow");
        _point_4 = new TargetPoint(point_4.transform.position, "land");
        _point_5 = new TargetPoint(point_5.transform.position, "hollow");
        _point_6 = new TargetPoint(point_6.transform.position, "land");
        _point_7 = new TargetPoint(point_7.transform.position, "hollow");
        _point_8 = new TargetPoint(point_8.transform.position, "land");
        _point_9 = new TargetPoint(point_9.transform.position, "hollow");
        _point_10 = new TargetPoint(point_10.transform.position, "hollow");
        _point_11 = new TargetPoint(point_11.transform.position, "hollow");
        _point_12 = new TargetPoint(point_12.transform.position, "land");
        _point_13 = new TargetPoint(point_13.transform.position, "land");
        _point_14 = new TargetPoint(point_14.transform.position, "hollow");
        _point_15 = new TargetPoint(point_15.transform.position, "land");
        _point_16 = new TargetPoint(point_16.transform.position, "hollow");
        _point_17 = new TargetPoint(point_17.transform.position, "land");

        //Создаем объекты траекторий и наполняем их точками
        TargetPoint[] _pointsarray_1 = { _target, _point_1, _point_2, _point_3, _point_4, _point_5, _point_6, _point_7, _point_8, _point_9, _point_13, _point_14, _point_15, _point_16, _point_17 };
        List<TargetPoint> _pointslist_1 = new List<TargetPoint>(_pointsarray_1);
        path1 = new Path("Path1", _pointslist_1, 10);

        TargetPoint[] _pointsarray_2 = { _target, _point_1, _point_2, _point_3, _point_4, _point_10, _point_12, _point_11, _point_8, _point_9, _point_13, _point_14, _point_15, _point_16, _point_17 };
        List<TargetPoint> _pointslist_2 = new List<TargetPoint>(_pointsarray_2);
        path2 = new Path("Path2", _pointslist_2, 10);


        _pathlist.Add(path1);
        _pathlist.Add(path2);


        //заполняем лист TP_Array
        TargetPoint[] _points_temp_array = { _target, _point_1, _point_2, _point_3, _point_4, _point_5, _point_6, _point_7, _point_8, _point_9, _point_10, _point_11, _point_12, _point_13, _point_14, _point_15, _point_16, _point_17 };
        _tp_Array= new List<TargetPoint>(_points_temp_array);

        foreach (TargetPoint temppoint in _tp_Array)
        {
            temppoint.fails = 0;
        }

    }

    public List<Path> GetPathList()
    {
        return _pathlist;
    }

    public List<TargetPoint> Get_tp_Array()
    {
        return _tp_Array;
    }

    public TargetPoint _getStartPoint()
    {
        return _point_17;

    }



}
