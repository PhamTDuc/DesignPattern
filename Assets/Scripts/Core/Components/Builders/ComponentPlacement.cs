using UnityEngine;

namespace Guinea.Core.Components
{
    ///<summary>
    ///<para>ComponentPlacement is used to determine where other peripherals attached to</para>
    ///<para>Please notice that this class will NOT work PROPERLY when any parent of this has scale other than Vector3(1f, 1f, 1f)</para>
    ///</summary>
    public class ComponentPlacement : MonoBehaviour
    {
        [SerializeField] Vector2Int m_subdivisions;
        [SerializeField] float m_cellSize;
        [SerializeField] float m_lineWidth;

        Material m_gridMaterial;
        static int s_subdivisionsID = Shader.PropertyToID("Subdivisions");
        static int s_lineWidthID = Shader.PropertyToID("LineWidth");

        public Quaternion Rotation { get => transform.rotation; }
        void OnValidate()
        {
            Vector2 scaleFactor = m_subdivisions;
            scaleFactor *= m_cellSize;
            transform.localScale = new Vector3(scaleFactor.x, 1f, scaleFactor.y);
            m_gridMaterial = GetComponent<MeshRenderer>().sharedMaterial;
            if (m_gridMaterial != null)
            {
                m_gridMaterial.SetVector(s_subdivisionsID, new Vector4(m_subdivisions.x, m_subdivisions.y, 0f, 0f));
                m_gridMaterial.SetFloat(s_lineWidthID, m_lineWidth);
            }
        }

        public Vector2Int GetIndicesOnGrid(Vector3 worldPosition)
        {
            Vector3 localPosition = Quaternion.Inverse(transform.rotation) * (worldPosition - transform.position);
            int x = Mathf.FloorToInt(localPosition.x / m_cellSize);
            int y = Mathf.FloorToInt(localPosition.z / m_cellSize);
            Vector2Int indices = new Vector2Int(x, y);
            indices.Clamp(new Vector2Int(0, 0), new Vector2Int(m_subdivisions.x - 1, m_subdivisions.y - 1));
            return indices;
        }

        public Vector3 GetLocalPosition(Vector3 worldPosition, bool isCentered = true)
        {
            float centerFactor = isCentered ? 0.5f : 0f;
            Vector2Int indices = GetIndicesOnGrid(worldPosition);
            return new Vector3((indices.x + centerFactor) * m_cellSize, 0f, (indices.y + centerFactor) * m_cellSize);
        }

        public Vector3 GetLocalPositionToReference(Vector3 worldPosition, Transform reference, bool isCentered = true)
        {
            Vector3 localPosition = GetLocalPosition(worldPosition, isCentered);
            return transform.position - reference.position + localPosition;
        }

        public Vector3 GetWorldPosition(Vector3 worldPosition, bool isCentered = true)
        {
            return transform.position + transform.rotation * GetLocalPosition(worldPosition, isCentered);
        }
    }
}