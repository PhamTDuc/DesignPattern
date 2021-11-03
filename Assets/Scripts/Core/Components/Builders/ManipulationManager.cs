using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Zenject;

namespace Guinea.Core.Components
{
    public class ManipulationManager : MonoBehaviour
    {
        [Tooltip("Tag for better refine selected object")]
        [SerializeField] string m_tag;
        [Tooltip("Layer that you want to implement the selection functionality")]
        [SerializeField] LayerMask m_selectionLayer;
        [SerializeField] float m_raycastLength;
        [Tooltip("Manipulator Component")]
        [SerializeField] Manipulator m_manipulator; // * Current Operator to manipulate
        [SerializeField] LayerMask m_operatorLayer;
        Context m_currentContext = new Context();
        IOperator m_operator;
        IOperator.Result m_currentOperatorResult = IOperator.Result.NO_OPERATOR;
        ManipulatorHandle m_currentHandle = null;
        Camera m_camera;

        SharedInteraction m_sharedInteraction;


        [Inject]
        void Initialize(SharedInteraction sharedInteraction)
        {
            m_sharedInteraction = sharedInteraction;
        }

        void Awake()
        {
            m_camera = Camera.main;
            // m_operator = m_manipulator;
            m_operator = GetComponent<MoveOperator>();
        }

        void OnEnable()
        {
            InputManager.Map.ComponentBuilder.Select.performed += OnSelect;
        }

        void OnDisable()
        {
            InputManager.Map.ComponentBuilder.Select.performed -= OnSelect;
        }

        void Update()
        {
            Vector3 mousePos = InputManager.Map.ComponentBuilder.Point.ReadValue<Vector2>();
            mousePos.z = m_camera.nearClipPlane;
            Ray ray = m_camera.ScreenPointToRay(mousePos);

            if (m_currentOperatorResult != IOperator.Result.NO_OPERATOR)
            {
                if (m_currentOperatorResult != IOperator.Result.INVOKE) // * Do not handle Input in INVOKE state
                {
                    HandleOperatorInput();
                }
                OnActionTriggered();
            }
            else
            {
                HandleManipulator(ray);
            }
        }

        private void HandleOperatorInput()
        {
            // if (EventSystem.current.IsPointerOverGameObject())
            // {
            //     return;
            // }

            if (InputManager.Map.Operator.Submit.triggered)
            {
                m_currentOperatorResult = IOperator.Result.FINISHED;
            }

            if (InputManager.Map.Operator.Cancel.triggered)
            {
                m_currentOperatorResult = IOperator.Result.CANCELLED;
            }
        }

        private void OnSelect(InputAction.CallbackContext context)
        {
            // if (EventSystem.current.IsPointerOverGameObject())
            // {
            //     return;
            // }
            if (m_currentOperatorResult != IOperator.Result.NO_OPERATOR)
            {
                return;
            }

            Vector3 mousePos = InputManager.Map.ComponentBuilder.Point.ReadValue<Vector2>();
            mousePos.z = m_camera.nearClipPlane;
            Ray ray = m_camera.ScreenPointToRay(mousePos);

            OnSelectCallback(ray);
        }

        // Handle Manipulator of current m_operator if exists
        private void HandleManipulator(Ray ray)
        {
            if (m_currentHandle != null && InputManager.Map.ComponentBuilder.Select.triggered)
            {
                m_currentOperatorResult = IOperator.Result.INVOKE;
                return;
            }

            if (m_currentContext.@object != null && Physics.Raycast(ray, out RaycastHit hit, m_raycastLength, m_operatorLayer))
            {
                ManipulatorHandle handle = hit.collider.GetComponent<ManipulatorHandle>();
                if (handle != null)
                {
                    m_currentHandle?.OnPointerExit();
                    m_currentHandle = handle;
                    m_currentHandle?.OnPointerEnter();
                    Manipulator manipulator = m_operator as Manipulator;
                    if (manipulator != null)
                    {
                        manipulator.Handle = m_currentHandle.Handle;
                        manipulator.Space = m_currentHandle.Space;
                    }
                    return;
                }
            }
            m_currentHandle?.OnPointerExit();
            m_currentHandle = null;
            return;
        }

        private void OnSelectCallback(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, m_raycastLength, m_selectionLayer))
            {
                if (!String.IsNullOrEmpty(m_tag) && hit.transform.CompareTag(m_tag) || String.IsNullOrEmpty(m_tag))
                {
                    SelectObject(hit.transform);
                }
            }

