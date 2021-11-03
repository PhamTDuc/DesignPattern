using UnityEngine;

namespace Guinea.Test
{
    public class TestAttachBaseComponent : MonoBehaviour
    {
#if DEVELOPMENT
        [SerializeField] GameObject m_grid; // * Visual grid to know where to attach element to

#endif
    }
}