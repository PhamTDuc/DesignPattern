using UnityEngine;

namespace Guinea.Core.Components
{
    public class GridPlacement<T>
    {
        int m_subX;
        int m_subY;
        float m_cellSize;
        Transform m_parent;

        T[,] m_gridArray;

        public GridPlacement(Transform parent, int width, int height, float cellSize = 1.0f)
        {
            m_parent = parent;
            m_subX = width;
            m_subY = height;
            m_cellSize = cellSize;
            m_gridArray = new T[width, height];
            DrawGridDebug();
        }

        public Vector3 GetCenterLocal(int x, int y)
        {
            return m_cellSize * new Vector3(x + 0.5f, 0f, y + 0.5f);
        }

        public Vector3 GetCenterWorld(int x, int y)
        {
            return m_parent.position + m_parent.rotation * GetCenterLocal(x, y);
        }

        private Vector3 GetLocalPosition(int x, int y)
        {
            return m_cellSize * new Vector3(x, 0f, y);
        }

        private Vector3 GetWorldPosition(int x, int y)
        {
            return m_parent.position + m_parent.rotation * GetLocalPosition(x, y);
        }

        public T GetData(int x, int y)
        {
            return m_gridArray[x, y];
        }

        public T GetData(Vector3 worldPosition)
        {
            Vector2Int indices = GetIndicesOnGrid(worldPosition);
            return m_gridArray[indices.x, indices.y];
        }

        public void SetData(int x, int y, T data)
        {
            m_gridArray[x, y] = data;
        }

        public void SetData(Vector3 worldPosition, T data)
        {
            Vector2Int indices = GetIndicesOnGrid(worldPosition);
            m_gridArray[indices.x, indices.y] = data;
        }

        public Vector2Int GetIndicesOnGrid(Vector3 worldPosition)
        {
            Vector3 localPosition = Quaternion.Inverse(m_parent.rotation) * (worldPosition - m_parent.position);
            int x = Mathf.FloorToInt(localPosition.x / m_cellSize - 1);
            int y = Mathf.FloorToInt(localPosition.z / m_cellSize - 1);
            Vector2Int indices = new Vector2Int(x, y);
            indices.Clamp(new Vector2Int(0, 0), new Vector2Int(m_subX - 1, m_subY - 1));
            return indices;
        }

        private void DrawGridDebug()
        {
            for (int i = 0; i < m_gridArray.GetLength(0); i++)
            {
                for (int j = 0; j < m_gridArray.GetLength(1); j++)
                {
                    Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i, j + 1), Color.red, 100f, false);
                    Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i + 1, j), Color.red, 100f, false);
                }
            }
            Vector3 topRight = GetWorldPosition(m_gridArray.GetLength(0), m_gridArray.GetLength(1));
            Debug.DrawLine(GetWorldPosition(0, m_gridArray.GetLength(1)), topRight, Color.red, 100f, false);
            Debug.DrawLine(topRight, GetWorldPosition(m_gridArray.GetLength(0), 0), Color.red, 100f, false);
        }
    }
}