using PlayerSM;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GunAimingLine : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private Transform _gunTip;

    [Title("Settings")]
    [SerializeField, Min(0)] private float _maxDistance = 500f;
    [SerializeField] private LayerMask excludeLayers;
    [SerializeField] private bool _is2D = false;

    [Title("Debug")]
    [SerializeField] private bool _debug = false;
    [SerializeField] private bool _drawGizmo = false;

    private LineRenderer lineRenderer;
    private Vector3 endPoint;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    private void OnEnable()
    {
        ShootingState.OnBodyState += CheckState;
    }

    private void OnDisable()
    {
        ShootingState.OnBodyState -= CheckState;
    }

    private void Update()
    {
        if (lineRenderer.enabled)
        {
            DrawLine();
        }
    }

    private void DrawLine()
    {
        Vector3 startPoint = _gunTip.position;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (_drawGizmo) Debug.DrawRay(Camera.main.transform.position, mouseRay.direction * _maxDistance * 2, Color.red);

        // Perform the first raycast
        endPoint = (Physics.Raycast(mouseRay, out RaycastHit firstHit, _maxDistance, ~excludeLayers))
            ? CalculateImpactPoint(startPoint, firstHit.point)
            : GetRayIntersectionWithPlane(mouseRay, _gunTip.position.y);

        // Now cast the second detectionRay in the calculated direction or no-impact point
        Ray secondRay = new(startPoint, (endPoint - startPoint).normalized);

        if (_drawGizmo) Debug.DrawRay(startPoint, secondRay.direction * _maxDistance, Color.cyan);

        if (Physics.Raycast(secondRay, out RaycastHit secondHit, _maxDistance, ~excludeLayers))
        {
            endPoint = secondHit.point;
        }
        else
        {
            endPoint = startPoint + secondRay.direction * _maxDistance;
        }

        if (_drawGizmo) Debug.DrawRay(endPoint, new Vector3(endPoint.x, -_maxDistance, endPoint.z), Color.green);

        _gunTip.LookAt(endPoint);

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }

    private Vector3 CalculateImpactPoint(Vector3 startPoint, Vector3 firstHitPoint)
    {
        Vector3 direction = (firstHitPoint - startPoint).normalized;
        if (_is2D) direction.y = 0;
        return NoImpact(startPoint, direction);
    }

    private Vector3 NoImpact(Vector3 startPoint, Vector3 direction)
    {
        Ray secondRay = new(startPoint, direction);
        if (Physics.Raycast(secondRay, out RaycastHit secondHit, _maxDistance, ~excludeLayers))
        {
            return secondHit.point;
        }
        else
        {
            return startPoint + direction * _maxDistance;
        }
    }

    // Get intersection of the detectionRay with a horizontal plane at the given height (y-coordinate)
    private Vector3 GetRayIntersectionWithPlane(Ray ray, float planeHeight)
    {
        if (Mathf.Approximately(ray.direction.y, 0f)) return Vector3.zero;

        float t = (planeHeight - ray.origin.y) / ray.direction.y;
        if (t < 0) return Vector3.zero;

        return ray.origin + t * ray.direction;
    }

    private void CheckState(bool state)
    {
        lineRenderer.enabled = state;
    }

    #region Gizmos

    private void OnDrawGizmos()
    {
        if (_drawGizmo)
        {
            DrawPlane(_gunTip.position.y);
        }

        if (_debug)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(Camera.main.transform.position, _maxDistance * 2 * mouseRay.direction);
        }
    }

    private void DrawPlane(float planeHeight)
    {
        float size = 10f;

        Gizmos.color = Color.green;

        for (float i = -size; i <= size; i += 1f)
        {
            Gizmos.DrawLine(new Vector3(i, planeHeight, -size), new Vector3(i, planeHeight, size));
            Gizmos.DrawLine(new Vector3(-size, planeHeight, i), new Vector3(size, planeHeight, i));
        }
    }

    #endregion

}
