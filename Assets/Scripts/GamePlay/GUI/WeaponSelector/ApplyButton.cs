﻿
using UnityEngine;
using System.Collections;

public class ApplyButton : MonoBehaviour
{
    private UIManager uimanager;
    public int ApplyButton_id;

    private GameObject gm;

    void Start()
    {
        gm = gameObject;
        uimanager = GameObject.Find("UIManager").GetComponent<UIManager>();
        uimanager.OnPress_WButton += Appeare;
        uimanager.OnCloseGUI += Disappeare;
        gm.SetActive(false);
    }


    public void Appeare( int Button_id)
    {
        if (Button_id == ApplyButton_id)
            gm.SetActive(true);
        else
            Disappeare();
    }

    public void Disappeare()
    {
        if (gm.activeSelf)
        gm.SetActive(false);
    }



}

