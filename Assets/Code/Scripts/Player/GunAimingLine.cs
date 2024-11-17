using PlayerSM;
using Sirenix.OdinInspector;
using System.Net;
using Unity.VisualScripting;
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

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    private void OnEnable()
    {
        PlayerController.OnChangeState += CheckState;
    }

    private void OnDisable()
    {
        PlayerController.OnChangeState -= CheckState;
    }

    private void Update()
    {
        if (lineRenderer.enabled) DrawLine();
    }
    private void DrawLine()
    {
        Vector3 startPoint = _gunTip.position;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector3 endPoint;

        if (_debug) Debug.DrawRay(Camera.main.transform.position, mouseRay.direction * _maxDistance * 2, Color.red);

        if (Physics.Raycast(mouseRay, out RaycastHit firstHit, _maxDistance, ~excludeLayers))
        {
            Vector3 direction = (firstHit.point - startPoint).normalized;

            if (_is2D) direction.y = 0;

            Ray secondRay = new(startPoint, direction);

            if (_drawGizmo) Debug.DrawRay(startPoint, direction * _maxDistance, Color.cyan);

            if (Physics.Raycast(secondRay, out RaycastHit secondHit, _maxDistance, ~excludeLayers))
            {
                endPoint = secondHit.point;
            }
            else
            {
                endPoint = NoImpact(startPoint, direction);
            }
        }
        else
        {
            endPoint = GetRayIntersectionWithPlane(mouseRay, _gunTip.position.y);

            Vector3 direction = endPoint - startPoint;
            direction.Normalize();

            direction  = startPoint + direction * _maxDistance;

            Ray secondRay = new(startPoint, direction);
            if (Physics.Raycast(secondRay, out RaycastHit secondHit, _maxDistance, ~excludeLayers))
            {
                endPoint = secondHit.point;
            }
            else
            {
                endPoint = direction;
            }
        }

        if (_drawGizmo) Debug.DrawRay(endPoint, new(endPoint.x, -_maxDistance, endPoint.z), Color.green);

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }

    Vector3 NoImpact(Vector3 startPoint, Vector3 direction)
    {
        Vector3 endPoint;

        if (_is2D) direction.y = 0;

        Ray secondRay = new(startPoint, direction);
        if (Physics.Raycast(secondRay, out RaycastHit secondHit, _maxDistance, ~excludeLayers))
        {
            endPoint = secondHit.point;
        }
        else
        {
            endPoint = startPoint + direction * _maxDistance;
        }

        return endPoint;
    }

    private Vector3 GetRayIntersectionWithPlane(Ray ray, float planeHeight)
    {
        if (Mathf.Approximately(ray.direction.y, 0f)) return Vector3.zero;

        float t = (planeHeight - ray.origin.y) / ray.direction.y;

        if (t < 0) return Vector3.zero;

        Vector3 intersectionPoint = ray.origin + t * ray.direction;

        return intersectionPoint;
    }

    private void CheckState(string state)
    {
        lineRenderer.enabled = state.ToLower().Contains("shoot");
    }
}
