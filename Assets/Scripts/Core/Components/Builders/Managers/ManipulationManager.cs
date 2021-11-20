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
        //// [Tooltip("Manipulator Component")] // TEST
        //// [SerializeField] MoveManipulator m_manipulator; // TEST
        [SerializeField] LayerMask m_operatorLayer;
        Context m_context;
        IOperator m_operator;
        IOperator.Result m_currentOperatorResult = IOperator.Result.NO_OPERATOR;
        ManipulatorHandle m_currentHandle = null;
        Camera m_camera;

        SharedInteraction m_sharedInteraction;
        OperatorManager m_operatorManager;


        [Inject]
        void Initialize(SharedInteraction sharedInteraction, OperatorManager operatorManager, Context context)
        {
            m_sharedInteraction = sharedInteraction;
            m_operatorManager = operatorManager;
            m_context = context;
        }

        void Awake()
        {
            m_camera = Camera.main;
            m_operator = m_operatorManager.Get<VerticalOperator>(); // * Set MoveOperator as default
            ResetContext();
        }

        void OnEnable()
        {
            InputManager.Map.EntityBuilder.Select.performed += OnSelect;
        }

        void OnDisable()
        {
            InputManager.Map.EntityBuilder.Select.performed -= OnSelect;
        }

        void Update()
        {
            Vector3 mousePos = InputManager.Map.EntityBuilder.Point.ReadValue<Vector2>();
            mousePos.z = m_camera.nearClipPlane;
            Ray ray = m_camera.ScreenPointToRay(mousePos);

            if (m_currentOperatorResult != IOperator.Result.NO_OPERATOR)
            {
                if (m_currentOperatorResult != IOperator.Result.INVOKE) // * Do not handle Input in INVOKE state
                {
                    HandleOperatorInput();
                }
                Tick();
            }
            else
            {
                HandleManipulator(ray);
            }
        }

        private void HandleOperatorInput()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

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
            if (m_currentOperatorResult != IOperator.Result.NO_OPERATOR || EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Vector3 mousePos = InputManager.Map.EntityBuilder.Point.ReadValue<Vector2>();
            mousePos.z = m_camera.nearClipPlane;
            Ray ray = m_camera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, m_raycastLength, m_selectionLayer))
            {
                if (!String.IsNullOrEmpty(m_tag) && hit.transform.CompareTag(m_tag) || String.IsNullOrEmpty(m_tag))
                {
                    if (hit.transform != m_context.@object)
                    {
                        Select(hit.transform);
                    }
                    else
                    {
                        InvokeCurrentOperator();
                    }
                }
            }
            Commons.Logger.Log("Call OnSelect()!");
        }

        // * Handle Manipulator of current m_operator if exists
        private void HandleManipulator(Ray ray)
        {
            if (m_currentHandle != null && InputManager.Map.EntityBuilder.Select.triggered)
            {
                InvokeCurrentOperator();
                return;
            }

            if (m_context.@object != null && Physics.Raycast(ray, out RaycastHit hit, m_raycastLength, m_operatorLayer))
            {
                ManipulatorHandle handle = hit.collider.GetComponent<ManipulatorHandle>();
                if (handle != null)
                {
                    m_currentHandle?.OnPointerExit();
                    m_currentHandle = handle;
                    m_currentHandle?.OnPointerEnter();
                    ////MoveManipulator manipulator = m_operator as MoveManipulator;
                    ////if (manipulator != null)
                    ////{
                    ////    manipulator.Handle = m_currentHandle.Handle;
                    ////    manipulator.Space = m_currentHandle.Space;
                    ////}
                    return;
                }
            }
            m_currentHandle?.OnPointerExit();
            m_currentHandle = null;
            return;
        }

        private void Tick()
        {
            switch (m_currentOperatorResult)
            {
                case IOperator.Result.INVOKE:
                    m_currentOperatorResult = m_operator.Invoke(m_context);
                    InputManager.AddBackKeyEvent(OnCancelManipulator);
                    InputManager.Map.Operator.Enable();
                    break;
                case IOperator.Result.RUNNING_MODAL:
                    m_currentOperatorResult = m_operator.Execute(m_context);
                    break;
                case IOperator.Result.FINISHED:
                    m_operator?.Finish(m_context);
                    m_currentOperatorResult = IOperator.Result.NO_OPERATOR;
                    InputManager.RemoveBackKeyEvent(OnCancelManipulator);
                    InputManager.Map.Operator.Disable();
                    break;
                case IOperator.Result.CANCELLED:
                    m_operator?.Cancel(m_context);
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
            if (m_context.@object != null)
            {
                MeshRenderer renderer = m_context.@object.GetComponent<MeshRenderer>();
                List<Material> materials = renderer.sharedMaterials.ToList();
                materials.Remove(m_sharedInteraction.SelectedMaterial);
                renderer.sharedMaterials = materials.ToArray();
            }
        }

        private void OnObjectSelect()
        {
            if (m_context.@object != null)
            {
                MeshRenderer renderer = m_context.@object.GetComponent<MeshRenderer>();
                List<Material> materials = renderer.sharedMaterials.ToList();
                if (!materials.Contains(m_sharedInteraction.SelectedMaterial))
                {
                    materials.Add(m_sharedInteraction.SelectedMaterial);
                    renderer.sharedMaterials = materials.ToArray();
                }
            }
        }

        public void Select(Transform trans)
        {
            MoveManipulator manipulator = m_operator as MoveManipulator;
            if (trans != m_context.@object)
            {
                OnObjectDeslect();
                m_context.@object = trans;
                OnObjectSelect();
                if (manipulator != null)
                {
                    manipulator.transform.position = m_context.@object.transform.position;
                    manipulator.gameObject.SetActive(true);
                }
                // else
                // {
                //     InvokeCurrentOperator(); // * InvokeCurrentOperator IOperator immediately when it is not a Manipulator
                // }
                InputManager.AddBackKeyEvent(OnResetContext);
            }
            // else
            // {
            //     if (manipulator == null)
            //     {
            //         InvokeCurrentOperator(); // * InvokeCurrentOperator IOperator immediately when it is not a Manipulator
            //     }
            // }
        }

        public void ResetContext()
        {
            OnObjectDeslect();
            m_context.@object = null;
            MonoBehaviour manipulator = m_operator as MonoBehaviour;
            if (manipulator != null)
            {
                manipulator.gameObject.SetActive(false);
            }
        }

        public void SwitchOperator(IOperator op)
        {
            MonoBehaviour manipulator = m_operator as MonoBehaviour;
            if (manipulator != null)
            {
                manipulator.gameObject.SetActive(false);
            }

            manipulator = op as MonoBehaviour;
            if (manipulator != null)
            {
                Debug.Log("This is Manipulator");
                manipulator.gameObject.SetActive(false);
                if (m_context.@object != null)
                {
                    manipulator.transform.SetPositionAndRotation(m_context.@object.position, Quaternion.identity);
                    manipulator.gameObject.SetActive(true);
                    if (m_currentOperatorResult == IOperator.Result.RUNNING_MODAL)
                    {
                        m_operator = (IOperator)manipulator;
                        InvokeCurrentOperator();
                        return;
                    }
                }
            }
            m_operator = op;
            Commons.Logger.Log($"Switch To Operator: {m_operator}");
        }

        private void OnResetContext(InputAction.CallbackContext context)
        {
            ResetContext();
            InputManager.RemoveBackKeyEvent(OnResetContext);
        }

        private void OnCancelManipulator(InputAction.CallbackContext context)
        {
            m_currentOperatorResult = IOperator.Result.CANCELLED;
        }

        public void InvokeCurrentOperator() => m_currentOperatorResult = IOperator.Result.INVOKE; // * Invoke Current Operator
        public void SwitchToMoveOperator() => SwitchOperator(m_operatorManager.Get<MoveOperator>());// TEST
        public void SwitchToMoveManipulator() => SwitchOperator(m_operatorManager.Get<MoveManipulator>());// TEST
    }
}