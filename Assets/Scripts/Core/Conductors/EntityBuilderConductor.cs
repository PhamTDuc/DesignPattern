using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Guinea.Core
{
    public class EntityBuilderConductor : MonoBehaviour
    {
        [SerializeField] InputAction m_action;
        Animator m_animator;
        bool m_gamePlayMode = false;

        void Awake()
        {
            m_animator = GetComponent<Animator>();
        }

        void OnEnable()
        {
            m_action.Enable();
            m_action.performed += OnAction;
        }

        void OnDisable()
        {
            m_action.Disable();
            m_action.performed -= OnAction;
        }

        private void OnAction(InputAction.CallbackContext ctx)
        {
            if (m_gamePlayMode)
            {
                m_animator.Play("EntityBuilderCam");
            }
            else
            {
                m_animator.Play("GameplayCam");
            }
            m_gamePlayMode = !m_gamePlayMode;
        }
    }
}