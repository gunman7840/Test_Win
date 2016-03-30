using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    //public static GameManager instance = null;

    private Camera _camera;
    private UIManager uimanager;
    private EventManager eventmanager;
    private Score score;

    //public int TargetRiched = 0;
    public int HP;
    public int WafesAll=0;
    private int WafesPassed=0;

    void Awake()
    {
        //Debug.Log(" Awake gamemanager ");
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        uimanager = GameObject.Find("UIManager").GetComponent<UIManager>();
        eventmanager=_camera.GetComponent<EventManager>();

        uimanager.OnPress_Pause += PauseGame;
        uimanager.OnPress_Resume += unPausegame;
        EventManager.OnTargetReached += SubtractHealth;
        eventmanager.OnSetwafes_start += SetWafes_start;
        eventmanager.OnEncreaseWafe += EncreaseWafe;

    }

    void PauseGame()
    {
        Time.timeScale = 0;
    }

    void unPausegame()
    {
        Time.timeScale = 1;
    }

    void SubtractHealth(Transform enemy_tr)
    {
        HP--;
        if(HP == 0)
        {
            eventmanager.OnGameOver_meth();
        }
    }

    public void SetWafes_start(int _wafes)
    {
        WafesAll += _wafes;
    }

    public void EncreaseWafe()
    {
        WafesPassed++;
    }

}
