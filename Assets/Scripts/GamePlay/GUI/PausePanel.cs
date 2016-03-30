

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    private Camera _camera;
    private UIManager uimanager;
    private GameObject gm;
    private Transform tr;

    void Awake()
    {
        gm = gameObject;
        tr = transform;
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        uimanager = GameObject.Find("UIManager").GetComponent<UIManager>();
        uimanager.OnPress_Pause += Appear;
        uimanager.OnPress_Resume += Disappeare;
        gm.SetActive(false);
    }

    void Appear()
    {
        gm.SetActive(true);
    }

    void Disappeare()
    {
        gm.SetActive(false);
    }

}