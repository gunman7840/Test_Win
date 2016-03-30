using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UpgradeSelector : MonoBehaviour
{
    //public static UpgradeSelector instance = null;
    private UIManager uimanager;
    public Camera _camera;
    public Sprite LockSprite;
    public Sprite UpSprite;
    protected WeaponManager _weaponmanager;
    private ResourceManager _resourceManager;

    protected SelectorButton up_button;
    protected SelectorButton sell_button;

    protected Rigidbody2D rb;
    protected Transform _transform;
    protected GameObject _gameObject;

    private WeaponType CurrentWeaponData;
    private WeaponType NextWeaponData;
    private Transform gun_transform;
    private int _gun_id; //СЮДА ЗАПИСЫВАЕМ АЙДИ БАШНИ КОТОРАЯ выбрана в данный момент

    protected GameObject radiusGM;
    protected Transform radiusT;



    void Start() //в авейке все это работает плохо
    {
        _gameObject = gameObject;
        _transform = transform;
        uimanager = GameObject.Find("UIManager").GetComponent<UIManager>();
        uimanager.OnCall_USelector += MoveandAppeare;
        uimanager.OnCall_USelector += ShowRadius; //показываем радиус текущей башни
        uimanager.OnPress_UButton += ShowRadius_next; //показываем радиус апгрейда
        uimanager.OnCloseGUI += Disappeare;
        uimanager.OnCloseGUI += HideRadius;
        uimanager.OnPress_UAButton += Change;
        _weaponmanager = _camera.GetComponent<WeaponManager>();
        _resourceManager = _camera.GetComponent<ResourceManager>();
        up_button = _transform.GetChild(0).GetComponent<SelectorButton>();
        sell_button = _transform.GetChild(1).GetComponent<SelectorButton>();

        radiusGM = GameObject.Find("Radius");
        radiusT = radiusGM.transform;


        _gameObject.SetActive(false);
    }



    public void MoveandAppeare(Transform gun_tr, int gun_id)
    {
        //Debug.Log("US " + gun_tr + gun_id);
        CurrentWeaponData = _weaponmanager.GetAllWeaponData(gun_id);
        NextWeaponData = _weaponmanager.GetAllWeaponData(gun_id + 1);

        if (NextWeaponData != null)
        {
            up_button.SetUButtonState(NextWeaponData, UpSprite);
        }
        else
        {
            up_button.SetButtonLock(LockSprite);
        }

        sell_button.SetSellButtonState(CurrentWeaponData);

        _gameObject.SetActive(true);
        _transform.position = new Vector3(gun_tr.position.x, gun_tr.position.y, _transform.position.z);

        gun_transform = gun_tr;
        _gun_id = gun_id;
        _resourceManager.CallOnChangeResources_method();

    }

    public void Disappeare()
    {
        _gameObject.SetActive(false);

    }

    public void Change(int Button_id)
    {
        //Debug.Log("upgrade selector change  " );
        //Debug.Log("upgrade selector   " + gun_transform + " " + _gun_id);
        if (Button_id == 0)
            _weaponmanager.UpgradeWeapon(gun_transform, _gun_id);
        else if (Button_id == 10)
            _weaponmanager.SellWeapon(gun_transform, _gun_id);
    }

    public void ShowRadius(Transform gun_tr, int gun_id)
    {
        //debugmanager.DrawDebugLine(_transform.position, (Vector2)_transform.position+ new Vector2(5, 0), Color.cyan);
        radiusT.position = _transform.position;
        int rad = _weaponmanager.GetAllWeaponData(gun_id).DetectRadius;
        radiusT.localScale = new Vector2(rad / 5f, rad / 5f);
        radiusGM.SetActive(true);
    }

    public void ShowRadius_next(int b_id)
    {
        int rad = _weaponmanager.GetAllWeaponData(_gun_id + 1).DetectRadius;
        radiusT.localScale = new Vector2(rad / 5f, rad / 5f);
        radiusGM.SetActive(true);
    }

    public void HideRadius()
    {
        radiusGM.SetActive(false);
    }



}