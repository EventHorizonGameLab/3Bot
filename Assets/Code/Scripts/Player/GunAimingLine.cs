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

        Plane plane = new (Vector3.up, new Vector3(0, _gunTip.position.y, 0));

        if (plane.Raycast(mouseRay, out float enter))
        {
            endPoint = mouseRay.GetPoint(enter);

            Ray secondRay = new(startPoint, (endPoint - startPoint).normalized);

            if (Physics.Raycast(secondRay, out RaycastHit secondHit, _maxDistance, ~excludeLayers))
            {
                endPoint = secondHit.point;
            }
            else
            {
                endPoint = startPoint + secondRay.direction * _maxDistance;
            }
        }
        else
        {
            endPoint = startPoint + mouseRay.direction * _maxDistance;
        }

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);

        _gunTip.LookAt(endPoint);
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

        float x, z;
        x = transform.position.x;
        z = transform.position.z;

        for (float i = -size; i <= size; i += 1f)
        {
            Gizmos.DrawLine(new Vector3(x + i, planeHeight, z - size), new Vector3(x + i, planeHeight, x + size));
            Gizmos.DrawLine(new Vector3(x - size, planeHeight, z + i), new Vector3(x + size, planeHeight, z + i));
        }
    }

    #endregion
}
