using Cinemachine;
using UnityEngine;
using Sirenix.OdinInspector;

namespace CameraCustom
{
    [RequireComponent(typeof(BoxCollider))]
    public class ChangeCamera : MonoBehaviour
    {
        [SerializeField, Required] private GameObject _camera;

        private BoxCollider _collider;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                CinemachineVirtualCamera activeCamera = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera;
                activeCamera.gameObject.SetActive(false);

                _camera.SetActive(true);

                Debug.Log("Camera On");
            }
        }

    }
}
