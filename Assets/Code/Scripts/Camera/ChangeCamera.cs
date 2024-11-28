using PlayerSM;
using Sirenix.OdinInspector;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField] private GameObject _camera;

    private void Start() { }

    private void OnEnable()
    {
        MovementState2.IsFollowingPath += SetActive;
    }

    private void OnDisable()
    {
        MovementState2.IsFollowingPath -= SetActive;
    }

    private void SetActive(bool active)
    {
        _camera.SetActive(!active);
    }
}
