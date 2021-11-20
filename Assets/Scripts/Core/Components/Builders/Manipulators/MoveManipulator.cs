using UnityEngine;
using Zenject;

namespace Guinea.Core.Components
{
    public class MoveManipulator : MonoBehaviour, IOperator
    {

        float m_z;
        [SerializeField] float m_scale = 1f;
        Vector3 m_prevMousePos;
        Vector3 m_prevManipulatorPos;
        Camera m_camera;
        ManipulatorHandle.HandleType m_handleType;
        ManipulatorHandle.SpaceType m_spaceType;

        SharedInteraction m_sharedInteraction;

        public ManipulatorHandle.HandleType Handle
        {
            get => m_handleType;
            set
            {
                m_handleType = value;
            }
        }
        public ManipulatorHandle.SpaceType Space
        {
            get => m_spaceType;
            set
            {
                m_spaceType = value;
            }
        }

        [Inject]
        void Initialize(SharedInteraction sharedInteraction)
        {
            m_sharedInteraction = sharedInteraction;
        }
        void Awake()
        {
            m_camera = Camera.main;
            gameObject.SetActive(false);
        }

        void Update()
        {
            transform.localScale = m_scale * Vector3.Distance(m_camera.transform.position, transform.position) / 15f * Vector3.one;
        }

        public IOperator.Result Invoke(Context context)
        {
            m_z = m_camera.WorldToScreenPoint(transform.position).z;
            m_prevMousePos = m_camera.ScreenToWorldPoint(MousePointOnScreen());
            m_prevManipulatorPos = transform.position;
            m_sharedInteraction.ChangeSelectedColor(Color.red);
            Commons.Logger.Log("Invoke Manipulator");
            Commons.Logger.Log($"HandleType: {m_handleType}. SpaceType: {m_spaceType}");
            // switch (m_spaceType)
            // {
            //     case ManipulatorHandle.SpaceType.LOCAL_SPACE:
            //         transform.rotation = context.@object.transform.rotation;
            //         break;
            //     case ManipulatorHandle.SpaceType.WORLD_SPACE:
            //         transform.rotation = Quaternion.identity;
            //         break;
            //     default:
            //         transform.rotation = Quaternion.identity;
            //         break;
            // }
            return IOperator.Result.RUNNING_MODAL;
        }
        public IOperator.Result Execute(Context context)
        {
            if (InputManager.Map.Operator.Pressed.ReadValue<float>() != 0f)
            {
                Vector3 translation = m_camera.ScreenToWorldPoint(MousePointOnScreen()) - m_prevMousePos;
                switch (m_handleType)
                {
                    case ManipulatorHandle.HandleType.ARBITARY:
                        // transform.position = m_prevManipulatorPos + translation;
                        break;
                    case ManipulatorHandle.HandleType.UP_DOWN:
                        transform.position = m_prevManipulatorPos + translation.y * transform.up;
                        break;
                    default:
                        break;
                }
                context.@object.transform.position = transform.position;
                return IOperator.Result.RUNNING_MODAL;
            }
            return IOperator.Result.FINISHED;
        }

        public IOperator.Result Cancel(Context context)
        {
            transform.position = m_prevManipulatorPos;
            context.@object.transform.position = m_prevManipulatorPos;
            InputManager.SetCursorVisible(true);
            Commons.Logger.Log("Cancel Manipulator");
            return IOperator.Result.CANCELLED;
        }

        public ICommand Finish(Context context)
        {
            InputManager.SetCursorVisible(true);
            Commons.Logger.Log("Finish Manipulator");
            return null;
        }

        //// public void Reset()
        //// {
        ////     transform.rotation = Quaternion.identity;
        //// }

        private Vector3 MousePointOnScreen()
        {
            Vector3 mousePos = InputManager.Map.EntityBuilder.Point.ReadValue<Vector2>();
            mousePos.z = m_z;
            return mousePos;
        }
    }
}