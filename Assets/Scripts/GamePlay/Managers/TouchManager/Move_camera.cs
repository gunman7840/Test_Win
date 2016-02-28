using UnityEngine;
using System.Collections;

public class Move_camera : MonoBehaviour
{
    public UIManager uimanager;
    private Camera _camera;
    private Transform _transform;

    public GameObject bound_min;
    public GameObject bound_max;
    //private float horizontalExtent, verticalExtent;
    private Vector3 Camera_extent;
    private Vector3 _min;
    private Vector3 _max;

    public float panSpeed = 2.0f;     

    private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
    private bool isPanning;     // Is the camera being panned?
    private float Padding = 0.5f;

    //-----moved from script
    public float moveSensitivityX = 1f;
    public float moveSensitivityY = 1f;
    public bool invertMoveX = false;
    public bool invertMoveY = false;

    public float inertiaDuration = 1.0f;
    private float scrollVelocity = 0.0f;
    private float timeTouchPhaseEnded;
    private Vector2 scrollDirection = Vector2.zero;

    //------------------------------------------------------------------------------
    void Start()
    {
        _camera = Camera.main;
        _transform = transform;
        //-----------Подгонка масштаба карты под размер экрана
        float ScreenRatio = (float)Screen.width / (float)Screen.height; //без кастов всегда возвращает единицу
        float mapWidth = bound_max.transform.position.x - bound_min.transform.position.x;
        float mapHeight = bound_max.transform.position.y - bound_min.transform.position.y;
        float DesiredVisibleX = mapWidth * 0.75f; //мы всегда хоти вижеть 75% ширины карты, независимо от соотношения сторон экрана
        float OrtSize = DesiredVisibleX / (2 * ScreenRatio);
        _camera.orthographicSize = OrtSize;

        Camera_extent = new Vector3(_camera.orthographicSize * ((float)Screen.width / (float)Screen.height), _camera.orthographicSize, 0); //чтобы в min & max упирался не центр камеры а ее край
        /*
        Debug.Log("Screen.width " + Screen.width);
        Debug.Log("Screen.height " + Screen.height);

        Debug.Log("ScreenRatio " + ScreenRatio);
        Debug.Log("DesiredVisibleX " + DesiredVisibleX);
        Debug.Log("DesiredVisibleX / 2 * ScreenRatio " + DesiredVisibleX / 2 * ScreenRatio);
        */

        if(OrtSize * 2> mapHeight) // это актуально при шировких экранах  типа айпада
        {
            _min = new Vector2(-Padding, -(OrtSize * 2 - mapHeight) / 2) + (Vector2)Camera_extent;
            _max = new Vector2(mapWidth + Padding, (OrtSize * 2 - mapHeight) / 2 + mapHeight) - (Vector2)Camera_extent;
        }
        else //это актуально на длинных экранах
        {
            _min = (Vector2)bound_min.transform.position + new Vector2(-Padding, -Padding) + (Vector2)Camera_extent;
            _max = (Vector2)bound_max.transform.position + new Vector2(Padding, Padding) - (Vector2)Camera_extent;
        }

        //Debug.Log("min " + ((Vector2)_min - (Vector2)Camera_extent));
        //Debug.Log("max " + ((Vector2)_max + (Vector2)Camera_extent));

        uimanager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    void Update()
    {
        PlatformSpec_Detect();
    }


    protected void PlatformSpec_Detect()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
        MakeDetectionMouse();
#else
       MakeDetectionTouch();
#endif
    }

    protected void MakeDetectionMouse()
    {


        //---------------------------------------------------------------------------------
        // Get the left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            // Get mouse origin         
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }
        // Disable movements on button release
        if (!Input.GetMouseButton(0)) isPanning = false;
        // Move the camera on it's XY plane
        if (isPanning && (uimanager.BlockScreenMove!=true))
        {
                Vector3 pos = _camera.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
                Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
                _transform.Translate(move, Space.Self);
        }
    }

    protected void MakeDetectionTouch()
    {
        //нужно оставновить движение экрана при зажатом селекторе
        //Debug.Log("MakeDetectionTouch");
        Touch[] touches = Input.touches;

        if (touches.Length < 1)
        {
            //if the camera is currently scrolling
            if (scrollVelocity != 0.0f)
            {
                //slow down over time
                //float t = (Time.time - timeTouchPhaseEnded) / inertiaDuration;
                //float frameVelocity = Mathf.Lerp(scrollVelocity, 0.0f, t);
                //_camera.transform.position += -(Vector3)scrollDirection.normalized * (frameVelocity * 0.05f) * Time.deltaTime;  // Вот из за этой строчки камера иногда улетает далеко от карты, нужно поиграть с коэффициентом 0.05

                //if (t >= 1.0f)
                  //  scrollVelocity = 0.0f;
            }
        }

        if (touches.Length ==1)
        {
            //Single touch (move)

            if (touches[0].phase == TouchPhase.Began)
                {
                    scrollVelocity = 0.0f;
                }
            else if (touches[0].phase == TouchPhase.Moved && (uimanager.BlockScreenMove != true))
                {
                    Vector2 delta = touches[0].deltaPosition;

                    float positionX = delta.x * moveSensitivityX * Time.deltaTime;
                    positionX = invertMoveX ? positionX : positionX * -1;

                    float positionY = delta.y * moveSensitivityY * Time.deltaTime;
                    positionY = invertMoveY ? positionY : positionY * -1;

                    _camera.transform.position += new Vector3(positionX, positionY, 0);

                    scrollDirection = touches[0].deltaPosition.normalized;
                    scrollVelocity = touches[0].deltaPosition.magnitude / touches[0].deltaTime;


                    if (scrollVelocity <= 100)
                        scrollVelocity = 0;
                }
            else if (touches[0].phase == TouchPhase.Ended)
                {
                    timeTouchPhaseEnded = Time.time;
                }
        }

    }


    void LateUpdate()
    {
        Vector3 limitedCameraPosition = _transform.position;
        limitedCameraPosition.x = Mathf.Clamp(limitedCameraPosition.x, _min.x, _max.x);
        limitedCameraPosition.y = Mathf.Clamp(limitedCameraPosition.y, _min.y, _max.y);
        _camera.transform.position = limitedCameraPosition;
    }

}