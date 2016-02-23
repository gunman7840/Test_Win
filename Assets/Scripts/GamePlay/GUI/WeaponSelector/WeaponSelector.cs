using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WeaponSelector : MonoBehaviour
{
    public static WeaponSelector instance = null;
    private UIManager uimanager;

    protected Rigidbody2D rb;
    protected RectTransform rt;
    protected Transform _transform;
    
    protected 

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //_transform = transform;
        //rb=GetComponent<Rigidbody2D>();
        uimanager= GameObject.Find("UIManager").GetComponent<UIManager>();
        uimanager.OnCallSelector += MoveandAppeare;
        uimanager.OnCloseGUI += Disappeare;
        _transform = transform;
        gameObject.SetActive(false);
    }



    public void MoveandAppeare(Vector2 _pos)
    {
        gameObject.SetActive(true);
        _transform.position = new Vector3(_pos.x, _pos.y, _transform.position.z);
        //говорят очень 
        //rt.anchoredPosition3D = new Vector3(_pos.x, _pos.y, transform.position.z);
    }

    public void Disappeare()
    {
        gameObject.SetActive(false);
    }


   

}