using UnityEngine;
using System.Collections;

public class Move_camera : MonoBehaviour
{
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


        //verticalExtent = _camera.orthographicSize;
        //horizontalExtent = _camera.orthographicSize * Screen.width / Screen.height;
        Camera_extent = new Vector3(_camera.orthographicSize * Screen.width / Screen.height, _camera.orthographicSize,0);
        _min = bound_min.transform.position  + Camera_extent;
        _max = bound_max.transform.position  - Camera_extent;

        Debug.Log("min" + _min);
        Debug.Log("max" + _max);
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
        if (isPanning)
        {
                Vector3 pos = _camera.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
                Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
                _transform.Translate(move, Space.Self);
        }

    }

    protected void MakeDetectionTouch()
    {
        //Debug.Log("MakeDetectionTouch");
        Touch[] touches = Input.touches;

        if (touches.Length < 1)
        {
            //if the camera is currently scrolling
            if (scrollVelocity != 0.0f)
            {
                //slow down over time
                float t = (Time.time - timeTouchPhaseEnded) / inertiaDuration;
                float frameVelocity = Mathf.Lerp(scrollVelocity, 0.0f, t);
                _camera.transform.position += -(Vector3)scrollDirection.normalized * (frameVelocity * 0.05f) * Time.deltaTime;

                if (t >= 1.0f)
                    scrollVelocity = 0.0f;
            }
        }

        if (touches.Length ==1)
        {
            //Single touch (move)

                if (touches[0].phase == TouchPhase.Began)
                {
                    scrollVelocity = 0.0f;
                }
                else if (touches[0].phase == TouchPhase.Moved)
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