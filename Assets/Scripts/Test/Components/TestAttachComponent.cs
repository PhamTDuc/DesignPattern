using UnityEngine;
using UnityEngine.InputSystem;
using Guinea.Core.Components;


namespace Guinea.Test
{
    public class TestAttachComponent : MonoBehaviour
    {
#if DEVELOPMENT
        [SerializeField] LayerMask m_layer;
        [SerializeField] Transform m_reference;
        [SerializeField] GameObject m_prefab;
        [Tooltip("Get centerPoint or edgePoint on grid")]
        [SerializeField] bool m_isCentered;
        Camera m_camera;
        InputAction click;

        void Awake()
        {
            m_camera = Camera.main;
            click = new InputAction(binding: "<Mouse>/leftButton");
            click.performed += OnLeftButtonClick;
        }

        void OnEnable()
        {
            click.Enable();
        }

        void OnDisable()
        {
            click.Disable();
        }

        void OnLeftButtonClick(InputAction.CallbackContext context)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            mousePos.z = m_camera.nearClipPlane;
            Ray ray = m_camera.ScreenPointToRay(mousePos);
            Debug.DrawRay(m_camera.ScreenToWorldPoint(mousePos), m_camera.transform.forward, Color.red, 1f, false);
            if (Physics.Raycast(ray, out RaycastHit hit, 100, m_layer))
            {
                ComponentPlacement component = hit.collider.GetComponent<ComponentPlacement>();
                if (component != null)
                {
                    Vector2Int indices = component.GetIndicesOnGrid(hit.point);
                    Debug.Log($"Indices: {indices}");
                    Vector3 pos = component.GetWorldPosition(hit.point, m_isCentered);
                    // Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    // Instantiate(m_prefab, pos, Quaternion.identity, m_reference);
                    Instantiate(m_prefab, pos, component.Rotation, m_reference);
                }
            }
        }
#endif
    }
}