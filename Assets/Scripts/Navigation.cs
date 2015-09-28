using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Navigation : MonoBehaviour {

	//  
    public GameObject target;
    public GameObject point_1;
    public GameObject point_2;
    public GameObject point_3;
    public GameObject point_4;
    public GameObject point_5;
    public GameObject point_6;
    public GameObject point_7;
    
    public class TargetPoint
    {
        public Vector2 position;
        public float distance;
        public int fails;
    }

    List<TargetPoint> TP_Array = new List<TargetPoint>(); //Лист содержащий все точки всех траекторий


    public List<Path> PathList = new List<Path>(); //Лист содержащий все пути
    public Path path1;
    public Path path2;
    public Path path3;

    static System.Random random = new System.Random();

    void Awake() {

        // awake чтобы инициализировать траектории были готовы к моменту старта Player

        //Создаем объекты траекторий и наполняем их точками
        Vector2[] _pointsarray_1 = { target.transform.position, point_1.transform.position, point_4.transform.position, point_7.transform.position };
        List<Vector2> _pointslist_1= new List<Vector2>(_pointsarray_1);
        path1 = new Path("Path1", _pointslist_1, 15);

        Vector2[] _pointsarray_2 = { target.transform.position, point_1.transform.position, point_3.transform.position, point_5.transform.position, point_6.transform.position, point_7.transform.position };
        List<Vector2> _pointslist_2 = new List<Vector2>(_pointsarray_2);
        path2 = new Path("Path2", _pointslist_2, 10);

        Vector2[] _pointsarray_3 = { target.transform.position, point_2.transform.position, point_3.transform.position, point_5.transform.position, point_6.transform.position, point_7.transform.position };
        List<Vector2> _pointslist_3 = new List<Vector2>(_pointsarray_3);
        path3 = new Path("Path3", _pointslist_3, 7);

        PathList.Add(path1);
        PathList.Add(path2);
        PathList.Add(path3);

        //заполняем лист TP_Array
        Vector2[] _points_temp_array={ target.transform.position, point_1.transform.position, point_2.transform.position, point_3.transform.position , point_4.transform.position , point_5.transform.position , point_6.transform.position , point_7.transform.position };
        foreach(Vector2 temppoint in _points_temp_array)
        {
            TargetPoint _point = new TargetPoint();
            _point.position = temppoint;
            _point.fails = 0;
            TP_Array.Add(_point);
        }

    }

    //------------------------------------------------------------------------------test
    public Vector2 GetStartPoint()
    {
        return point_7.transform.position ;
    }    
    //------------------------------------------------------------------------------test

    public Path GetPath(Vector2 CurrentPoint)  
    {
        
        // Метод возвращает траекторию выбранную из массива случайным образом с учетом приоритетов , а также содержащую точку в которой объект находится сейчас
        //Заполняем словарь в котором содержаться только доступные траектории и их относительные приоритеты
        Path defaultPath = null; ; // Используем эту траекторию в случае фейла алгоритма
        Dictionary<Path, float> Available_PathList = new Dictionary<Path, float>();
        int SUMPriority = 0; // Сумма приоритетов всех доступных траекторий, это значение мы примем за 100 % при расчете приоритетов

        //Пробегаемся по всем доступным траекториям и вычисляем SUMPriority

        foreach (Path item in PathList)
        {
            if (item.PointsList.Contains(CurrentPoint))
            {
                if (defaultPath == null)
                {
                    defaultPath = item;
                }
                SUMPriority += item.PathPriority;
                SUMPriority -= item.PathFails;
            }
        }
        //Пробегаемся второй раз и заполняем словарь Available_PathList
        foreach(Path item in PathList)
        {
            if (item.PointsList.Contains(CurrentPoint))
            {          
                float _priority=((float)item.PathPriority - (float)item.PathFails) / SUMPriority;
                //Debug.Log("---------------------------Path actual priority " + item.PathName + " " + _priority);
                Available_PathList.Add(item, _priority);
            }
        }

       //Собственно выбираем траекторию
       var rnd = random.NextDouble(); //Получаем значение между 0 и 1
      
       foreach (var item in Available_PathList)
        {
            if (rnd < Available_PathList[item.Key]) //Если значение приоритета больше чем случайное значение, то возвращаем его. В противном случае уменьшаем случайное значение и переходим к след элементу массива
            {
                Debug.Log("Get Path " + item.Key.PathName + " prioprity: " + Available_PathList[item.Key] );
                return item.Key;
            }
               rnd -= Available_PathList[item.Key];           
        }
        return defaultPath;
    }

    public Vector2 GetTarget(Vector2 _position,Path Trajectory, Vector2 _currentNextTarget)
    {
        //Debug.Log("------------------------------------------------------------------------Get target ");
     
         var debugmanager = GameObject.Find("DebugManager").GetComponent<DebugManager>(); 


        //Понижаем рейтинг точки до которой не удалось добраться
        RateTarget(Trajectory, _currentNextTarget);

        Vector2 _target = _currentNextTarget; //На случай если не удасться найти ничего подходящего, оставляем текущую точку
        //Из массива  TP_Array получаем все точки удовлетворяющие условиям, сортируем их по полю  fails           
        var query=  from point in TP_Array
                    where (Mathf.Sqrt(Mathf.Pow((_position.x - point.position.x), 2) + Mathf.Pow((_position.y - point.position.y), 2)) <20)
                    orderby point.fails ascending
                    select point;
        foreach (TargetPoint item in query)
        {
            RaycastHit2D hit = Physics2D.Linecast(new Vector2(_position.x, _position.y + 2), item.position);
            if (hit.point.x==0 )                     //Оставляем только те точки которые достаточно близко и в прямой видимости, если лайнкаст не встречает препятсвия то точка  hit = 0.0
            {
                _target = item.position;
                debugmanager.DrawDebugLine(_position, item.position, Color.blue);
                //Debug.Log("Get POINT " + item.fails);
                break;
            }
        }

        return _target;         
    }

    public void RateTarget(Path _trajectory, Vector2 _point)  
    {
        TargetPoint tp = TP_Array.Find(x => x.position==_point); //в массиве находим таргет поинт по условию
        tp.fails += 1;
        _trajectory.PathFails += 1;
    }

}
