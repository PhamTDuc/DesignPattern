using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Guinea.Core.Components
{
    public class OperatorManager : MonoBehaviour
    {
        private static Dictionary<Type, IOperator> s_operators = new Dictionary<Type, IOperator>();
        private static Queue<Tuple<IOperator, IOperator.Exec>> s_operatorQueue = new Queue<Tuple<IOperator, IOperator.Exec>>();
        private static Stack<IOperator> s_operatorStack = new Stack<IOperator>();
        private static IOperator.Result s_currentOperatorResult;
        private static Context s_context;
        private static int s_currentIndex = -1;

        void Awake()
        {
            LoadAllOperators();
        }

        // * Load operators from children of OperatorManager
        void LoadAllOperators()
        {
            var operators = GetComponentsInChildren<IOperator>();
            foreach (var op in operators)
            {
                s_operators.Add(op.GetType(), op);
            }
        }

        public static void Initialize(Context context)
        {
            s_context = context;
            Commons.Logger.Log($"OperatorManager::Initialize() Context: {s_context}");
        }

        void OnEnable()
        {
            InputManager.Map.EntityBuilder.Get().actionTriggered += OnNoOperatorRunning;
        }

        void OnDisable()
        {
            InputManager.Map.EntityBuilder.Get().actionTriggered -= OnNoOperatorRunning;
        }

        private static void OnNoOperatorRunning(InputAction.CallbackContext context)
        {
            if (InputManager.Map.EntityBuilder.Select.triggered)
            {
                Ray ray = Utils.ScreenPointToRay();
                if (Physics.Raycast(ray, out RaycastHit hit, Utils.RayCastLength, Utils.SelectableLayer))
                {
                    if (!String.IsNullOrEmpty(Utils.SelectableTag) && hit.transform.CompareTag(Utils.SelectableTag) || String.IsNullOrEmpty(Utils.SelectableTag))
                    {
                        if (hit.transform != s_context.@object)
                        {
                            s_context.Select(hit.transform);
                        }
                        else
                        {
                            Execute<MoveOperator>(IOperator.Exec.INVOKE);
                        }
                    }
                }
            }
        }

        private static void OnModalOperator(InputAction.CallbackContext context)
        {
            // Commons.Logger.Log($"OperatorManager::OnModalRunning(): {s_operatorQueue.Count}-{s_currentOperatorResult}");
            if (s_operatorQueue.Count > 0)
            {
                switch (s_currentOperatorResult)
                {
                    case IOperator.Result.RUNNING_MODAL:
                        s_currentOperatorResult = s_operatorQueue.Peek().Item1.Modal(s_context, InputManager.Map.EntityBuilder);
                        break;
                    case IOperator.Result.FINISHED:
                        Finish();
                        InputManager.Map.EntityBuilder.Get().actionTriggered -= OnModalOperator;
                        InputManager.Map.EntityBuilder.Get().actionTriggered += OnNoOperatorRunning;
                        if (s_operatorQueue.Count >= 1) Execute();
                        break;
                    case IOperator.Result.CANCELLED:
                        s_currentOperatorResult = s_operatorQueue.Dequeue().Item1.Cancel(s_context);
                        InputManager.Map.EntityBuilder.Get().actionTriggered -= OnModalOperator;
                        InputManager.Map.EntityBuilder.Get().actionTriggered += OnNoOperatorRunning;
                        if (s_operatorQueue.Count >= 1) Execute();
                        break;
                }
            }
        }

        public static void Register<T>() where T : IOperator
        {
            IOperator instance = (IOperator)Activator.CreateInstance(typeof(T));
            s_operators.Add(typeof(T), instance);
        }

        public static void Register<T>(T instance) where T : IOperator
        {
            Commons.Logger.Assert(instance != null, $"You are registering a null instance of {typeof(T)} to OperatorManager");
            s_operators.Add(typeof(T), instance);
        }

        public static void UnRegister<T>() where T : IOperator
        {
            s_operators.Remove(typeof(T));
        }

        public static void Execute<T>(IOperator.Exec exec)
        {
            IOperator op = s_operators[typeof(T)];
            s_operatorQueue.Enqueue(new Tuple<IOperator, IOperator.Exec>(op, exec));
            if (s_operatorQueue.Count == 1) Execute();
        }

        private static void Execute()
        {
            if (s_operatorQueue.Count >= 0)
            {
                var op = s_operatorQueue.Peek();
                if (op.Item2 == IOperator.Exec.INVOKE)
                {
                    s_currentOperatorResult = op.Item1.Invoke(s_context, InputManager.Map.EntityBuilder);
                }
                else
                {
                    s_currentOperatorResult = op.Item1.Execute(s_context);
                }

                Commons.Logger.Log($"OperatorManager::Execute(): {op.Item1.GetType()}---Result: {s_currentOperatorResult}");
                switch (s_currentOperatorResult)
                {
                    case IOperator.Result.RUNNING_MODAL:
                        InputManager.Map.EntityBuilder.Get().actionTriggered -= OnNoOperatorRunning;
                        InputManager.Map.EntityBuilder.Get().actionTriggered += OnModalOperator;
                        break;
                    case IOperator.Result.FINISHED:
                        Finish();
                        if (s_operatorQueue.Count >= 1) Execute();
                        break;
                    case IOperator.Result.CANCELLED:
                        s_currentOperatorResult = s_operatorQueue.Dequeue().Item1.Cancel(s_context);
                        if (s_operatorQueue.Count >= 1) Execute();
                        break;
                    default:
                        break;
                }
            }
        }

        public static void Undo()
        {
            if (s_operatorStack.Count > s_currentIndex && s_currentIndex >= 0)
            {
                Commons.Logger.Log("OperatorManager::Undo()");
                s_operatorStack.ElementAt(s_operatorStack.Count - s_currentIndex - 1).Cancel(s_context);
                s_currentIndex--;
            }
        }

        public static void Redo()
        {
            if (s_operatorStack.Count > s_currentIndex && s_currentIndex >= 0)
            {
                Commons.Logger.Log("OperatorManager::Redo()");
                s_operatorStack.ElementAt(s_operatorStack.Count - s_currentIndex - 1).Execute(s_context);
                s_currentIndex++;
            }
        }

        private static void Finish()
        {
            var op = s_operatorQueue.Dequeue().Item1;
            s_currentOperatorResult = op.Execute(s_context);
            if (s_operatorStack.Count - 1 != s_currentIndex)
            {
                s_operatorStack.Clear();
                s_currentIndex = -1;
            }
            s_operatorStack.Push((IOperator)op.Clone());
            s_currentIndex++;
        }

        public static void Execute<T>(IOperator.Exec exec, params object[] args) where T : IOperator
        {
            IOperator op = s_operators[typeof(T)];
            var opProperties = op.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(field => field.GetCustomAttributes(typeof(OpProperty), false).Length > 0).ToArray();
            Commons.Logger.Assert(args.Length == opProperties.Length, "OperatorManager::TestExecute(): Args.Length must be equal to opProperties.Length");
            for (int i = 0; i < opProperties.Length; i++)
            {
                opProperties[i].SetValue(op, args[i]);
                Commons.Logger.Log($"OperatorManager::Execute<T>(): Set property {typeof(T)}.{opProperties[i].Name}={args[i]}");
            }
            Execute<T>(exec);
        }
    }
}