using UnityEngine;

public class CursorControl : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private LayerMask floorMask;
    [SerializeField] private float cursorHeight;
    
    private Ray _ray;
    private RaycastHit _rayHit;
    private Vector3 _hitPoint;
    private Vector3 _worldPoint;

    public Vector3 CurrentCursorPoint => _hitPoint;

    private void Awake()
    {
        _ray = new Ray();
        _rayHit = new RaycastHit();
    }

    private void FixedUpdate()
    {
        _worldPoint = mainCam.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, 
            Input.mousePosition.y, 
            mainCam.nearClipPlane));
        _ray.origin = mainCam.transform.position;
        _ray.direction = (_worldPoint - _ray.origin).normalized;

        if (Physics.Raycast(_ray, out _rayHit, Mathf.Infinity, floorMask))
        {
            _hitPoint = RoundWorldPoint(_rayHit.point);
            _hitPoint.y = cursorHeight;
            transform.position = _hitPoint;
        }
    }

    private Vector3 RoundWorldPoint(Vector3 pos)
    {
        return new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
    }
}
