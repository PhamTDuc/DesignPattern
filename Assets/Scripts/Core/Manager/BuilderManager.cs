using UnityEngine;
using Guinea.Core.Components;
using Guinea.Core.Interactions;
using System;

namespace Guinea.Core
{
    [RequireComponent(typeof(ComponentSelection))]
    public class BuilderManager : MonoBehaviour
    {
        [SerializeField] LayerMask m_placement;
        [SerializeField] GameObject m_gridView;
        [SerializeField] float m_lineWidth;
        [SerializeField] Material m_selectedMaterial;
        Material m_prevMaterial; // * Original material of selected object
        ComponentSelection m_componentSelection;
        Placement m_cachePlacement;

        static int s_subdivisionsID = Shader.PropertyToID("Subdivisions");
        static int s_lineWidthID = Shader.PropertyToID("LineWidth");
        void Awake()
        {
            m_componentSelection = GetComponent<ComponentSelection>();
            m_gridView.SetActive(false);
        }

        void OnEnable()
        {

            m_componentSelection.OnObjectSelected += ObjectSelected;
            m_componentSelection.OnObjectDeselected += ObjectDeselected;
            m_componentSelection.OnExecute += OnExecute;
            m_componentSelection.OnReSelected += OnObjectReSelected;
        }

        void OnDisable()
        {
            m_componentSelection.OnObjectSelected -= ObjectSelected;
            m_componentSelection.OnObjectDeselected -= ObjectDeselected;
            m_componentSelection.OnExecute -= OnExecute;
            m_componentSelection.OnReSelected -= OnObjectReSelected;
        }

        private void ObjectSelected(ComponentSelection.Context context)
        {
            Renderer renderer = context.selected.GetComponent<MeshRenderer>();
            m_prevMaterial = renderer.material;
            renderer.material = m_selectedMaterial;
        }

        private void ObjectDeselected(ComponentSelection.Context context)
        {
            m_gridView.gameObject.SetActive(false);
            Renderer renderer = context.selected.GetComponent<MeshRenderer>();
            renderer.material = m_prevMaterial;
            context.selected.position = context.prevPos;
        }

        private void OnObjectReSelected(ComponentSelection.Context context)
        {
            m_gridView.gameObject.SetActive(false);
            Renderer renderer = context.selected.GetComponent<MeshRenderer>();
            renderer.material = m_prevMaterial;
        }

        private void OnExecute(ComponentSelection.Context context)
        {
            Ray ray = m_componentSelection.GetRayFromMousePosition();
            if (Physics.Raycast(ray, out RaycastHit hit, 100, m_placement))
            {
                Placement placement = hit.collider.GetComponent<Placement>();

                if (placement != null && placement != m_cachePlacement && !placement.transform.IsChildOf(context.selected))
                {
                    m_cachePlacement = placement;
                    MatchGridViewToCurrentPlacement(m_cachePlacement);
                    m_gridView.gameObject.SetActive(true);
                }

                // *Only attach when has ComponentPlacement and this not is a child of current selected object
                if (m_cachePlacement != null)
                {
                    context.selected.position = m_cachePlacement.GetWorldPosition(hit.point);
                    context.selected.rotation = m_cachePlacement.Rotation;
                }
            }
            else
            {
                m_gridView.gameObject.SetActive(false);
                m_cachePlacement = null;
                context.selected.rotation = context.prevRot;
                context.selected.position = context.GetObjectFollowMousePosition();
            }
        }

        private void MatchGridViewToCurrentPlacement(Placement placement)
        {
            m_gridView.transform.SetPositionAndRotation(placement.transform.position, placement.transform.rotation);
            m_gridView.transform.position += m_gridView.transform.up * 0.001f;
            Vector2 scaleFactor = placement.Subdivisions;
            scaleFactor *= placement.CellSize;
            m_gridView.transform.localScale = new Vector3(scaleFactor.x, 1f, scaleFactor.y);
            Material material = m_gridView.GetComponent<MeshRenderer>().sharedMaterial;
            material.SetVector(s_subdivisionsID, new Vector4(placement.Subdivisions.x, placement.Subdivisions.y, 0f, 0f));
            material.SetFloat(s_lineWidthID, m_lineWidth);
        }
    }
}