using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Navigation : MonoBehaviour {

    public LayerMask myLayerMask; //Маска для лайнкастов в методе GetTarget
    protected ILevel LevelData;
    protected DebugManager debugmanager;

    protected List<TargetPoint> TP_Array = new List<TargetPoint>(); //Лист содержащий все точки всех траекторий
    protected List<Path> PathList = new List<Path>(); //Лист содержащий все пути
    protected List<TargetPoint> TP_Array_air = new List<TargetPoint>(); //Лист содержащий все точки всех траекторий
    protected List<Path> PathList_air = new List<Path>(); //Лист содержащий все пути
    static System.Random random = new System.Random();


    void Awake() {
        //Debug.Log("navigation awake");
        // awake чтобы инициализировать траектории были готовы к моменту старта Player
        LevelData = GameObject.Find("Level").GetComponent<ILevel>(); //Получаем доступ к классу
        debugmanager = GameObject.Find("DebugManager").GetComponent<DebugManager>();
        PathList = LevelData.GetPathList("land");
        TP_Array= LevelData.Get_tp_Array("land");
        PathList_air = LevelData.GetPathList("air");
        TP_Array_air = LevelData.Get_tp_Array("air");
    }

    public Path GetPath(TargetPoint CurrentPoint, string enemytype)  
    {
        //Debug.Log("navigation GetPath  ");
        // Метод возвращает траекторию выбранную из массива случайным образом с учетом приоритетов , а также содержащую точку в которой объект находится сейчас
        //Заполняем словарь в котором содержаться только доступные траектории и их относительные приоритеты

        List<Path> temp_PathList = null;
        switch (enemytype)
        {
            case "inf":
                temp_PathList = PathList;
                break;
            case "air":
                temp_PathList = PathList_air;
                break;
        }

        Path defaultPath = null; ; // Используем эту траекторию в случае фейла алгоритма
        Dictionary<Path, float> Available_PathList = new Dictionary<Path, float>(); //закэшировать
        int SUMPriority = 0; // Сумма приоритетов всех доступных траекторий, это значение мы примем за 100 % при расчете приоритетов

        //Пробегаемся по всем доступным траекториям и вычисляем SUMPriority
       

        foreach (Path item in temp_PathList)
        {
            /*
            if (item == null)
                Debug.Log("NULL ");

            Debug.Log("temp_PathList " + temp_PathList.Count);
            Debug.Log("PathName " + item.PathName);
            Debug.Log("item.PointsList " + item.PointsList.Count);
            */


            if (item.PointsList.Contains(CurrentPoint))
            {
                if (defaultPath == null)
                {
                    defaultPath = item;
                }
                SUMPriority += item.PathPriority;
            }
        }
        //Пробегаемся второй раз и заполняем словарь Available_PathList
        //Debug.Log("temp_PathList " + temp_PathList.Count);
        foreach (Path item in temp_PathList)
        {
            if (item.PointsList.Contains(CurrentPoint))
            {
               //debugmanager.DrawDebugLine(new Vector2(0, 0), CurrentPoint.position, Color.red);
                float _priority = (float)item.PathPriority / SUMPriority;
                Available_PathList.Add(item, _priority);
                //Debug.Log("item.PathName" + item.PathName);
                //Debug.Log("_priority" + _priority);
            }
        }

        //Debug.Log("Available_PathList " + Available_PathList.Count);
        //Собственно выбираем траекторию

        var rnd = random.NextDouble(); //Получаем значение между 0 и 1
        foreach (var item in Available_PathList)
        {
            if (rnd < Available_PathList[item.Key]) //Если значение приоритета больше чем случайное значение, то возвращаем его. В противном случае уменьшаем случайное значение и переходим к след элементу массива
            {
                //Debug.Log("Get Path " + item.Key.PathName + " prioprity: " + Available_PathList[item.Key] );
                return item.Key;
            }
               rnd -= Available_PathList[item.Key];           
        }


        return defaultPath;
    }
   
    public TargetPoint GetTarget(Vector2 _position, TargetPoint _currentNextTarget, string enemytype)
    {
        //Debug.Log("------------------------------------------------------------------------Get target ");
        // var debugmanager = GameObject.Find("DebugManager").GetComponent<DebugManager>(); 

        TargetPoint _target = _currentNextTarget; //На случай если не удасться найти ничего подходящего, оставляем текущую точку
                                                  //Из массива  TP_Array получаем все точки удовлетворяющие условиям  

        List<TargetPoint> temp_TP_Array = null;
        switch (enemytype)
        {
            case "inf":
                temp_TP_Array = TP_Array;
                break;
            case "air":
                temp_TP_Array = TP_Array_air;
                break;
        }


        var query=  from point in temp_TP_Array
                    where (Vector2.Distance(_position, point.position)<7)
                    orderby Vector2.Distance(_position, point.position) ascending
                    select point;
        foreach (TargetPoint item in query)
        {
            RaycastHit2D hit = Physics2D.Linecast(_position, item.position, myLayerMask);
            if (hit.point.x==0 )    //Оставляем только те точки которые достаточно близко и в прямой видимости, если лайнкаст не встречает препятсвия то точка  hit = 0.0
            {
                _target = item;
                debugmanager.DrawDebugLine(_position, item.position, Color.blue);
                break;
            }
        }

        return _target;         
    }

    public TargetPoint GetStartPoint(Vector2 _position, string enemytype)
    {
        //Debug.Log("------------------------------------------------------------------------Get target ");
        // var debugmanager = GameObject.Find("DebugManager").GetComponent<DebugManager>(); 

        List<TargetPoint> temp_TP_Array=null;
        switch (enemytype)
        {
            case "inf":
                temp_TP_Array = TP_Array;
                break;
            case "air":
                temp_TP_Array = TP_Array_air;
                break;
        }
        TargetPoint _target=temp_TP_Array[0];
        foreach (TargetPoint point in temp_TP_Array)
        {
           if( Vector2.Distance(_position, point.position) < 3 && (Vector2.Distance(_position, point.position)< Vector2.Distance(_position, _target.position)))
            {
                _target = point;
            }
        }
        return _target;
    }
}
