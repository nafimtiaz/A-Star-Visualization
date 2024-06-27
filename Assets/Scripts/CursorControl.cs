using System;
using Unity.VisualScripting;
using UnityEngine;

public class CursorControl : MonoBehaviour
{
    [SerializeField] private AppManager appManager;
    [SerializeField] private Camera mainCam;
    [SerializeField] private LayerMask floorMask;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float cursorHeight;
    [SerializeField] private MeshRenderer cursorImage;

    private Ray _ray;
    private RaycastHit _rayHit;
    private Vector3 _hitPoint;
    private Vector3 _worldPoint;
    public bool cursorHittingFloor;
    public bool canPlace;

    public Vector3 CurrentCursorPoint => _hitPoint;

    private void Awake()
    {
        _ray = new Ray();
        _rayHit = new RaycastHit();
    }

    private void FixedUpdate()
    {
        cursorImage.enabled = canPlace;

        if (canPlace)
        {
            UpdateCursorPosition();
        }
    }

    private void UpdateCursorPosition()
    {
        var position = mainCam.transform.position;
        _worldPoint = mainCam.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            position.y));
        _ray.origin = position;
        _ray.direction = (_worldPoint - _ray.origin).normalized;

        if (Physics.Raycast(_ray, out _rayHit, Mathf.Infinity, floorMask))
        {
            cursorHittingFloor = true;
            _hitPoint = RoundWorldPoint(_rayHit.point);
            _hitPoint.y = cursorHeight;
            transform.position = _hitPoint;
        }
        else
        {
            cursorHittingFloor = false;
        }
    }

    private Vector3 RoundWorldPoint(Vector3 pos)
    {
        return new Vector3(Mathf.Floor(pos.x), 0f,  Mathf.Floor(pos.z));
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RequestAddObstacle();
        }
        
        if (Input.GetMouseButton(1))
        {
            RequestRemoveObstacle();
        }
    }

    private void RequestRemoveObstacle()
    {
        Vector3 origin = mainCam.transform.position;
        Vector3 dir = ((mainCam.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            origin.y))) - origin).normalized;

        RaycastHit hitInfo;
        if (Physics.Raycast(origin, dir,out hitInfo, Mathf.Infinity,obstacleMask))
        {
            appManager.RemoveObstacle(hitInfo.point);
        }
    }

    private void RequestAddObstacle()
    {
        if (cursorHittingFloor)
        {
            _hitPoint.y = 0;
            appManager.AddObstacle(_hitPoint);
        }
    }
}
