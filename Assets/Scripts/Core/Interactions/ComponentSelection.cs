using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Guinea.Core.Interactions
{
    public class ComponentSelection : MonoBehaviour
    {
        [Tooltip("Tag for better refine selected object")]
        [SerializeField] string m_tag;
        [Tooltip("Layer that you want to implement the selection functionality")]
        [SerializeField] LayerMask m_selectionLayer;
        public event Action<Context> OnObjectSelected;
        public event Action<Context> OnReSelected;
        public event Action<Context> OnObjectDeselected;
        public event Action<Context> OnExecute;

        Camera m_camera;
        Context m_context = new Context();

        void Awake()
        {
            m_camera = Camera.main;
        }

        void OnEnable()
        {
            InputManager.Map.ComponentBuilder.Select.performed += OnSelect;
            // InputManager.Map.ComponentBuilder.DeSelect.performed += ctx => Cancel();
            InputManager.Map.ComponentBuilder.Point.performed += OnExecuteCallback;
            // InputManager.SetActionMap(InputManager.Map.ComponentBuilder, true);
        }

        void OnDisable()
        {
            InputManager.Map.ComponentBuilder.Select.performed -= OnSelect;
            // InputManager.Map.ComponentBuilder.DeSelect.performed -= ctx => Cancel();
            InputManager.Map.ComponentBuilder.Point.performed -= OnExecuteCallback;
        }

        void OnSelect(InputAction.CallbackContext context)
        {
            Ray ray = GetRayFromMousePosition();
            if (Physics.Raycast(ray, out RaycastHit hit, 100, m_selectionLayer))
            {
                Transform selection = hit.transform;
                Debug.Log($"OnSelect Invoke: {selection.name}");
                bool isNotTag = String.IsNullOrEmpty(m_tag);
                if (!isNotTag && selection.CompareTag(m_tag) || isNotTag)
                {
                    if (m_context.HasNotSelected(selection))
                    {
                        if (m_context.HasObjectSelect)
                        {
                            OnObjectDeselected?.Invoke(m_context);
                        }
                        m_context.OnSelect(selection);
                        InputManager.AddBackKeyEvent(BackKeyEvent);
                        OnObjectSelected?.Invoke(m_context);
                    }
                    else
                    {
                        OnReSelected?.Invoke(m_context);
                        Reset();
                    }
                    return;
                }

            }
            Cancel();
        }

        void OnExecuteCallback(InputAction.CallbackContext context)
        {
            if (m_context.HasObjectSelect)
            {
                OnExecute?.Invoke(m_context);
            }
        }

        void BackKeyEvent(InputAction.CallbackContext context) => Cancel();

        public void Cancel()
        {
            if (m_context.HasObjectSelect)
            {
                OnObjectDeselected?.Invoke(m_context);
            }
            Reset();
        }

        private void Reset()
        {
            m_context.Reset();
            InputManager.RemoveBackKeyEvent(BackKeyEvent);
        }

        public Ray GetRayFromMousePosition()
        {
            Vector3 mousePos = InputManager.Map.ComponentBuilder.Point.ReadValue<Vector2>();
            mousePos.z = m_camera.nearClipPlane;
            return m_camera.ScreenPointToRay(mousePos);
        }

        public void ResetAllEvents()
        {
            OnObjectSelected = null;
            OnReSelected = null;
            OnObjectDeselected = null;
            OnExecute = null;
        }

        public struct Context
        {
            public Transform selected;
            public Vector3 offset;

            public float z;
            public Quaternion prevRot;
            public Vector3 prevPos;
            public bool HasObjectSelect => selected != null;
            public bool HasNotSelected(Transform trans) => selected != trans;
            public Context(Transform selected, Vector3 offset, float z, Vector3 prevPos, Quaternion prevRot)
            {
                this.selected = selected;
                this.offset = offset;
                this.z = z;
                this.prevPos = prevPos;
                this.prevRot = prevRot;
            }

            public void OnSelect(Transform select)
            {
                this.selected = select;
                this.z = Camera.main.WorldToScreenPoint(select.position).z;
                this.offset = selected.position - GetMouseWorldPosition();
                this.prevPos = select.position;
                this.prevRot = select.rotation;
            }

            public Vector3 GetMouseWorldPosition()
            {
                Vector3 mousePos = Mouse.current.position.ReadValue();
                mousePos.z = this.z;
                return Camera.main.ScreenToWorldPoint(mousePos);
            }

            public Vector3 GetObjectFollowMousePosition()
            {
                return offset + GetMouseWorldPosition();
            }

            public void Reset()
            {
                selected = null;
                z = 0f;
                offset = Vector3.zero;
            }
        }
    }
}
