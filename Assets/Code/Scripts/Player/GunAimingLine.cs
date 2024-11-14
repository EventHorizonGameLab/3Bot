using PlayerSM;
using UnityEngine;

public class GunAimingLine : MonoBehaviour
{
    [SerializeField] private Transform gunTip;
    [SerializeField] private LayerMask excludeLayers;
    [SerializeField] private bool _is2D = false;

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
        Vector3 startPoint = gunTip.position;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float maxDistance = 500f;

        if (_is2D)
        {
            ray = new Ray(new Vector3(gunTip.position.x, ray.origin.y, gunTip.position.z), new Vector3(ray.direction.x, 0, ray.direction.z).normalized);
        }


        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, ~excludeLayers))
        {
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(1, ray.GetPoint(maxDistance));
        }

        lineRenderer.SetPosition(0, startPoint);
    }

    private void CheckState(string state)
    {
        lineRenderer.enabled = state.ToLower().Contains("shoot");
    }
}
