using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Navigation : MonoBehaviour {

    protected Level2 LevelData;
    protected DebugManager debugmanager;

    protected List<TargetPoint> TP_Array = new List<TargetPoint>(); //Лист содержащий все точки всех траекторий
    protected List<Path> PathList = new List<Path>(); //Лист содержащий все пути
    static System.Random random = new System.Random();

    void Awake() {

        // awake чтобы инициализировать траектории были готовы к моменту старта Player
        LevelData = GameObject.Find("path").GetComponent<Level2>(); //Получаем доступ к классу
        debugmanager = GameObject.Find("DebugManager").GetComponent<DebugManager>();
        PathList = LevelData.GetPathList();
        TP_Array= LevelData.Get_tp_Array();

    }

    //------------------------------------------------------------------------------test
    public TargetPoint GetStartPoint()
    {
        return LevelData._getStartPoint();
    }    
    //------------------------------------------------------------------------------test

    public Path GetPath(TargetPoint CurrentPoint)  
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
                Available_PathList.Add(item, _priority);
            }
        }

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

    public TargetPoint GetTarget(Vector2 _position,Path Trajectory, TargetPoint _currentNextTarget)
    {
        //Debug.Log("------------------------------------------------------------------------Get target ");
     
        // var debugmanager = GameObject.Find("DebugManager").GetComponent<DebugManager>(); 


        //Понижаем рейтинг точки до которой не удалось добраться
        RateTarget(Trajectory, _currentNextTarget);

        TargetPoint _target = _currentNextTarget; //На случай если не удасться найти ничего подходящего, оставляем текущую точку
        //Из массива  TP_Array получаем все точки удовлетворяющие условиям, сортируем их по полю  fails           
        var query=  from point in TP_Array
                    where (Vector2.Distance(_position, point.position)<7)
                    orderby Vector2.Distance(_position, point.position) ascending
                    select point;
        foreach (TargetPoint item in query)
        {
            RaycastHit2D hit = Physics2D.Linecast(new Vector2(_position.x, _position.y + 2), item.position);
            if (hit.point.x==0 )    //Оставляем только те точки которые достаточно близко и в прямой видимости, если лайнкаст не встречает препятсвия то точка  hit = 0.0
            {
                _target = item;
                debugmanager.DrawDebugLine(_position, item.position, Color.blue);
                break;
            }
        }

        


        return _target;         
    }

    public void RateTarget(Path _trajectory, TargetPoint _point)  
    {
        //TargetPoint tp = TP_Array.Find(x => x.position==_point); //в массиве находим таргет поинт по условию //Применялось когда в аргументе _point мы получали Vector2
        _point.fails += 1;
        _trajectory.PathFails += 1;
    }

    public TargetPoint GetFinalTarget()
    {
        return LevelData._target;
    }

}
