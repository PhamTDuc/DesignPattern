using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Guinea.Core;

[assembly: AssemblyDescription("This is my custom attribute")]

namespace Guinea.Test
{
    public class TestAttribute : MonoBehaviour
    {
#if UNITY_EDITOR
        private Assembly m_assembly;

        void Awake()
        {
            // TestAssembly();
            // TestCustomAttribute();
            LoadReflectionAssembly();
            // TestBasicReflection();
            TestCreateInstanceUsingReflection();
        }

        void TestAssembly()
        {
            Assembly assembly = typeof(TestAttribute).Assembly;
            AssemblyName name = assembly.GetName();
            Debug.Log($"Assembly Name: {name}");
            Debug.Log($"Assembly Name: {this.GetType().Assembly}");
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            var description = attributes[0] as AssemblyDescriptionAttribute;
            if (description != null)
            {
                Debug.Log($"Description: {description.Description}");
            }
        }

        void TestCustomAttribute()
        {
            Validator.IsValid(typeof(Validator.Employee), "name");
        }

        void LoadReflectionAssembly()
        {
            const string dllFileName = "ReflectionAssembly.dll";
            string dllFilePath = Application.dataPath + "/../Library/ScriptAssemblies/" + dllFileName;
            m_assembly = Assembly.LoadFrom(dllFilePath);
            Debug.Log("Load DLL: " + m_assembly);
        }

        void TestBasicReflection()
        {
            const string targetNameSpace = "Reflection.Utils";
            Type[] types = m_assembly.GetTypes().Where(t => t.Namespace == targetNameSpace).ToArray();
            foreach (Type type in types)
            {
                Debug.Log("Type extracted: " + type);
            }

            MethodInfo printHello = types[0].GetMethod("PrintHello", BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Log($"Method Info: {printHello}");

            if (printHello != null)
            {
                ParameterInfo[] parameterInfos = printHello.GetParameters();
                foreach (ParameterInfo parameterInfo in parameterInfos)
                {
                    Debug.Log($"Parameter Info: {parameterInfo}");
                }
            }
            object[] parameters = new object[] { "Pham Duc" };
            Debug.Log($"Invoke method {printHello.Name}: {printHello.Invoke(null, parameters)}"); // ! Take notice that the stdout stream is not as expected

            MethodInfo calculateSum = types[0].GetMethod("CalculateSum", BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Log($"Method Info: {calculateSum}");
            parameters = new object[] { 1, new int[] { 2, 3, 4, 5, 6 } };
            object result = calculateSum.Invoke(null, parameters);
            Debug.Log($"<color=green><b>Sum of {parameters} is {(int)result}</b></color>");
        }

        void TestCreateInstanceUsingReflection()
        {
            // Type data_t = m_assembly.GetType("Data", true); // ! ERROR
            // Type data_t = m_assembly.GetType("Utils.Data", true); // ! ERROR
            Type data_t = m_assembly.GetType("Reflection.Utils.Data", true); // * Must specify NameSpace in GetType
            object o = Activator.CreateInstance(data_t);
            Debug.Log($"Activator.CreateInstance() create: {o}");
        }
#endif
    }
}