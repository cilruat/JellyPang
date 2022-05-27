using UnityEngine;
using System.Collections;

public class InputCtrl : MonoBehaviour
{
    public delegate void OnTouchBoardFunc(Vector3 touchWorldPos);
    public delegate void OnSlideBoardFunc(GridPosition.DIRECTION dir);
    public event OnTouchBoardFunc OnTouchBoard;
    public event OnSlideBoardFunc OnSlideBoard;

    private bool _isLock = false;
    private Plane _boardPlane;
    private Camera _mainCamera;
    private Vector3? _touchPos = null;
    private float _slideMinDist = 45f;   //슬라이드 판정에 필요한 최소한의 터치 거리

    public bool IsLock { get => _isLock; set => _isLock = value; }

    public void Init(Vector3 boardWorldPos, Vector3 boardNormal, OnTouchBoardFunc touchBoardCallback, OnSlideBoardFunc slideBoardCallback)
    {
        _boardPlane = new Plane(boardNormal, boardWorldPos);
        _mainCamera = Camera.main;

        OnTouchBoard += touchBoardCallback;
        OnSlideBoard += slideBoardCallback;
    }

    public void AdjustSlideMinDistance()
    {
        _slideMinDist = Screen.height * ((Define.GRID_WORLD_WIDTH * 0.85f) / (_mainCamera.orthographicSize * 2f));
    }

    public void Update()
    {
        if (_isLock)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (OnTouchBoard != null)
                OnTouchBoard(GetTouchWorldPos());

            _touchPos = Input.mousePosition;
        }
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    if (_touchPos == null)
        //        return;
        //
        //    if (OnSlideBoard != null)
        //        OnSlideBoard(GridPosition.DIRECTION.NONE);
        //
        //    _touchPos = null;
        //}
        //else if (Input.GetMouseButton(0))
        //{
        //    if (_touchPos == null)
        //        return;
        //
        //    Vector3 touchStartPos = (Vector3)_touchPos;
        //    Vector3 curTouchPos = Input.mousePosition;
        //
        //    Vector3 delta = curTouchPos - touchStartPos;
        //
        //    //두 포인트의 거리가 필요거리 미만인 경우 
        //    if (delta.sqrMagnitude < Mathf.Pow(_slideMinDist, 2f))
        //        return;
        //
        //    float angle = Vector3.SignedAngle(Vector3.up, delta, Vector3.back);
        //
        //    GridPosition.DIRECTION dir = GridPosition.DIRECTION.NONE;
        //
        //    if ((angle <= 0 && angle > -45f) || (angle >= 0 && angle < 45f))
        //        dir = GridPosition.DIRECTION.UP;
        //    else if (angle >= 45f && angle <= 135f)
        //        dir = GridPosition.DIRECTION.RIGHT;
        //    else if ((angle > 135f && angle <= 180f) || (angle < -135f && angle >= -180f))
        //        dir = GridPosition.DIRECTION.DOWN;
        //    else
        //        dir = GridPosition.DIRECTION.LEFT;
        //
        //    if (OnSlideBoard != null)
        //        OnSlideBoard(dir);
        //
        //    _touchPos = null;
        //}
    }

    private Vector3 GetTouchWorldPos()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        if (_boardPlane.Raycast(ray, out rayDistance))
            return ray.GetPoint(rayDistance);

        return Vector3.zero;
    }

//    private void OnGUI()
//    {
//#if UNITY_EDITOR
//        GUIStyle style = new GUIStyle();

//        Rect rect = new Rect(Screen.width - 200, 20, 200, Screen.height * 0.4f);
//        style.alignment = TextAnchor.UpperLeft;
//        style.fontSize = (int)(rect.height * 0.05f);
//        style.normal.textColor = Color.black;

//        string text = string.Format("mouse : {0}", Input.mousePosition);
//        GUI.Label(rect, text, style);
//#endif
//    }
}
