using UnityEngine;

namespace Guinea.Core.Components
{
    [RequireComponent(typeof(MeshCollider))]
    public class Placement : MonoBehaviour
    {
        [SerializeField] Vector2Int m_subdivisions;
        [SerializeField] float m_cellSize;
        [SerializeField] AttachType m_attachType;
        public Vector2Int Subdivisions => m_subdivisions;
        public float CellSize => m_cellSize;
        public Quaternion Rotation => transform.rotation;
        public AttachType AttachType => m_attachType;
        void OnValidate()
        {
            Vector2 scaleFactor = m_subdivisions;
            scaleFactor *= m_cellSize;
            transform.localScale = new Vector3(scaleFactor.x, 1f, scaleFactor.y);
        }

        private Vector2Int GetIndicesOnGrid(Vector3 worldPosition)
        {
            Vector3 localPosition = Quaternion.Inverse(transform.rotation) * (worldPosition - transform.position);
            int x = m_subdivisions.x % 2 == 0 ? Utils.CeilingMaxMagnitude(localPosition.x / m_cellSize) : Utils.FloorMinMagnitude(localPosition.x / m_cellSize);
            int y = m_subdivisions.y % 2 == 0 ? Utils.CeilingMaxMagnitude(localPosition.z / m_cellSize) : Utils.FloorMinMagnitude(localPosition.z / m_cellSize);
            return new Vector2Int(x, y);
        }

        public Vector3 GetLocalPosition(Vector3 worldPosition)
        {
            Vector2 indices = GetIndicesOnGrid(worldPosition);
            indices.x += m_subdivisions.x % 2 == 0 ? -0.5f * Mathf.Sign(indices.x) : 0f;
            indices.y += m_subdivisions.y % 2 == 0 ? -0.5f * Mathf.Sign(indices.y) : 0f;
            return new Vector3(indices.x * m_cellSize, 0f, indices.y * m_cellSize);
        }

        public Vector3 GetLocalPositionToReference(Vector3 worldPosition, Transform reference)
        {
            Vector3 localPosition = GetLocalPosition(worldPosition);
            return transform.position + localPosition - reference.position;
        }

        public Vector3 GetWorldPosition(Vector3 worldPosition)
        {
            return transform.position + transform.rotation * GetLocalPosition(worldPosition);
        }
    }
}