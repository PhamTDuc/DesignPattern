using UnityEngine;
using Guinea.Core.Components;

namespace Guinea.Test
{
    public class TestGridPlacement : MonoBehaviour
    {
#if DEVELOPMENT
        [SerializeField] Vector2Int m_subdivisions;
        [SerializeField] float m_gridSize;
        GridPlacement<int> m_grid;
        void Awake()
        {
            m_grid = new GridPlacement<int>(transform, m_subdivisions.x, m_subdivisions.y, m_gridSize);
        }

        void Start()
        {
            Debug.Log(m_grid.GetIndicesOnGrid(new Vector3(3f, 0, 3f)));
        }
#endif
    }

}