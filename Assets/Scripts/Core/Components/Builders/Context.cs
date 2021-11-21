using UnityEngine;

namespace Guinea.Core.Components
{
    // TODO: Check if m_object is destroyed when getting property @object
    public class Context
    {
        Transform m_object;
        public Transform @object => m_object;

        public void Select(Transform trans)
        {
            m_object = trans;
            Debug.Log($"Context::Select({m_object})");
        }
    }
}