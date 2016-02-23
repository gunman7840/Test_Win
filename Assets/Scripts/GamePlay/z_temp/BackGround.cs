using UnityEngine;
using System.Collections;

public class BackGround : MonoBehaviour {

    public GameObject BG;
    public GameObject Box;
    public GameObject Box2;
    

    void BoardSetup()
    {
       
        //GameObject bg = Instantiate(BG, new Vector3 (0f, 0f, 0f), Quaternion.identity) as GameObject;
        //GameObject box = Instantiate(Box, new Vector3 (5f, 0f, 0f), Quaternion.identity) as GameObject;
        //GameObject box2 = Instantiate(Box2, new Vector3 (0f, 0f, 0f), Quaternion.identity) as GameObject;


    }


    public void SetupScene()
    {
        BoardSetup();
    }

}
