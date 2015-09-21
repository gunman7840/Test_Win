using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    //public GameObject Robot;

    private BackGround boardScript;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);


        DontDestroyOnLoad(gameObject);
        boardScript = GetComponent<BackGround>();
        InitGame();


    }

    void InitGame()
    {
        boardScript.SetupScene();
        //GameObject robot = Instantiate(Robot, new Vector3(-13f, -5f, 0f), Quaternion.identity) as GameObject;
       
    }




   
}
