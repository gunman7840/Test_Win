using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UpgradeSelector : MonoBehaviour
{
    public static UpgradeSelector instance = null;
    private UIManager uimanager;
    public Camera _camera;
    protected WeaponManager weaponmanager;


    protected Rigidbody2D rb;
    protected RectTransform rt;
    protected Transform _transform;

    private Transform gun_transform;
    private int _gun_id;

    protected

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        
        //----------------------
      
    }

    void Start()
    {
        uimanager = GameObject.Find("UIManager").GetComponent<UIManager>();
        uimanager.OnCallUpgradeSelector += MoveandAppeare;
        uimanager.OnCloseGUI += Disappeare;
        uimanager.OnPressUpgradeSelectorButton_app += Change;
        _transform = transform;
        weaponmanager = _camera.GetComponent<WeaponManager>();
        gameObject.SetActive(false);
    }



    public void MoveandAppeare(Transform gun_tr, int gun_id)
    {
        //Debug.Log("US " + gun_tr + gun_id);

        gameObject.SetActive(true);
        _transform.position = new Vector3(gun_tr.position.x, gun_tr.position.y, _transform.position.z);

        gun_transform = gun_tr;
        _gun_id = gun_id;
        //говорят очень непроизводительно двигать трансформ гуи элемента
        //rt.anchoredPosition3D = new Vector3(_pos.x, _pos.y, transform.position.z);

        //Здесь нужно обратиться в weaponmanager с айдишником башни, получить оттуда айди след апгрейда, цену, описание и так далее, и записать это все в локальные поля
    }

    public void Disappeare()
    {
        gameObject.SetActive(false);

    }

    public void Change(int Button_id)
    {
        Debug.Log("upgrade selector change  " );
        Debug.Log("upgrade selector   " + gun_transform + " " + _gun_id);
        if (Button_id == 0)
            weaponmanager.UpgradeWeapon(gun_transform, _gun_id, _transform);
        else if (Button_id == 1)
            weaponmanager.SellWeapon(gun_transform);
    }


  

}