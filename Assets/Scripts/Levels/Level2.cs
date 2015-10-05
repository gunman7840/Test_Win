using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Level2 : MonoBehaviour
{

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



    protected List<TargetPoint> _tp_Array = new List<TargetPoint>(); //Лист содержащий все точки всех траекторий


    protected List<Path> _pathlist = new List<Path>(); //Лист содержащий все пути
    protected Path path1;
    protected Path path2;
    protected Path path3;
    protected Path path4;

    void Awake()
    {

        // awake чтобы инициализировать траектории были готовы к моменту старта Player

        //Создаем объекты траекторий и наполняем их точками
        Vector2[] _pointsarray_1 = { target.transform.position, point_1.transform.position, point_6.transform.position, point_7.transform.position, point_10.transform.position };
        List<Vector2> _pointslist_1 = new List<Vector2>(_pointsarray_1);
        path1 = new Path("Path1", _pointslist_1, 10);

        Vector2[] _pointsarray_2 = { target.transform.position, point_1.transform.position, point_6.transform.position, point_8.transform.position, point_9.transform.position, point_10.transform.position };
        List<Vector2> _pointslist_2 = new List<Vector2>(_pointsarray_2);
        path2 = new Path("Path2", _pointslist_2, 10);

        Vector2[] _pointsarray_3 = { target.transform.position, point_2.transform.position, point_3.transform.position, point_4.transform.position, point_5.transform.position, point_6.transform.position, point_7.transform.position, point_10.transform.position };
        List<Vector2> _pointslist_3 = new List<Vector2>(_pointsarray_3);
        path3 = new Path("Path3", _pointslist_3, 8);

        Vector2[] _pointsarray_4 = { target.transform.position, point_2.transform.position, point_3.transform.position, point_4.transform.position, point_5.transform.position, point_6.transform.position, point_8.transform.position, point_9.transform.position, point_10.transform.position };
        List<Vector2> _pointslist_4 = new List<Vector2>(_pointsarray_4);
        path4 = new Path("Path4", _pointslist_4, 8);

        _pathlist.Add(path1);
        _pathlist.Add(path2);
        _pathlist.Add(path3);
        _pathlist.Add(path4);

        //заполняем лист TP_Array
        Vector2[] _points_temp_array = { target.transform.position, point_1.transform.position, point_2.transform.position, point_3.transform.position, point_4.transform.position, point_5.transform.position, point_6.transform.position, point_7.transform.position, point_8.transform.position, point_9.transform.position, point_10.transform.position };
        foreach (Vector2 temppoint in _points_temp_array)
        {
            TargetPoint _point = new TargetPoint();
            _point.position = temppoint;
            _point.fails = 0;
            _tp_Array.Add(_point);
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

    public Vector2 _getStartPoint()
    {
        return point_10.transform.position;

    }



}
