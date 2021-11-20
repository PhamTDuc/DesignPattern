using System;
using UnityEngine;
using System.Collections.Generic;
using Zenject;

namespace Guinea.Core.Components
{
    public class OperatorManager : MonoBehaviour
    {
        [SerializeField] MoveOperator m_moveOperator; // TEST: Operator should be register by client
        [SerializeField] MoveManipulator m_moveManipulator; // TEST: MoveManipulator should be register by client
        [SerializeField] VerticalOperator m_verticalOperator; // TEST: VerticalOperator should be register by client

        Dictionary<Type, IOperator> m_operators = new Dictionary<Type, IOperator>();
        IOperator m_runningOperator;
        IOperator.Result m_currentOperatorResult = IOperator.Result.NO_OPERATOR;

        Context m_context;

        [Inject]
        void Initialize(Context context)
        {
            m_context = context;
        }

        void Awake()
        {
            Register<MoveOperator>(m_moveOperator);
            Register<MoveManipulator>(m_moveManipulator);
            Register<VerticalOperator>(m_verticalOperator);
        }

        void Update()
        {
            if (m_runningOperator == null) return;
            switch (m_currentOperatorResult)
            {
                case IOperator.Result.NO_OPERATOR:
                    m_currentOperatorResult = m_runningOperator.Invoke(m_context);
                    break;
                case IOperator.Result.RUNNING_MODAL:
                    m_currentOperatorResult = m_runningOperator.Execute(m_context);
                    break;
                case IOperator.Result.FINISHED:
                    m_runningOperator.Finish(m_context);
                    m_runningOperator = null;
                    m_currentOperatorResult = IOperator.Result.NO_OPERATOR;
                    break;
                case IOperator.Result.CANCELLED:
                    m_runningOperator.Cancel(m_context);
                    m_runningOperator = null;
                    m_currentOperatorResult = IOperator.Result.NO_OPERATOR;
                    break;
            }

        }

        public void Register<T>() where T : IOperator
        {
            IOperator instance = (IOperator)Activator.CreateInstance(typeof(T));
            m_operators.Add(typeof(T), instance);
        }

        public void Register<T>(T instance) where T : IOperator
        {
            Commons.Logger.Assert(instance != null, $"You are registering a null instance of {typeof(T)} to OperatorManager");
            m_operators.Add(typeof(T), instance);
        }

        public void UnRegister<T>() where T : IOperator
        {
            m_operators.Remove(typeof(T));
        }

        public IOperator Get<T>() where T : IOperator => m_operators[(typeof(T))];
        public void Execute<T>() where T : IOperator
        {
            m_currentOperatorResult = IOperator.Result.CANCELLED;
            m_runningOperator = Get<T>();
        }
    }
}