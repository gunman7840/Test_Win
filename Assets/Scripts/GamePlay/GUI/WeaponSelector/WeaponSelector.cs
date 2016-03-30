using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WeaponSelector : MonoBehaviour
{

    public Camera _camera;
    public Sprite LockSprite;

    private WeaponManager _weaponmanager;
    private ResourceManager _resourceManager;
    private DebugManager debugmanager;

    private UIManager uimanager;


    protected RectTransform rt;
    public Transform _transform;
    protected GameObject _gameObject;

    protected GameObject radiusGM;
    protected Transform radiusT;

    protected


    void Start() //в авейке все это работает плохо
    {
        //Debug.Log("WeaponSelector Start");

        debugmanager = GameObject.Find("DebugManager").GetComponent<DebugManager>();

        uimanager = GameObject.Find("UIManager").GetComponent<UIManager>();
        uimanager.OnCall_WSelector += MoveandAppeare;
        uimanager.OnCloseGUI += Disappeare;
        uimanager.OnPress_WButton += ShowRadius;
        uimanager.OnCloseGUI += HideRadius;
        _transform = transform;
        _gameObject = gameObject;
        _gameObject.SetActive(false);

        _weaponmanager = _camera.GetComponent<WeaponManager>();
        _resourceManager = _camera.GetComponent<ResourceManager>();

        radiusGM = GameObject.Find("Radius");
        radiusT = radiusGM.transform;

        foreach (Transform t in _transform)
        {
            if (t.name.Contains("WButton"))
            {
                SelectorButton button = t.GetComponent<SelectorButton>();
                WeaponType weapondata = _weaponmanager.GetPrimaryWeaponData(button.id);
                //Debug.Log("but " + id);

                if (weapondata != null)
                {
                    button.SetWButtonState(weapondata);
                }
                else
                {
                    button.SetButtonLock(LockSprite);
                }
            }
        }
    }

    public void MoveandAppeare(Vector2 _pos)
    {
        gameObject.SetActive(true);
        _transform.position = new Vector3(_pos.x, _pos.y, _transform.position.z);
        _resourceManager.CallOnChangeResources_method();
        
    }

    public void Disappeare()
    {
        gameObject.SetActive(false);
    }

    public void ShowRadius(int id)
    {
        //debugmanager.DrawDebugLine(_transform.position, (Vector2)_transform.position+ new Vector2(5, 0), Color.cyan);
        radiusT.position = _transform.position;
        int rad = _weaponmanager.GetPrimaryWeaponData(id).DetectRadius;
        radiusT.localScale = new Vector2(rad / 5f, rad / 5f);
        radiusGM.SetActive(true);
    }

    public void HideRadius()
    {
        radiusGM.SetActive(false);
    }


}