            Debug.Log("Call OnSelectCallback!");
        }

        private void OnActionTriggered()
        {
            switch (m_currentOperatorResult)
            {
                case IOperator.Result.INVOKE:
                    m_currentOperatorResult = m_operator.Invoke(m_currentContext);
                    InputManager.AddBackKeyEvent(OnCancelManipulator);
                    InputManager.Map.Operator.Enable();
                    break;
                case IOperator.Result.RUNNING_MODAL:
                    m_currentOperatorResult = m_operator.Execute(m_currentContext);
                    break;
                case IOperator.Result.FINISHED:
                    m_operator?.Finish(m_currentContext);
                    m_currentOperatorResult = IOperator.Result.NO_OPERATOR;
                    InputManager.RemoveBackKeyEvent(OnCancelManipulator);
                    InputManager.Map.Operator.Disable();
                    break;
                case IOperator.Result.CANCELLED:
                    m_operator?.Cancel(m_currentContext);
                    m_currentOperatorResult = IOperator.Result.NO_OPERATOR;
                    InputManager.RemoveBackKeyEvent(OnCancelManipulator);
                    InputManager.Map.Operator.Disable();
                    break;
                case IOperator.Result.PASS_THROUGH:
                    break;
                default:
                    break;
            }
        }

        private void OnObjectDeslect()
        {
            if (m_currentContext.@object != null)
            {
                MeshRenderer renderer = m_currentContext.@object.GetComponent<MeshRenderer>();
                List<Material> materials = renderer.sharedMaterials.ToList();
                materials.Remove(m_sharedInteraction.SelectedMaterial);
                renderer.sharedMaterials = materials.ToArray();
            }
        }

        private void OnObjectSelect()
        {
            if (m_currentContext.@object != null)
            {
                MeshRenderer renderer = m_currentContext.@object.GetComponent<MeshRenderer>();
                List<Material> materials = renderer.sharedMaterials.ToList();
                if (!materials.Contains(m_sharedInteraction.SelectedMaterial))
                {
                    materials.Add(m_sharedInteraction.SelectedMaterial);
                    renderer.sharedMaterials = materials.ToArray();
                }
            }
        }

        public void SelectObject(Transform trans)
        {
            Manipulator manipulator = m_operator as Manipulator;
            if (trans != m_currentContext.@object)
            {
                OnObjectDeslect();
                m_currentContext.@object = trans;
                OnObjectSelect();
                if (manipulator != null)
                {
                    manipulator.transform.position = m_currentContext.@object.transform.position;
                    manipulator.gameObject.SetActive(true);
                }
                else
                {
                    Invoke(); // * Invoke IOperator immediately when it is not a Manipulator
                }
                InputManager.AddBackKeyEvent(OnResetContext);
            }
            else
            {
                if (manipulator == null)
                {
                    Invoke(); // * Invoke IOperator immediately when it is not a Manipulator
                }
            }
        }

        public void ResetContext()
        {
            OnObjectDeslect();
            m_currentContext.@object = null;
            Manipulator manipulator = m_operator as Manipulator;
            if (manipulator != null)
            {
                manipulator.gameObject.SetActive(false);
            }
        }

        public void SwitchOperator(IOperator op)
        {
            Manipulator manipulator = op as Manipulator;
            if (manipulator != null)
            {
                manipulator?.gameObject.SetActive(false);
                manipulator.gameObject.SetActive(false);
                if (m_currentContext.@object != null)
                {
                    manipulator.transform.SetPositionAndRotation(manipulator.transform.position, Quaternion.identity);
                    manipulator.gameObject.SetActive(true);
                    if (m_currentOperatorResult == IOperator.Result.RUNNING_MODAL)
                    {
                        m_operator = manipulator;
                        m_currentOperatorResult = IOperator.Result.INVOKE;
                        return;
                    }
                }
            }
            m_operator = op;
        }

        public void OnResetContext(InputAction.CallbackContext context)
        {
            ResetContext();
            InputManager.RemoveBackKeyEvent(OnResetContext);
        }

        public void OnCancelManipulator(InputAction.CallbackContext context)
        {
            m_currentOperatorResult = IOperator.Result.CANCELLED;
        }

        public void Invoke() => m_currentOperatorResult = IOperator.Result.INVOKE; // * Invoke Current Operator

    }
}