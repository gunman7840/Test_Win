using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour {

    public delegate void EventAction(GameObject enemy) ;

    public static event EventAction OnTargetReached;

    public void TargetReached(GameObject enemy)
    {
        OnTargetReached(enemy);
        //Debug.Log("TargetReached");
    }

    /*
    public static event EventAction DestroyEnemy_event;

    public void DestroyEnemy(GameObject enemy)
    {
        DestroyEnemy_event(enemy);
        //Debug.Log("TargetReached");
    }
    */
}
