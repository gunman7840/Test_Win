using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour {

    public delegate void EventAction(GameObject enemy) ;

    public static event EventAction OnTargetReached;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

      

    }

    public void TargetReached(GameObject enemy)
    {
        OnTargetReached(enemy);
        //Debug.Log("TargetReached");
    }
}
