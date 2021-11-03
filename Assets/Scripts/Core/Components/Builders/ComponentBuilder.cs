using UnityEngine;
using Guinea.Core.Interactions;

namespace Guinea.Core.Components
{
    [RequireComponent(typeof(ComponentSelection))]
    public class ComponentBuilder : MonoBehaviour
    {
        // * Using LayerMask for more efficient when using RayCast
        [SerializeField] LayerMask m_gridPlacementLayer;
        [SerializeField] Material m_selectedMaterial;
        Material m_prevMaterial; // * Original material of selected object
        ComponentSelection m_componentSelection;

        #region Unity Callbacks
        void Awake()
        {
            m_componentSelection = GetComponent<ComponentSelection>();
        }

        void OnEnable()
        {

            m_componentSelection.OnObjectSelected += ObjectSelected;
            m_componentSelection.OnObjectDeselected += ObjectDeselected;
            m_componentSelection.OnExecute += HasObjectSelected;
        }

        void OnDisable()
        {
            m_componentSelection.OnObjectSelected -= ObjectSelected;
            m_componentSelection.OnObjectDeselected -= ObjectDeselected;
            m_componentSelection.OnExecute -= HasObjectSelected;
        }
        #endregion

        void ObjectSelected(ComponentSelection.Context context)
        {
            Renderer renderer = context.selected.GetComponent<MeshRenderer>();
            m_prevMaterial = renderer.material;
            renderer.material = m_selectedMaterial;
        }

        void ObjectDeselected(ComponentSelection.Context context)
        {
            Renderer renderer = context.selected.GetComponent<MeshRenderer>();
            renderer.material = m_prevMaterial;
        }

        void HasObjectSelected(ComponentSelection.Context context)
        {
            Ray ray = m_componentSelection.GetRayFromMousePosition();
            context.selected.position = context.offset + context.GetMouseWorldPosition();
        }
    }
}