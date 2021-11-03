using UnityEngine;
using Guinea.Core.Components;
using Guinea.Core.Interactions;

namespace Guinea.Test
{
    [RequireComponent(typeof(ComponentSelection))]
    public class TestComponentSelection : MonoBehaviour
    {
#if DEVELOPMENT
        [SerializeField] LayerMask m_componentPlacement;
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
            m_componentSelection.OnReSelected += ctx => m_componentSelection.Cancel();
        }

        void OnDisable()
        {
            m_componentSelection.OnObjectSelected -= ObjectSelected;
            m_componentSelection.OnObjectDeselected -= ObjectDeselected;
            m_componentSelection.OnExecute -= HasObjectSelected;
            m_componentSelection.OnReSelected -= ctx => m_componentSelection.Cancel();
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
            if (Physics.Raycast(ray, out RaycastHit hit, 100, m_componentPlacement))
            {
                ComponentPlacement componentPlacement = hit.collider.GetComponent<ComponentPlacement>();

                // *Only attach when has ComponentPlacement and this not is a child of current selected object
                if (componentPlacement != null && !hit.collider.transform.IsChildOf(context.selected))
                {
                    context.selected.position = componentPlacement.GetWorldPosition(hit.point, true);
                    context.selected.rotation = componentPlacement.Rotation;
                    return;
                }
            }
            context.selected.rotation = context.prevRot;
            context.selected.position = context.GetObjectFollowMousePosition();
        }
#endif
    }
}