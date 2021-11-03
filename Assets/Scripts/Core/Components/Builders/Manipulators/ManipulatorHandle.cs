using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Guinea.Core.Components
{
    public class ManipulatorHandle : MonoBehaviour
    {
        [SerializeField] HandleType m_handleType;
        [SerializeField] SpaceType m_spaceType;
        [SerializeField] Material m_overlayMaterial;
        [SerializeField] Color m_color;

        public HandleType Handle => m_handleType;
        public SpaceType Space => m_spaceType;

        MeshRenderer m_renderer;

        static int s_colorID = Shader.PropertyToID("_Color");
        void Awake()
        {
            m_renderer = GetComponent<MeshRenderer>();
        }

        public void OnPointerEnter()
        {
            List<Material> materials = m_renderer.sharedMaterials.ToList();
            m_overlayMaterial.SetVector(s_colorID, m_color);
            materials.Add(m_overlayMaterial);
            m_renderer.sharedMaterials = materials.ToArray();
            // Debug.Log($"OnPointerEnter(): {gameObject.name}");
        }

        public void OnPointerExit()
        {
            List<Material> materials = m_renderer.sharedMaterials.ToList();
            materials.Remove(m_overlayMaterial);
            m_renderer.sharedMaterials = materials.ToArray();
            // Debug.Log($"OnPointerExit(): {gameObject.name}");
        }

        public enum HandleType
        {
            UP_DOWN = 0,
            ARBITARY,
        }
        public enum SpaceType
        {
            LOCAL_SPACE = 0,
            WORLD_SPACE,
        }
    }
}