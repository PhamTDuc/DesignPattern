using UnityEngine;
using Zenject;

namespace Guinea.Core.Components
{
    public class VerticalOperator : MonoBehaviour, IOperator
    {
        Camera m_camera;
        float m_z;
        Vector3 m_offset;

        Vector3 m_prevPos;

        void Awake()
        {
            m_camera = Camera.main;
        }
        public IOperator.Result Cancel(Context context)
        {
            context.@object.position = m_prevPos;
            return IOperator.Result.CANCELLED;
        }

        public IOperator.Result Execute(Context context)
        {
            Vector3 pos = m_offset + m_camera.ScreenToWorldPoint(MousePointOnScreen());
            pos.x = m_prevPos.x;
            pos.z = m_prevPos.z;
            context.@object.transform.position = pos;
            return IOperator.Result.RUNNING_MODAL;
        }

        public ICommand Finish(Context context)
        {
            return null;
        }

        public IOperator.Result Invoke(Context context)
        {
            m_z = m_camera.WorldToScreenPoint(context.@object.position).z;
            m_offset = context.@object.position - m_camera.ScreenToWorldPoint(MousePointOnScreen());

            m_prevPos = context.@object.position;
            return IOperator.Result.RUNNING_MODAL;
        }

        private Vector3 MousePointOnScreen()
        {
            Vector3 mousePos = InputManager.Map.EntityBuilder.Point.ReadValue<Vector2>();
            mousePos.z = m_z;
            return mousePos;
        }
    }
}