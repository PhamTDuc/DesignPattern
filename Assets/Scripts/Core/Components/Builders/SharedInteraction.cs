using UnityEngine;


namespace Guinea.Core.Components
{
    public class SharedInteraction : MonoBehaviour
    {
        [SerializeField] GameObject m_gridView;
        [SerializeField] Material m_sharedObjectSelectedMaterial;
        [Range(0f, 1f)]
        [SerializeField] float m_lineWidth;
        Material m_gridViewMaterial;
        Frame m_root = null; // * Entity Root 
        public Material SelectedMaterial => m_sharedObjectSelectedMaterial;
        public Frame Root => m_root;

        static int s_subdivisionsID = Shader.PropertyToID("Subdivisions");
        static int s_lineWidthID = Shader.PropertyToID("LineWidth");
        static int s_roundnessID = Shader.PropertyToID("Roundness");
        static int s_colorID = Shader.PropertyToID("_Color");

        void Awake()
        {
            m_gridViewMaterial = m_gridView.GetComponent<MeshRenderer>().sharedMaterial;
            m_gridViewMaterial.SetFloat(s_lineWidthID, m_lineWidth);
            HideGridView();
        }

        public void ChangeSelectedColor(Color color) => m_sharedObjectSelectedMaterial.SetColor(s_colorID, color);

        public void HideGridView() => m_gridView.gameObject.SetActive(false);

        public void SetGridViewFromPlacement(Placement placement)
        {
            m_gridView.transform.SetPositionAndRotation(placement.transform.position, placement.transform.rotation);
            m_gridView.transform.position += m_gridView.transform.up * 0.001f;

            Vector2 scaleFactor = placement.Subdivisions;
            scaleFactor *= placement.CellSize;
            m_gridView.transform.localScale = new Vector3(scaleFactor.x, 1f, scaleFactor.y);
            m_gridViewMaterial.SetVector(s_subdivisionsID, new Vector4(placement.Subdivisions.x, placement.Subdivisions.y, 0f, 0f));

            float roundness;
            switch (placement.AttachType)
            {
                case AttachType.SQUARE:
                    roundness = 0.1f;
                    break;
                case AttachType.CIRCLE:
                    roundness = 0.5f;
                    break;
                default:
                    roundness = 0.05f;
                    break;
            }
            m_gridViewMaterial.SetFloat(s_roundnessID, roundness);

            m_gridView.gameObject.SetActive(true);
        }

        public void SetEntityRoot(ComponentBase component)
        {
            if (component != null && m_root == null)
            {
                Frame frame = component.GetComponent<Frame>();
                if (frame != null)
                {
                    m_root = frame;
                    Debug.Log($"SetEntityRoot:{frame.gameObject.name}");
                }
            }
        }

        public void ResetEntityRoot() => m_root = null;
    }
}