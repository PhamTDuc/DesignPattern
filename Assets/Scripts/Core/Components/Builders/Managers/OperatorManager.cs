using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Guinea.Core.Components
{
    public class OperatorManager : MonoBehaviour
    {
        private static Dictionary<Type, IOperator> s_operators = new Dictionary<Type, IOperator>();
        private static Queue<Tuple<IOperator, IOperator.Exec>> s_operatorQueue = new Queue<Tuple<IOperator, IOperator.Exec>>();
        private static List<IOperator> s_operatorStack = new List<IOperator>();
        private static IOperator.Result s_currentOperatorResult;
        private static Context s_context;
        private static int s_currentIndex;
        private static bool s_debug = false;

        [Inject]
        void Initialize(Context context)
        {
            Commons.Logger.Assert(s_context == null, "s_context is already initialized!");
            s_context = context;
            Commons.Logger.Log($"OperatorManager::Initialize() Context: {s_context}");
        }

        void Awake()
        {
            LoadAllOperators();
        }

        // * Load operators from children of OperatorManager
        private void LoadAllOperators()
        {
            var operators = GetComponentsInChildren<IOperator>();
            foreach (var op in operators)
            {
                s_operators.Add(op.GetType(), op);
            }
        }

        // * CleanUp when and preventing Assertion Error when switching Scene
        void Destroy()
        {
            s_operators.Clear();
            s_operatorQueue.Clear();
            s_operatorStack.Clear();
            s_context = null;
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
                            Debug.Log("OperatorManager::OnNoOperatorRunning()--Execute<MoveOperator>()");
                            Execute<MoveOperator>(IOperator.Exec.INVOKE);
                        }
                    }
                }
            }
        }

        private static void OnModalOperator(InputAction.CallbackContext context)
        {
            Commons.Logger.LogIf(s_debug, $"OperatorManager::OnModalRunning(): {s_operatorQueue.Count}-{s_currentOperatorResult}");
            if (s_operatorQueue.Count > 0)
            {
                switch (s_currentOperatorResult)
                {
                    case IOperator.Result.RUNNING_MODAL:
                        s_currentOperatorResult = s_operatorQueue.Peek().Item1.Modal(s_context, InputManager.Map.EntityBuilder);
                        InputManager.Map.EntityBuilder.Select.Enable();
                        InputManager.Map.EntityBuilder.Unselect.Enable();
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
            Debug.Log($"Add Action<{typeof(T)}> to Queue");
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

                switch (s_currentOperatorResult)
                {
                    case IOperator.Result.RUNNING_MODAL:
                        InputManager.Map.EntityBuilder.Get().actionTriggered -= OnNoOperatorRunning;
                        InputManager.Map.EntityBuilder.Select.Disable();
                        InputManager.Map.EntityBuilder.Unselect.Disable();
                        InputManager.Map.EntityBuilder.Get().actionTriggered += OnModalOperator;
                        break;
                    case IOperator.Result.FINISHED:
                        Finish();
                        s_currentOperatorResult = IOperator.Result.PASS_THROUGH;
                        if (s_operatorQueue.Count >= 1) Execute();
                        break;
                    case IOperator.Result.CANCELLED:
                        s_operatorQueue.Dequeue().Item1.Cancel(s_context);
                        s_currentOperatorResult = IOperator.Result.PASS_THROUGH;
                        if (s_operatorQueue.Count >= 1) Execute();
                        break;
                    default:
                        break;
                }
                Commons.Logger.Log($"OperatorManager::Execute(): {op.Item1.GetType()}---Result: {s_currentOperatorResult}");
            }
        }

        public static void Undo()
        {
            if (s_operatorStack.Count > s_currentIndex && s_currentIndex >= 0)
            {
                s_operatorStack.ElementAt(s_currentIndex).Cancel(s_context);
                Commons.Logger.Log($"OperatorManager::Undo()--{s_operatorStack.ElementAt(s_currentIndex).GetType()}");
                s_currentIndex--;
            }

        }

        public static void Redo()
        {
            if (s_operatorStack.Count > s_currentIndex - 1 && s_currentIndex >= -1)
            {
                s_currentIndex++;
                if (s_operatorStack.Count > 0)
                {
                    s_operatorStack.RemoveRange(0, s_currentIndex);
                    if (s_operatorStack.Count > 0) s_operatorStack.ElementAt(0).Execute(s_context);
                    s_currentIndex = 0;
                    Commons.Logger.Log("OperatorManager::Redo()");
                }
            }
        }

        private static void Finish()
        {
            var op = s_operatorQueue.Dequeue().Item1;
            op.Execute(s_context);
            if (s_currentIndex != s_operatorStack.Count - 1)
            {
                s_operatorStack.Clear();
            }
            s_operatorStack.Add((IOperator)op.Clone());
            s_currentIndex = s_operatorStack.Count - 1;
            Debug.Log($"Add to Stack: {s_operatorStack.Count} -- {op.GetType()}");
        }

        public static void Execute<T>(IOperator.Exec exec, params object[] args) where T : IOperator
        {
            IOperator op = s_operators[typeof(T)];
            var opProperties = op.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(field => field.GetCustomAttributes(typeof(OpProperty), false).Length > 0).ToArray();
            Commons.Logger.Assert(args.Length == opProperties.Length, "OperatorManager::Execute(): Args.Length must be equal to opProperties.Length");
            for (int i = 0; i < opProperties.Length; i++)
            {
                opProperties[i].SetValue(op, args[i]);
                Commons.Logger.Log($"OperatorManager::Execute<T>(): Set property {typeof(T)}.{opProperties[i].Name}={args[i]}");
            }
            Execute<T>(exec);
        }
    }
}