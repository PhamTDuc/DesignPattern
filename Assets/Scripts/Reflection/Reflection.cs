using UnityEngine;

namespace Reflection
{
    public static class ReflectionUtils
    {

    }

    namespace Utils
    {
        public static class Utils
        {
            static void PrintHello(string name = null)
            {
                Debug.Log($"Say hello to {name}");
            }

            static int CalculateSum(int a, params int[] args)
            {
                int result = a;

                foreach (int value in args)
                {
                    result += value;
                }
                return result;
            }
        }

        public class Data
        {
            public string name;
            public int age;

        }

    }
}

